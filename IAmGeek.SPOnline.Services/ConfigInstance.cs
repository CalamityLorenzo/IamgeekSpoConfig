using IAmGeek.SPOnline.Config;
using IAmGeek.SPOnline.Interfaces;
using IAmGeek.SPOnline.Services;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAmGeek.SPOnline.Configurations
{
    public class ConfigInstance : ConfigBase, IDisposable
    {
        // Application Aka SiteCollection config info
        private readonly IDictionary<string, string> appConfig;
        //
        private readonly IDictionary<Type, Func<ClientObject>> serviceData;
        // For passing data between action processors
        private readonly IDictionary<Type, object> objectData;

        // Not ready for use yet
        private readonly IEnumerable<IEnumerable<AppOperation>> _actionStack;
        /// <summary>
        /// Tenancy level SharePoint context
        /// </summary>
        private readonly ClientContext _globalContext;

        public override IDictionary<string, string> Properties
        {
            get
            {
                return appConfig;
            }
        }

        public override IDictionary<Type, Func<ClientObject>> ServiceData
        {
            get
            {
                return serviceData;
            }
        }

        public override IDictionary<Type, object> ObjectData
        {
            get
            {
                return objectData;
            }
        }

        public override IEnumerable<IEnumerable<AppOperation>> ActionStack
        {
            get
            {
                return _actionStack;
            }
        }

        public override ClientContext GlobalContext
        {
            get
            {
                return _globalContext;
            }
        }

        public ConfigInstance(GlobalOptions GlobalOptions) : base(GlobalOptions)
        {
            this.appConfig = new Dictionary<string, string>();
            this.objectData = new Dictionary<Type, object>();
            this.serviceData = new Dictionary<Type, Func<ClientObject>>();
            this._globalContext = new ClientContext(this.Options.AdminSiteCollection);
            this._globalContext.Credentials = Utils.SPOCredentials(this.Options.UserName, this.Options.UserPassword);
        }

        public override void Dispose()
        {
            this.GlobalContext.Dispose();
        }
    }
}
