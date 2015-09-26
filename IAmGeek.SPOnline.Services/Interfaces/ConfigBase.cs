using IAmGeek.SPOnline.Config;
using IAmGeek.SPOnline.Services;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAmGeek.SPOnline.Interfaces
{
    public abstract class ConfigBase : IConfigBuilder
    {
        private readonly GlobalOptions _globalOptions;
        public ConfigBase(GlobalOptions GlobalOptions)
        {
            _globalOptions = GlobalOptions;
        }

        public virtual IEnumerable<IEnumerable<AppOperation>> ActionStack
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual ClientContext GlobalContext
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual IDictionary<Type, object> ObjectData
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public GlobalOptions Options
        {
            get
            {
                return _globalOptions;
            }

        }

        public virtual IDictionary<string, string> Properties
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual IDictionary<Type, Func<ClientObject>> ServiceData
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
