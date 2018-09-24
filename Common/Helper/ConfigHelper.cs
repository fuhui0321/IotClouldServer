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

namespace IotCloudService.Common.Helper
{
    public class ConfigHelper
    {


        #region Redis


        public static string RedisEndPoint
        {
            get
            {
                return ConfigurationManager.AppSettings["RedisEndPoint"];

            }

        }

        public static int PoolMaxSize
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["PoolMaxSize"]);

            }

        }

        public static string MqttEndPoint
        {
            get
            {
                return ConfigurationManager.AppSettings["MqttEndPoint"];

            }

        }



        #endregion

    }
}
