using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Referee.DAL;
using Referee.Models;

namespace Referee.Helpers
{
    public class ConfigHelper
    {
        private UOW Unit;
        private Cache CacheRepository;

        public ConfigHelper(UOW _unit)
        {
            Unit = _unit;
            this.CacheRepository = new Cache();
        }

        public string GetValue(string key) 
        {
            string ConfigValue = String.Empty;
           
            var Conf = this.GetRawFromDB(key);
            ConfigValue = Conf.Value;
            
            return ConfigValue;
        }

        public bool SetValue(string key, string val)
        {
            var Conf = this.GetRawFromDB(key);
            if (Conf == null)
            {
                return false;
            }
            Conf.Value = val;
            Unit.ConfigRepository.Update(Conf);
            Unit.Save();
            this.CacheRepository.Remove(key);
            this.CacheRepository.Insert(key, val);
            return true;
        }

        private AppConfig GetRawFromDB(string key)
        {
            return Unit.ConfigRepository.Get(filter: c => c.Key == key).FirstOrDefault();            
        }

        public static string GetAppSettings(string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key];
        }
    }
}