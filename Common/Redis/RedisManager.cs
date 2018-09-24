/** 
*  @brief  RedisManager
*  @author Fu Hui
*  @version 1.0
*  @date   2017-07-01
*/

#define debug


using CSRedis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IotCloudService.Common;
using IotCloudService.Common.Modes;
using System.Net;
using IotCloudService.Common.Helper;

namespace IotCloudService.Common.Redis
{
    public class RedisManager
    {
        /// <summary>
        /// redis配置文件信息
        /// </summary>
        private static RedisConfiguration _redisConfig /*= RedisConfiguration.GetConfig()*/;
        private static string _pwd;        
        private static RedisConnectionPool _prcm;
        //private static string _IP;
        private static int _PoolMaxSize;

        private static string _host;
        private static int _port;

        /// <summary>
        /// 静态构造方法，初始化链接池管理对象
        /// </summary>
        static RedisManager()
        {
            CreateManager();
        }

        /// <summary>
        /// 创建链接池管理对象
        /// IP地址中可以加入auth验证   password@ip:port
        /// </summary>
        private static void CreateManager()
        {

            {
                //IPEndPoint result = null;
                var host = ConfigHelper.RedisEndPoint.Trim();
                if (host.IndexOf("@") > -1)
                {
                    var hostParts = host.Split('@');
                    _pwd = hostParts[0];
                    var ip = hostParts[1].Split(':');

                    _host = ip[0];
                    _port = int.Parse(ip[1]);

                    //result = new IPEndPoint(IPAddress.Parse(ip[0]), int.Parse(ip[1]));
                }
                else
                {
                    var hostParts = host.Split(':');
                    _host = hostParts[0];
                    _port = int.Parse(hostParts[1]);
                    //result = new IPEndPoint(IPAddress.Parse(hostParts[0]), int.Parse(hostParts[1]));
                }
                //_IP = result;
                _PoolMaxSize = ConfigHelper.PoolMaxSize;



            }

            //_prcm = new RedisConnectionPool(_IP, _PoolMaxSize);
            _prcm = new RedisConnectionPool(_host,_port, _PoolMaxSize);
        }

        /// <summary>
        /// 客户端缓存操作对象
        /// </summary>
        public static RedisClient GetClient()
        {
            var p = "";
           // var endPoint = _redisConfig.AnsyzeHost(ref p);
            var client = new RedisClient(_host, _port);
            if (!string.IsNullOrWhiteSpace(_pwd))
            {
                client.Auth(_pwd);
            //client.Select(1);
            }
            return client;
        }

        private static RedisClient GetRpcClient()
        {
            if (_prcm == null)
                CreateManager();
            var client = _prcm.GetClient();
            if (client != null && !string.IsNullOrWhiteSpace(_pwd))
            {
                client.Auth(_pwd);
            }
            return client;
        }
        public static RedisPubSubServer GetPubServer(Action<string, string> OnMessage, params string[] channel)
        {
            var client = GetRpcClient();
            if (client == null)
            {
                LoggerMng.Log.Error("连接池已经消耗至尽");
                throw new Exception("连接池已经消耗至尽");
            }

            return new Common.Redis.RedisPubSubServer(client, channel, OnMessage);
        }
    }
}
