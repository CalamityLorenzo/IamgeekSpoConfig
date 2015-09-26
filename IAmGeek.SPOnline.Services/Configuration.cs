using IAmGeek.SPOnline.Config;
using IAmGeek.SPOnline.Services;
using IAmGeek.SPOnline.Services.Extensions;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using Microsoft.SharePoint.Client.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAmGeek.SPOnline
{
    public class Configuration
    {
        // MIscellanous Data
        static ICollection<Func<IDictionary<string, string>>> _appendAppConfig = new List<Func<IDictionary<string, string>>>();
        public readonly IDictionary<string, string> _appConfig;

        internal readonly IDictionary<Type, Func<ClientObject>> _serviceData;
        // For passing data between action processors
        public readonly IDictionary<Type, object> _objectData;

        // Not ready for use yet
        private readonly IEnumerable<IEnumerable<ServiceAction>> _actionStack;
        private readonly ClientContext _globalContext;

        private static Configuration _configurator = null;
        readonly GlobalOptions Options;

        static Func<GlobalOptions> _globalConfig;
        static Func<GlobalOptions> GlobalOptions
        {
            get
            {
                if (_globalConfig == null)
                {
                    _globalConfig = Utils.BasicOptions();
                }

                return _globalConfig;
            }

            set
            {
                if (_globalConfig == null)
                {
                    _globalConfig = value;
                }
                else
                {
                    throw new ArgumentException("Options are already set");
                }
            }
        }

        private static Configuration Config
        {
            get
            {
                return _configurator;
            }
            set
            {
                _configurator = value;
            }
        }

        private Configuration()
        {
            this._appConfig = new Dictionary<string, string>();
            this.Options = Configuration.GlobalOptions();
            this._objectData = new Dictionary<Type, object>();
            this._serviceData = new Dictionary<Type, Func<ClientObject>>();
            this._globalContext = new ClientContext(this.Options.AdminSiteCollection);
            this._globalContext.Credentials = Utils.SPOCredentials(Options.UserName, Options.UserPassword);
        }

        public void DisposeGlobalContext()
        {
            this._globalContext.Dispose();
        }

        public static GlobalOptions StartUpOptions
        {
            get
            {
                return Config.Options;
            }
        }

        public static IDictionary<string, string> UserOptions
        {
            get
            {
                return Config._appConfig;
            }
        }

        public static void StartUp()
        {
            // call some magic stuff here
            //Now instantiate.
            Config = new Configuration();
            // User set properties
            foreach(var AppDetails in _appendAppConfig)
            {
                Config._appConfig.AddEntriesToDictionary(AppDetails());
            }

            Config._serviceData.Add(typeof(TaxonomySession), () =>
            {
                var ts = TaxonomySession.GetTaxonomySession(Config._globalContext);
                Config._globalContext.Load(ts);
                return ts;
            });

            Config._serviceData.Add(typeof(PeopleManager), () =>
            {
                return new PeopleManager(Config._globalContext);
            });

            Config._serviceData.Add(typeof(Tenant), () =>
            {
                return new Tenant(Config._globalContext);
            });

        }

        public static void DisposeContext()
        {
            Config.DisposeGlobalContext();
        }

        public static T GetService<T>() where T : ClientObject
        {
            return Config._serviceData[typeof(T)]() as T;
        }

        public static string GetAppProperty(string PropertyName)
        {
            return Config._appConfig[PropertyName];
        }

        public static void AddOrUpdateAppProperty(string propertyName, string value)
        {
            if (Config._appConfig.ContainsKey(propertyName)){
                Config._appConfig[propertyName] = value;
            }
            else
            {
                Config._appConfig.Add(propertyName, value);
            }
        }

        public static Func<GlobalOptions> SetGlobalOptions
        {
            set
            {
                if (_configurator == null)
                {
                    Configuration._globalConfig = value;
                }
                else {
                    throw new ArgumentException("Config already started");
                }
            }
        }

        public static void AddApplicationProperties(Func<IDictionary<string, string>>  options)
        {
                // Append to store if before config, or just invoke it on the object
                if(Config == null)
                {
                    _appendAppConfig.Add(options);
                }
                else
                {
                    Config._appConfig.AddEntriesToDictionary(options());
                }
            
        }

        //static IDictionary<string, string> _actionOptions = new Dictionary<string, string>();
        //public static void SetApplicationProperties(IDictionary<string, string> Options)
        //{
        //    if (Config == null)
        //    {
        //        // this appends and appends and appends.
        //        _actionOptions.AddEntriesToDictionary(Options);
        //    }
        //    else
        //    {
        //        Config._appConfig.AddEntriesToDictionary(Options);
        //    }
        //}

        //public static void SetApplicationProperties(IEnumerable<KeyValuePair<string, string>> Options)
        //{
        //    SetApplicationProperties(Options.ToDictionary(x => x.Key, y => y.Value));
        //}

        public static void AddSiteActionObject<T>(Type type, T objData)
        {
            Config._objectData.Add(type, objData);
        }

        public static T GetSiteActionObject<T>() where T : class
        {
            return Config._objectData[typeof(T)] as T;
        }

    }
}
