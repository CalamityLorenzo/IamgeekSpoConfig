using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using Microsoft.SharePoint.Client.Taxonomy;

namespace IAmGeek.SPOnline.Services.Registrations
{
    class SPOnlineUserProfile : ServiceRegister
    {
        private PeopleManager pm;
        
        public SPOnlineUserProfile(string siteAddress, SharePointOnlineCredentials creds) : base(siteAddress, creds)
        {

        }

    }
}
