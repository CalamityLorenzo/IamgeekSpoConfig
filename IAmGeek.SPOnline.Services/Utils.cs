using IAmGeek.SPOnline.Config;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IAmGeek.SPOnline.Services
{
    public class Utils
    {
        public static SharePointOnlineCredentials SPOCredentials(string username, string password)
        {
            var securePass = new SecureString();
            foreach (var item in password)
            {
                securePass.AppendChar(item);
            }
            return new SharePointOnlineCredentials(username, securePass);
        }

        public static Func<ClientContext> GetClientContext(string url, string username, string password)
        {
            return new Func<ClientContext>(() =>
            {
                var creds = Utils.SPOCredentials(username, password);
                var clientContext = new ClientContext(url);
                clientContext.Credentials = creds;
                return clientContext;
            });

        }

        internal static void WaitForOperation(SpoOperation longRunningOperation, String operation = "")
        {
            if (!string.IsNullOrEmpty(operation))
            {
                Console.WriteLine("Waiting for : {0}", operation);
            }

            var polTime = longRunningOperation.PollingInterval;

            while (!longRunningOperation.IsComplete && !longRunningOperation.HasTimedout)
            {
                Thread.Sleep(polTime);
                Console.Write("...");
                longRunningOperation.Context.Load(longRunningOperation);
                longRunningOperation.Context.ExecuteQuery();
            }


            if (longRunningOperation.IsComplete)
            {
                Console.WriteLine("Completed");
            }

        }

        internal static Func<GlobalOptions> BasicOptions()
        {
            return () => new GlobalOptions();
        }
    }



}
