using IAmGeek.SPOnline.Services.Executors;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAmGeek.SPOnline.Services.Extensions
{
    static class ExtMethods
    {
        public static void AddEntriesToDictionary<T,U>(this IDictionary<T,U> dict, IDictionary<T,U> newEntries)
        {
            // Without mercy it shall bomb if there is a matching key.
            foreach (KeyValuePair<T, U> item in newEntries) {
                dict.Add(item);
            }

        }

        public static List CreateOpenUniDocLibrary(this SiteColManagement siteCol, string address, string listName)
        {
            return siteCol.CreateNewCustomList(address, listName, listName.Replace(" ",""), 10001, Guid.Parse("f7cd43d1-a362-4da9-b512-ca0d4e93b98b"));
        }
        public static List CreateOpenUniDocLibrary(this SiteColManagement siteCol, string address, string listName, string listAddress)
        {
            return siteCol.CreateNewCustomList(address, listName, listAddress, 10001, Guid.Parse("f7cd43d1-a362-4da9-b512-ca0d4e93b98b"));
        }
    }
}
