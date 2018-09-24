/** 
*  @brief  Product
*  @author Fu Hui
*  @version 1.0
*  @date   2017-07-01
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.SelfHost;
using log4net;
using System.IO;
using log4net.Config;
using IotCloudService.Common.Helper;
using IotCloudService.Common.Redis;
using CSRedis;
using uPLibrary.Networking.M2Mqtt;
using IotCloudService.MqttClientHelper;
using IotCloudService.IotMessagePushLibrary.JPush;

namespace IotCloudService.SelfHost
{
    class Program
    {
        static void Main(string[] args)
        {


            //跨域配置
            // config.EnableCors(new EnableCorsAttribute("*", "*", "*"));




            UserManagerHelper.InitUserManager();
            CompanyManagerHelper.Initialize();
            CompanyManagerHelper.StartDeviceStatusThread();


            


            LoggerManager.Log.Info("服务启动开始...\n");
            try
            {
                Assembly.Load("IotCloudService.WebApi, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
                HttpSelfHostConfiguration configuration = new HttpSelfHostConfiguration("http://47.100.169.224:8080");
                using (HttpSelfHostServer httpServer = new HttpSelfHostServer(configuration))
                {
                    httpServer.Configuration.EnableCors();

                    httpServer.Configuration.Routes.MapHttpRoute(
                        name: "DefaultApi",
                        routeTemplate: "api/{controller}/{action}/{id}",
                        defaults: new { id = RouteParameter.Optional });

                    httpServer.OpenAsync().Wait();
                    Console.WriteLine("Press Enter to quit.");
                    Console.ReadLine();
                }

            }
            catch (Exception ex)
            {
                LoggerManager.Log.Info(ex.Message);
               
                LoggerManager.Log.Info("出现异常，服务退出！\n");
                Console.ReadLine();
            }
        }
    }
}
