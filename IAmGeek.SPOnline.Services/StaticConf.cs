using IAmGeek.SPOnline.Config;
using IAmGeek.SPOnline.Configurations;
using IAmGeek.SPOnline.Interfaces;
using IAmGeek.SPOnline.Services;
using IAmGeek.SPOnline.Services.Extensions;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client.Taxonomy;
using Microsoft.SharePoint.Client.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAmGeek.SPOnline
{
    public static class SPOConfiguration
    {
        // Config instance members
        private static IConfigBuilder _configMaster;
        private static IConfigBuilder Config
        {
            get
            {
                return _configMaster;
            }
            set
            {
                _configMaster = value;
            }
        }

        public static void StartUp()
        {

            Config = new ConfigInstance(SPOConfiguration._globalConfig());
            // call some magic stuff here
            // User set properties
            foreach (var AppDetails in _appendAppConfig)
            {
                Config.Properties.AddEntriesToDictionary(AppDetails());
            }

            Config.ServiceData.Add(typeof(TaxonomySession), () =>
            {
                var ts = TaxonomySession.GetTaxonomySession(Config.GlobalContext);
                Config.GlobalContext.Load(ts);
                return ts;
            });

            Config.ServiceData.Add(typeof(PeopleManager), () =>
            {
                return new PeopleManager(Config.GlobalContext);
            });

            Config.ServiceData.Add(typeof(Tenant), () =>
            {
                return new Tenant(Config.GlobalContext);
            });
        }

        public static void AddSiteActionObject<T>(Type type, T objData)
        {
            Config.ObjectData.Add(type, objData);
        }

        private static Func<GlobalOptions> _globalConfig = () => new GlobalOptions();
        public static Func<GlobalOptions> SetGlobalOptions
        {
            set
            {
                if (Config == null)
                {
                    SPOConfiguration._globalConfig = value;
                }
                else
                {
                    throw new ArgumentException("Config already started");
                }
            }
        }

        // MIscellanous Data stored before the app is properly started
        private static ICollection<Func<IDictionary<string, string>>> _appendAppConfig = new List<Func<IDictionary<string, string>>>();
        public static void AddApplicationProperties(Func<IDictionary<string, string>> options)
        {
            // Append to store if before config, or just invoke it on the object
            if (Config == null)
            {
                _appendAppConfig.Add(options);
            }
            else
            {
                Config.Properties.AddEntriesToDictionary(options());
            }

        }

        public static T GetSiteActionObject<T>() where T : class
        {
            return Config.ObjectData[typeof(T)] as T;
        }

        public static void DisposeContext()
        {
            if (Config != null)
            {
                Config.Dispose();
            }
        }


        public static string GetAppProperty(string PropertyName)
        {
            return Config.Properties[PropertyName];
        }

        public static void AddOrUpdateAppProperty(string propertyName, string value)
        {
            if (Config.Properties.ContainsKey(propertyName))
            {
                Config.Properties[propertyName] = value;
            }
            else
            {
                Config.Properties.Add(propertyName, value);
            }
        }

        public static T GetService<T>() where T : class
        {
            return Config.ServiceData[typeof(T)]() as T;

        }


        /// <summary>
        /// Creates a service action, 
        /// including the configuration object
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Description"></param>
        /// <param name="Actions"></param>
        /// <returns></returns>
        public static AppOperation CreateServiceAction(string Name, string Description, Func<IConfigBuilder, bool> Actions)
        {
            return new AppOperation(Config, Name, Description, Actions);

        }
    }
}
