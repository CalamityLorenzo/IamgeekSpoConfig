using System;
using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using IAmGeek.SPOnline.Config;
using IAmGeek.SPOnline.Services;

namespace IAmGeek.SPOnline.Interfaces
{
    public interface IConfigBuilder : IUserInfo, IDisposable
    {
        IEnumerable<IEnumerable<AppOperation>> ActionStack { get; }
        IDictionary<Type, object> ObjectData { get; }

    }
    
    public interface IUserInfo
    {
        GlobalOptions Options { get; }
        IDictionary<string, string> Properties { get; }
        T GetService<T>() where T : ClientObject;
    }
  
}