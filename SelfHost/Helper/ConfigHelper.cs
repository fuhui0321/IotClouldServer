/** 
*  @brief  Product
*  @author Fu Hui
*  @version 1.0
*  @date   2017-07-01
*/
#define debug

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.SelfHost.Helper
{
    public class ConfigHelper
    {
        public static string Upload_srv
        {
            get
            {
                return ConfigurationManager.AppSettings["Upload_srv"];
            }

        }
        public static string JustIn_srv
        {
            get
            {
                return ConfigurationManager.AppSettings["JustIn_srv"];
            }

        }
        public static string PubSub_srv
        {
            get
            {
                return ConfigurationManager.AppSettings["PubSub_srv"];
            }

        }
        public static string SessionId
        {
            get
            {
                return ConfigurationManager.AppSettings["SessionId"];
            }

        }
        public static TimeSpan Frequency
        {
            get
            {
                return TimeSpan.Parse(ConfigurationManager.AppSettings["Frequency"]);
            }

        }
        public static double Diff_Float
        {
            get
            {
                return double.Parse(ConfigurationManager.AppSettings["Diff_Float"]);
            }

        }


        public static string StationCode
        {
            get
            {
                return ConfigurationManager.AppSettings["StationCode"];

            }

        }
        public static string CompanyCode
        {
            get
            {
                return ConfigurationManager.AppSettings["CompanyCode"];

            }

        }
        public static bool IsDebug
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings["IsDebug"]);

            }

        }
        public static int Limit
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["Limit"]);

            }

        }
        public static string Emu_db_ConnStr
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["emu_db"].ConnectionString;
            }
        }

        #region Redis
        public static TimeSpan Redis_Frequency
        {
            get
            {
                return TimeSpan.Parse(ConfigurationManager.AppSettings["Redis_Frequency"]);

            }

        }

        public static string RedisEndPoint
        {
            get
            {
                return ConfigurationManager.AppSettings["RedisEndPoint"];

            }

        }

        public static string RedisDbName
        {
            get
            {
                return ConfigurationManager.AppSettings["RedisDbName"];

            }

        }


        public static string Alarm_RedisEndPoint
        {
            get
            {
                return (ConfigurationManager.AppSettings["Alarm_RedisEndPoint"] ?? "").Trim();

            }

        }

        public static string Alarm_RedisDbName
        {
            get
            {
                return (ConfigurationManager.AppSettings["Alarm_RedisDbName"] ?? "").Trim();
            }

        }
        public static string Strategy_RedisEndPoint
        {
            get
            {
                return (ConfigurationManager.AppSettings["Strategy_RedisEndPoint"] ?? "").Trim();

            }

        }

        public static string Strategy_RedisDbName
        {
            get
            {
                return (ConfigurationManager.AppSettings["Strategy_RedisDbName"] ?? "").Trim();
            }

        }

        public static string[] ClearTable
        {
            get
            {
                return (ConfigurationManager.AppSettings["ClearTable"] ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

        }

        #endregion

    }
}
