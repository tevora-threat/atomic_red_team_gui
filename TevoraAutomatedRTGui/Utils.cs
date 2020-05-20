using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using CsvHelper;
using System.Threading;
using System.Management.Automation; 
namespace TevoraAutomatedRTGui
{
    static class Utils
    {

        public static string LoadYamlFromPath(string path)
        {

            string yaml_name = Path.GetFileName(path) + ".yaml";
            // Open the text file using a stream reader.
            using (StreamReader sr = new StreamReader(Path.Combine(path, yaml_name)))
            {
                // Read the stream to a string, and write the string to the console.
                String yaml = sr.ReadToEnd();
                return yaml;
            }



        }
        public static void Log(string message)
        {
        }

        /* https://stackoverflow.com/questions/1469764/run-command-prompt-commands */
        public static string RunCommandInBackground(string command, string name, string context_dir =@"c:\users\public")
  
        {
            
                var proc1 = new ProcessStartInfo();

                proc1.UseShellExecute = false;
            proc1.CreateNoWindow = true;
            proc1.WindowStyle = ProcessWindowStyle.Hidden;


            proc1.WorkingDirectory = context_dir;

                if (name == "powershell")
                {
                command = "\"" + command.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace('\n',';') + "\"";
                    command = "Powershell.exe -Command " + command;
                }
                else
                {
                command = command.Replace("\n", "&");
                }
                proc1.FileName = @"C:\Windows\System32\cmd.exe";
                //proc1.Verb = "runas";
                proc1.Arguments = "/c " + command;


                proc1.RedirectStandardError = true;
                var process = new Process {
                    StartInfo = proc1 };
            StringBuilder error = new StringBuilder();

            using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
            {
                process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data == null)
                { 
                    try
                    {
                        errorWaitHandle.Set();
                    }
                    catch
                    {

                    }
                }
                else
                {
                    error.AppendLine(e.Data);
                }
            };

                process.Start();
                int timeout = 30000;
                process.BeginErrorReadLine();

                if (process.WaitForExit(timeout) &&
                errorWaitHandle.WaitOne(timeout))
                {

                }
                else
                {

                    process.Kill();
                }


                return error.ToString();
            }

        }
        public static void ReportToCsv(Report report, string output_dir, string prefix="redteamsim_log_")
        {

            string path = Path.Combine(output_dir,prefix + report.started.ToString("yyyyMMddHHmmssfff") + ".csv");
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteRecords(report.results);
            }
        }


        public static string EvaluatePS(string expression)
        {

            PowerShell psinstance = PowerShell.Create().AddScript(expression);
            try{
                var results = psinstance.Invoke();

                return results[0].ToString();
              
            }
            catch (Exception e)
            {
                return "Powershell Execution Failed " + e ;
            }

        }
        public static void RunnablesToCsv(List<AtomicRunnable> report, string output_dir)
        {

            string path = Path.Combine(output_dir, "testplan_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv");
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteRecords(report);
            }
        }
    }
}
