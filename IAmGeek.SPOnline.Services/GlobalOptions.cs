using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IAmGeek.SPOnline.Config
{
    [DataContract]
    public class GlobalOptions
    {
        internal GlobalOptions() { }

        public GlobalOptions(string userName, string passWord, string adminSiteCollection)
        {
            this.UserName = userName;
            this.UserPassword = passWord;
            this.AdminSiteCollection = adminSiteCollection;
        }

        [DataMember]
        internal string UserName { get; set; }
        [DataMember]
        internal string UserPassword { get; set; }
        [DataMember]
        internal string AdminSiteCollection { get; set; }
     }
}
