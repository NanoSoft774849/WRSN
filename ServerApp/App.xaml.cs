using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ServerApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string getAppSettingValue(string key)
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
        public static string GetDeviceInfoFile()
        {
            return getAppSettingValue("dev_info_file");
        }
        public static string GetDataPath()
        {
            return getAppSettingValue("data_path");
        }
    }
    
}
