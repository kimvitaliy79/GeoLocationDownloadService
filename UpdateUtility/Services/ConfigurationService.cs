using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateUtility.Services
{
    static class ConfigurationService
    {
        public static int AfterDaysToRunApp
        {
            get
            {
                int AfterDaysToRunApp = 0;
                string days = ConfigurationManager.AppSettings["AfterDaysToRunApp"];
                if (!string.IsNullOrEmpty(days))
                {
                    Int32.TryParse(days, out AfterDaysToRunApp);
                }
                return AfterDaysToRunApp;
            }
        }

        public static string DownloadFileHref
        {
            get
            {
                return ConfigurationManager.AppSettings["DownloadFileHref"];                
            }
        }

        public static string PathForDownload
        {
            get
            {
                return ConfigurationManager.AppSettings["PathForDownload"];                
            }
        }
        public static bool AlwaysUpdate
        {
            get
            {               
                return Convert.ToBoolean(ConfigurationManager.AppSettings["AlwaysUpdate"]);
            }
        }




    }
}
