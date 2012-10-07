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
        public static string GetAppSettings(string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key];
        }
    }
}