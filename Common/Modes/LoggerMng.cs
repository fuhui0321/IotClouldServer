/** 
*  @brief  LoggerMng
*  @author Fu Hui
*  @version 1.0
*  @date   2017-07-01
*/

#define debug

using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Modes
{
    public class LoggerMng
    {
        private readonly static ILog _log;

        static LoggerMng()
        {
            //object section = ConfigurationManager.GetSection("log4net");
            //log4net.Config.XmlConfigurator.Configure(section as System.Xml.XmlElement);

            XmlConfigurator.Configure();
            _log = LogManager.GetLogger(typeof(LoggerMng));
        }

        public static ILog Log
        {
            get
            {
                return _log;
            }
        }


    }
}
