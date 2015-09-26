using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sp = Microsoft.SharePoint.Client;

namespace IAmGeek.SPOnline.Services.Registrations
{
    abstract class ServiceRegister
    {
        private readonly string siteAddress;
        protected readonly sp.SharePointOnlineCredentials credentials;
        protected sp.ClientContext ctx = null;
        protected sp.ClientObject service;
        public ServiceRegister(string siteAddress, sp.SharePointOnlineCredentials creds) {
            this.siteAddress = siteAddress;
            this.credentials = creds;
        }

        public abstract object ReturnService();

        public abstract object ReturnService(sp.ClientContext ctx);

        protected T InstantiateService<T>(Func<sp.ClientContext,T> GetService) where T : sp.ClientObject
        {
            if (ctx == null)
            {
                using(ctx = new sp.ClientContext(this.siteAddress))
                {
                    service = GetService(ctx);
                    return service as T;
                }
            }
            return service as T;
        }

        protected T InstantiateService<T>(sp.ClientContext ctx, Func<sp.ClientContext, T> GetService) where T : sp.ClientObject
        {
            return GetService(ctx);
        }

    }
}
