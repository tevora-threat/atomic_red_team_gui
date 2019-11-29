using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace TevoraAutomatedRTGui
{

    class Report
    {
        public DateTime started;
        public DateTime finished;
        public List<ReportEntry> results;
    }

    class ReportEntry
    {
        public string time { get; set; }
        public string command { get; set; }
        public string attack_technique { get; set; }
        public string display_name { get; set; }
        public string error { get; set; }
        public bool success { get; set; }
    }

    class AtomicEngine
    {
        private string atomicLocation;
        public List<Atomic> atomics { get; set; }
        public List<AtomicRunnable> atomic_runnables { get; set; }

        public AtomicEngine(string atomicLocation, string outputLocation, string config_path)
        {
            this.atomicLocation = atomicLocation;
            this.atomics = new List<Atomic>();
            var atomic_dirs = Directory.EnumerateDirectories(atomicLocation).OrderBy(filename => filename);


            foreach (string atomic_dir in atomic_dirs)
            {
                var atomic = new Atomic(atomic_dir);
                this.atomics.Add(atomic);

            }
            this.GenerateRunables();

        }
        private void GenerateRunables()
        {
            if (this.atomics == null)
            {
                return;
            }

            this.atomic_runnables = new List<AtomicRunnable>();
            foreach (Atomic atomic in this.atomics)
            {
                foreach (AtomicTest atomic_test in atomic.atomic_yaml.atomic_tests)
                {
                    if (!atomic_test.supported_platforms.Contains("windows"))
                    {
                        continue;
                    }
                    if (atomic_test.executor.steps != null && atomic_test.executor.command == null)
                    {
                        continue;
                    }
                    string command = atomic_test.executor.command;
                    if (atomic_test.input_arguments != null)
                    {

                        Console.WriteLine(atomic.atomic_yaml.attack_technique + ": " + command);
                        foreach (KeyValuePair<string, InputArgument> argument in atomic_test.input_arguments)
                        {
                            string replace_target = "#{" + argument.Key + "}";

                            if (atomic.args.ContainsKey(argument.Key))
                            {
                                command = command.Replace(replace_target, atomic.args[argument.Key]);
                            }
                            else
                            {
                                if(argument.Value.type =="Path" || argument.Value.type == "path")
                                {
                                    if (argument.Value.default_value == "")
                                    {
                                        argument.Value.default_value = Path.Combine(this.atomicLocation, atomic.atomic_yaml.attack_technique);
                                    }
                                    else
                                    {
                                        argument.Value.default_value = Path.Combine(this.atomicLocation, atomic.atomic_yaml.attack_technique, argument.Value.default_value);
                                    }

                                }
                                command = command.Replace(replace_target, argument.Value.default_value);

                            }
                        }
                    }
                    Console.WriteLine(command);
                    AtomicRunnable atomic_runnable = new AtomicRunnable();
                    atomic_runnable.command = command;
                    atomic_runnable.display_name = atomic.atomic_yaml.display_name;
                    atomic_runnable.name = atomic_test.name;
                    atomic_runnable.type = atomic_test.executor.name;

                    atomic_runnable.attack_technique = atomic.atomic_yaml.attack_technique; 
                    this.atomic_runnables.Add(atomic_runnable);
                }
            }
        }
        public void ExportTestPlan(MainWindow window)
        {
            try {
                Utils.RunnablesToCsv(this.atomic_runnables, window.output_dir);
            }
            catch
            {
             
            }
            
        }
     
        public async Task<Report> RunTests(MainWindow window)
        {
            

            Report report = new Report {
                started = DateTime.Now,
                results = new List<ReportEntry>(),
                finished = DateTime.Now

        };
     
            int i = 0;
            foreach (AtomicRunnable atomic in this.atomic_runnables)

            {
                string[] commands;
                if(atomic.type=="command_prompt")
                {
                    commands = atomic.command.Split('\n');
                }
                else
                {
                    commands = new string[1];
                    commands[0] = atomic.command; 
                }
                foreach (string command in commands)

                {
                    if(command =="")
                    {
                        continue;
                    }
                    SimulationStatusUpdate status = new SimulationStatusUpdate
                    {
                        number = i,
                        message = "Running " + atomic.attack_technique + ": " + command
                    };
                    window.UpdateStatus(status);

                    ReportEntry entry = new ReportEntry
                    {
                        time = DateTime.Now.ToString("u"),
                        command = command,
                        display_name = atomic.display_name,
                        attack_technique = atomic.attack_technique,
                        success = false
                    };

                    try
                    {
                        string exitcode = Utils.RunCommandInBackground(command, atomic.type);

                        entry.error = exitcode;
                        entry.success = true;

                    }
                    catch (Exception e)
                    {
                        entry.error = e.Message;

                    }
                    report.results.Add(entry);
                }
                Thread.Sleep(100);
                i++;
                Utils.ReportToCsv(report, window.output_dir);
            }
            report.finished = DateTime.Now;
            SimulationStatusUpdate finalstatus = new SimulationStatusUpdate
            {
                number = i,
                message = "Completed!"
            };
            window.UpdateStatus(finalstatus);

            return report; 
        }
    }
}
