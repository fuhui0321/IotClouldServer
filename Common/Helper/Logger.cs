/** 
*  @brief  Product
*  @author Fu Hui
*  @version 1.0
*  @date   2017-07-01
*/

using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Helper
{
    public class LoggerManager
    {
        private readonly static ILog _log;

        static LoggerManager()
        {
            Init();
            _log = LogManager.GetLogger(typeof(LoggerManager));
        }

        public static ILog Log
        {
            get
            {
                return _log;
            }
        }

        private static void Init()
        {
            var path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!path.EndsWith("\\"))
            {
                path += "\\";
            }
            path += "log4net.config";

            var logCfg = new FileInfo(path);
            XmlConfigurator.ConfigureAndWatch(logCfg);
        }

    }
}
