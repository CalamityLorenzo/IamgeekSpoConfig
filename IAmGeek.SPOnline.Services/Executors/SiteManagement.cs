using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAmGeek.SPOnline.Services
{
    public class SiteManagement
    {
        private readonly Tenant Tenant;
        private static SiteManagement SiteManager = null;

        private SiteManagement()
        {
            this.Tenant = SPOConfiguration.GetService<Tenant>();
        }

        public bool DoesSiteCollectionExist(string siteUrl)
        {
            Site site = null;

            // These are seriously weird, it handles ServerExceptions
            // note you MUST have a finally or a catch (Just like the real thing)
            // Also all this codes executes (On the client) BEFORE you call execute query
            // Then it executes on the server.

            ExceptionHandlingScope scope = new ExceptionHandlingScope(Tenant.Context);
            using (scope.StartScope())
            {
                using (scope.StartTry())
                {
                    site = Tenant.GetSiteByUrl(siteUrl);
                }
                using (scope.StartFinally())
                {
                    // Any ideas?
                }
            }

            Tenant.Context.ExecuteQuery();

            return (!scope.HasException);


        }

        public bool CreateNewSite(string SiteCollection, string Title, string SiteOwnerEmail, string WebTemplate = "")
        {

            SiteCreationProperties scp = new SiteCreationProperties();

            scp.Lcid = 1033;
            scp.Owner = SiteOwnerEmail;
            scp.StorageMaximumLevel = 100;
            scp.UserCodeMaximumLevel = 300;
            scp.TimeZoneId = 2;
            scp.Title = Title;
            scp.Url = SiteCollection;

            if (!String.IsNullOrEmpty(WebTemplate))
            {
                scp.Template = WebTemplate;
            }
            var longOperation = Tenant.CreateSite(scp);
            Tenant.Context.Load(longOperation);
            Tenant.Context.ExecuteQuery();

            Utils.WaitForOperation(longOperation, "Create site");
            return true;

        }

        public bool DeleteSite(string SiteCollection, bool EmptyRecycleBin)
        {
            var deleteOperation = Tenant.RemoveSite(SiteCollection);

            Tenant.Context.Load(deleteOperation);
            Tenant.Context.ExecuteQuery();
            Utils.WaitForOperation(deleteOperation, "Deleting Site");

            if (EmptyRecycleBin)
            {
                var emptyBin = Tenant.RemoveDeletedSite(SiteCollection);
                Tenant.Context.Load(emptyBin);
                Tenant.Context.ExecuteQuery();
                Utils.WaitForOperation(emptyBin, "Emptying Site from Recycle bin");
            }
            return true;
        }
        
        public static SiteManagement Manager
        {
            get
            {
                if (SiteManager == null)
                {
                    SiteManager = new SiteManagement();
                }
                return SiteManager;
            }
        }
    }
}
