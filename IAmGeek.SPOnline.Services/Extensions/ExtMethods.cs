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
    }
}
