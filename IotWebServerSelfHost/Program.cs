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

using IotCloudService.IotWebServerWebApi.Filters;
using IotCloudService.IotWebServerWebApi.Helper;
using IotCloudService.IotMessagePushLibrary.JPush;

//using IotCloudService.WebApi.Filters;

namespace IotCloudService.IotWebServerWebApi
{
    class Program
    {
        static void Main(string[] args)
        {


            MySqlConnectPoolHelper.getPool().InitMySqlConnectPool();
            UserManagerHelper.InitUserManager();
            CompanyManagerHelper.Initialize();
            CompanyManagerHelper.StartAlarmStoreService();
            CompanyManagerHelper.StartDataStoreService();
            FileServerHelper.Initialize();

            //CompanyManagerHelper.StartDeviceStatusThread();

            //跨域配置
            // config.EnableCors(new EnableCorsAttribute("*", "*", "*"));


            //var _client = RedisManager.GetClient();
            //routeTemplate: "api/{controller}/{id}",

           

            LoggerManager.Log.Info("服务启动开始...\n");
            try
            {
                
                Assembly.Load("IotCloudService.IotWebServerWebApi, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
                HttpSelfHostConfiguration configuration = new HttpSelfHostConfiguration("http://127.0.0.1:8081");
                using (HttpSelfHostServer httpServer = new HttpSelfHostServer(configuration))
                {
                    httpServer.Configuration.EnableCors();

                    httpServer.Configuration.MapHttpAttributeRoutes();
                    httpServer.Configuration.Filters.Add(new ApiSecurityFilter());

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
