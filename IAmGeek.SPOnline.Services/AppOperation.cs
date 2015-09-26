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

        private readonly Func<IConfigBuilder, bool> appOperation;

        public string Name { get; private set; }

        public string Description { get; private set; }

        public AppOperation(string Name, string Description)
        {
            this.Name = Name;
            this.Description = Description;
        }

        public AppOperation(string Name, string Description, Func<IConfigBuilder, bool> appOperation) : this(Name, Description)
        {
            this.appOperation = appOperation;
        }

        internal AppOperation(IConfigBuilder configOptions, string Name, string Description) : this(Name, Description)
        {
            Config = configOptions;
        }

        internal AppOperation(IConfigBuilder configOptions, string Name, string Description, Func<IConfigBuilder, bool> appOperation) : this(Name, Description, appOperation)
        {
            this.Config = configOptions;
        }

        public virtual bool Execute()
        {

            if (appOperation != null)
            {
                return appOperation(this.Config);
            }

            return false;
        }

        public virtual Task<bool> ExecuteAsync()
        {

            Task<bool> TaskExecutor;
            if (appOperation != null){
                TaskExecutor = new Task<bool>(() => appOperation(this.Config));
            }
            else
            {
                TaskExecutor = new Task<bool>(() => false);
            }

            return TaskExecutor;
        }

        public override string ToString()
        {
            return Name + " " + Description;
        }
    }
}
