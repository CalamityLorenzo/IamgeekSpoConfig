using System;
using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using IAmGeek.SPOnline.Config;
using IAmGeek.SPOnline.Services;

namespace IAmGeek.SPOnline.Interfaces
{
    public interface IConfigBuilder : IDisposable
    {
        IEnumerable<IEnumerable<AppOperation>> ActionStack { get; }
        IDictionary<string, string> Properties { get; }
        IDictionary<Type, object> ObjectData { get; }
        IDictionary<Type, Func<ClientObject>> ServiceData { get; }
        ClientContext GlobalContext { get; }
        GlobalOptions Options { get; }

    }

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