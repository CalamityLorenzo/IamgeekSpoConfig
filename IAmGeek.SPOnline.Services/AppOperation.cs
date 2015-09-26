using IAmGeek.SPOnline.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAmGeek.SPOnline.Services
{
    /// <summary>
    /// Performs an applicatiopn operation and returns true.
    /// You provide the process, it provides a container with the configuration information.
    /// </summary>
    public class AppOperation
    {
        private readonly IConfigBuilder Config;

        private readonly Func<IConfigBuilder, bool> task;
        
        public AppOperation(string Name, string Description) {
            this.Name = Name;
            this.Description = Description;
        }

        public AppOperation(string Name, string Description, Func<IConfigBuilder, bool> serviceTask):this(Name, Description)
        {
            task = serviceTask;
        }

        internal AppOperation(IConfigBuilder configOptions, string Name, string Description) : this(Name, Description)
        {
            Config = configOptions;
        }

        internal AppOperation(IConfigBuilder configOptions, string Name, string Description, Func<IConfigBuilder, bool> serviceTask) : this(Name, Description, serviceTask)
        {
            this.Config = configOptions;
        }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public virtual bool Execute() {

            if (task != null)
            {
                return task(this.Config);
            }

            return false;
        }

        public override string ToString()
        {
            return Name + " " + Description;
        }
    }
}
