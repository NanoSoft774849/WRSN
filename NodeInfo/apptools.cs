using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;
namespace NodeInfo
{
    public static class ns_appconfig
    {

        public static string getConfigValue(string key)
        {
            string value = "";
            try
            {
                //  value = ConfigurationSettings.AppSettings.Get(key);
                System.Configuration.AppSettingsReader reader = new AppSettingsReader();
                value = reader.GetValue(key, typeof(string)).ToString();
            }
            catch (Exception)
            {
                value = "";
            }
            return value;
        }
    public static string getImagesPath()
        {
            return getConfigValue("images_path");
        }
}
//----------    
}
