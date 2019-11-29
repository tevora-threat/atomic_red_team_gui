using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TevoraAutomatedRTGui
{


    class InputArgument
    {

        public string description;
        public string type{get; set;}
        [YamlMember(typeof(string), Alias ="default")]
        public string default_value{get; set;}
    }

    class Executor
    {
        public string name{get; set;}
        public string command{get; set;}
        public string steps { get; set; }

    }

    // The test, deserialized from the YAML
    class AtomicTest
    {
        public string name{get; set;}
        public string description{get; set;}
        public Dictionary<string, InputArgument> input_arguments{get; set;}
        public HashSet<string> supported_platforms{get; set;}
        public Executor executor { get; set; }
    }

    class AtomicYaml
    {
        public string attack_technique { get; set; }
        public string attack_link { get; set; }

        public string display_name { get; set; }
        public List<AtomicTest> atomic_tests { get; set; }
    }

    class AtomicRunnable
    {
      public string display_name { get; set; }
      public string name { get; set; }
       public string command { get; set; }
       public string attack_technique { get; set; }
        public string type { get; set; }


    }


    // The atomic that runs all the atomic tests in an atomic YAML
    class Atomic
    {
        private string yaml;
        private string path;
        public AtomicYaml atomic_yaml { get; set; }
        public Dictionary<string, string> args;
        private List<AtomicRunnable> atomic_runnables;

        public Atomic(string path, Dictionary<string, string> args = null )
        {


            if (args == null)
            {
                this.args = new Dictionary<string, string>();
            }
            else
            {
                this.args = args;

            }
            this.path = path;
            this.yaml = Utils.LoadYamlFromPath(path);

            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder().Build();
            this.atomic_yaml = deserializer.Deserialize<AtomicYaml>(this.yaml);

        }


    
    

        public void RunTest()
        {
            foreach (AtomicRunnable atomic_runnable in this.atomic_runnables)
            {
                
            

                Console.WriteLine("Running command " + atomic_runnable.command);
              //  Utils.RunCommandInBackground(command);

            }
        }
    }
}
