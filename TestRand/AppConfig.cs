using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;

namespace TestRand
{
    public class net_planning
    {
        private static double solveQuadratic(double a, double b , double c)
        {
            double u = (b * b) - 4 * a * c;
            double us = Math.Sqrt(u);
            double x_0 = (-b + us) / (2 * a);
            double x_1 = (-b - us) / (2 * a);
            return Math.Abs(x_0 >= x_1 ? x_0 : x_1);
        }
        public static int getOptimumNumberofLayers(int mc=0, int bs=0)
        {
            int layers = 0;
            mc = AppConfig.getNumberofChargers();
            double N = (AppConfig.getNumberOfNodes()) / 3;
            // psi*(ps+1)= N
            //psi^2+ps-N=0;
            double s = solveQuadratic(1, 1, -N);
            layers = (int) Math.Ceiling(s);

            return layers;
        }
    }
    public enum eng_threshold_method
    {
        assign_threshold_algorithm =0,
        guassian =1,
        constant=2,
        random=3,
    }
    public class AppConfig
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
        public static string getConfigValue(string key, object def_value)
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
                value = string.Format("{0}", def_value);
            }
            return value;
        }
        public static int getSimTimeInSeconds()
        {
            string value = getConfigValue("sim_time");
            if (string.IsNullOrEmpty(value)) return 1 * 60;//
            return int.Parse(value) * 60;
        }
        public static int getSimTimeInMiliSeconds()
        {
            return getSimTimeInSeconds() * 1000;
        }
        public static int get_sample_fx()
        {
            string sample_fx= getConfigValue("samples_fx");
            return int.Parse(sample_fx);
        }
        public static double get_net_size_width()
        {
            string net = getConfigValue("net_size_width", 1000);
            return double.Parse(net);
        }
        public static double get_net_size_len()
        {
            string value = getConfigValue("net_size_length", 1000);
            return double.Parse(value);
        }
        public static int getNumberOfNodes()
        {
            string value = getConfigValue("n_nodes", 100);
            return int.Parse(value);
        }
        public static double getComRange()
        {
            string value = getConfigValue("com_range", 30);
            return double.Parse(value);
        }
        public static double getAlpha()
        {
            string value = getConfigValue("alpha", 0.3);
            double v = double.Parse(value);
           // if (v > 0.5) return 0.3;
            return v;
        }
        public static double get_sensor_node_battery_cap()
        {
            string value = getConfigValue("sensor_node_battery_cap", 100);
            return double.Parse(value);
        }
        public static double get_mobile_charger_battery_cap()
        {
            string value = getConfigValue("mobile_charger_battery_cap", 8500);
            return double.Parse(value);
        }
        public static bool getSleepModeStatus()
        {
            string value = getConfigValue("enable_sleep_mode", 1);
            return int.Parse(value) == 1;
        }
        public static double getSleepModeEng()
        {
            string value = getConfigValue("sleep_mode_eng", 0.003);
            return double.Parse(value);
        }
        public static int getNumberofChargers()
        {
            string value = getConfigValue("mc_count", 2);
            return int.Parse(value);
        }
        public static double getEngConsumption()
        {
            string value = getConfigValue("eng_consumption", 0.02);
            return double.Parse(value);
        }
        public static double getEnTxRx()
        {
            string value = getConfigValue("tx_rx_packet_eng", 0.02);
            return double.Parse(value);
        }
        public static double getChargeRate()
        {
            string value = getConfigValue("charge_rate", 1.5);
            return double.Parse(value);
        }
        public static double get_eng_threshold_constant()
        {
            string value = getConfigValue("eng_threshold_constant", 20);
            return double.Parse(value);
        }
        public static eng_threshold_method getThresholdMethod()
        {
            string value = getConfigValue("eng_threshold_method", 0);
            int i = int.Parse(value);
            switch(i)
            {
                case 0:
                    return eng_threshold_method.assign_threshold_algorithm;
                case 1:
                    return eng_threshold_method.guassian;
                case 2:
                    return eng_threshold_method.constant;
                case 3:
                    return eng_threshold_method.random;

            }
            return eng_threshold_method.assign_threshold_algorithm;
        }
        public static int net_timer_time_range_start()
        {
            string value = getConfigValue("net_timer_time_range_start", 10);
            return int.Parse(value);
        }
        public static int net_timer_time_range_end()
        {
            string value = getConfigValue("net_timer_time_range_end", 20);
            return int.Parse(value);
        }
        public static double getMaxLinkLen()
        {
            //max_link_len
            string value = getConfigValue("max_link_len", 64);
            return double.Parse(value);
        }
        public static double getRemainingEnergy()
        {
            string value = getConfigValue("remaining_engery", 100);
            return double.Parse(value);
        }
        public static double getRemainingEnergyPercent()
        {
            string value = getConfigValue("remaining_engery", 100);
            return double.Parse(value) / 100;
        }
        public static double get_mcv_speed()
        {
            string value = getConfigValue("mcv_speed", 5);
            return double.Parse(value);
        }
        /// <summary>
        /// percent of maximum energy
        /// </summary>
        /// <returns></returns>
        public static double get_mcv_eng_per_meter()
        {
            string value = getConfigValue("mcv_eng_per_meter", 1);
            return double.Parse(value);
        }
        public static bool use_auto_connect()
        {
            string value = getConfigValue("use_auto_connect", 1);
            return int.Parse(value)==1;
        }
        /// <summary>
        /// beta_0 in percent 
        /// </summary>
        /// <returns></returns>
        public static double getBeta_0()
        {
            string value = getConfigValue("beta_0", 5);
            return double.Parse(value) / 100.0;
        }
        /// <summary>
        /// beta_1 in percent
        /// </summary>
        /// <returns></returns>
        public static double getBeta_1()
        {
            string value = getConfigValue("beta_1", 35);
            return double.Parse(value) / 100.0;
        }
    }
}
