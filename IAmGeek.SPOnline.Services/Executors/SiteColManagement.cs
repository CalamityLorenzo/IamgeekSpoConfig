using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Publishing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAmGeek.SPOnline.Services.Executors
{
    public class SiteColManagement : IDisposable
    {
        private readonly ClientContext ctx;
        private readonly Web currentWeb;
        private readonly Site currentSite;
        private readonly List solutionsGallery;
        private readonly List masterPageGallery;

        public SiteColManagement(string userName, string password, string siteAddress)
        {
            var creds = Utils.SPOCredentials(userName, password);

            this.ctx = new ClientContext(siteAddress);
            this.ctx.Credentials = creds;
            this.currentWeb = this.ctx.Web;
            this.solutionsGallery = ctx.Web.Lists.GetByTitle("Solution Gallery");
            this.masterPageGallery = ctx.Web.Lists.GetByTitle("Master Page Gallery");
            this.currentSite = ctx.Site;

            this.ctx.Load(currentSite);
            this.ctx.Load(currentWeb);
            
            this.ctx.ExecuteQuery();
        }

        public void UploadAndActivateSolution(string solutionPath, string solutionName)
        {
            // get the solutions gallery
            var address = this.currentWeb.ServerRelativeUrl;

            this.ctx.Load(masterPageGallery, sf => sf.RootFolder);
            var solutionsFolder = this.masterPageGallery.RootFolder;

            using (FileStream fs = new FileStream(solutionPath, FileMode.Open))
            {
                FileCreationInformation fci = new FileCreationInformation()
                {
                    ContentStream = fs,
                    Url = solutionName,
                    Overwrite = true // we want to fail
                };
                var uploadedFile = solutionsFolder.Files.Add(fci);

                ctx.Load(uploadedFile);
                ctx.ExecuteQuery();
            }
            // Creates DeisgnPackInfo then activates
            ActivateSolution(solutionName);

        }

        public void ActivateSolution(string solutionName)
        {

            var packageName = solutionName.EndsWith(".wsp") ? solutionName.Substring(0, solutionName.Length - 4) : solutionName;
            var wspPackage = new DesignPackageInfo()
            {
                PackageGuid = Guid.Empty,
                PackageName = packageName,
            };
            InstallSolution(wspPackage, solutionName);
        }

        public void ActivateSolution(string solutionName, int MajorVersion, int MinorVersion)
        {
            var packageName = solutionName.EndsWith(".wsp") ? solutionName.Substring(0, solutionName.Length - 4) : solutionName;
            var wspPackage = new DesignPackageInfo()
            {
                MajorVersion = MajorVersion,
                MinorVersion = MinorVersion,
                PackageGuid = Guid.Empty,
                PackageName = packageName,
            };
            InstallSolution(wspPackage, solutionName);
        }

        private void InstallSolution(DesignPackageInfo wspInfo, string solutionName)
        {
            var packageName = solutionName.EndsWith(".wsp") ? solutionName : solutionName + ".wsp";
            try
            {
                var solutionFolder = masterPageGallery.RootFolder;
                ctx.Load(solutionFolder);
                var fileUrl = solutionFolder.ServerRelativeUrl + "/" + packageName;
                DesignPackage.Install(ctx, ctx.Site, wspInfo, fileUrl);
                ctx.ExecuteQuery();
            }
            catch(Exception ex)
            {
                Console.WriteLine("{0} install failed", packageName);
                Console.WriteLine("{0}\r\n{1}", ex.Message, ex.StackTrace);
            }
        }

        public void ApplyWebTemplate(string TemplateId)
        {
            this.ctx.Web.ApplyWebTemplate(TemplateId);
            this.ctx.ExecuteQuery();
        }

        public void Dispose()
        {
            if (this.ctx != null)
            {
                this.ctx.Dispose();
            }
        }

        public Web CreateNewSubSite(string webName, string webAddress, string webTemplateId, string parentRelativeAddress, bool deleteIfexists)
        {
            var parentWeb = this.currentSite.OpenWeb(parentRelativeAddress);
            this.ctx.Load(parentWeb, pa=>pa.Webs, pa => pa.Webs.Include(wb => wb.Title));
            this.ctx.ExecuteQuery();
            var alreadyExists = parentWeb.Webs.FirstOrDefault(wb => wb.Title == webName);

            if(alreadyExists!=null && !deleteIfexists)
            {
                Console.WriteLine("{0} already exists, exiting createsite.", webName);
                return null;
            }

            if(alreadyExists!=null && deleteIfexists)
            {
                Console.WriteLine("{0} deleting existing site.", webName);
                alreadyExists.DeleteObject();
                ctx.ExecuteQuery();
            }

            WebCreationInformation wci = new WebCreationInformation
            {
                Title = webName,
                Url = webAddress,
                Language = 1033,
                WebTemplate = webTemplateId,
                UseSamePermissionsAsParentSite = true
            };

          var newWeb =   parentWeb.Webs.Add(wci);
            ctx.Load(newWeb);
            ctx.ExecuteQuery();

            return newWeb;
        }

        /// <summary>
        /// Create list definied in a custom solution
        /// </summary>
        /// <param name="webRelAddress"></param>
        /// <param name="listName"></param>
        /// <param name="listTitle"></param>
        /// <returns></returns>
        public List CreateNewCustomList(string webRelAddress, string listName, string listTitle, int TemplateId, Guid TemplateFeatureId)
        {
            ListCreationInformation lci = new ListCreationInformation
            {
                Title = listTitle,
                Url = listName,
                TemplateFeatureId = TemplateFeatureId
            };

            var cWeb = this.ctx.Site.OpenWeb(webRelAddress);
            this.ctx.ExecuteQuery();
            var list = cWeb.Lists.Add(lci);
            this.ctx.ExecuteQuery();
            return list;
        }

    }
}
