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

  
}