/** 
*  @brief  RedisConfiguration
*  @author Fu Hui
*  @version 1.0
*  @date   2017-07-01
*/

#define debug

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Redis
{
    public sealed class RedisConfiguration : ConfigurationSection
    {
        public static RedisConfiguration GetConfig()
        {
            RedisConfiguration section = GetConfig("RedisConfig");
            return section;
        }


        public IPEndPoint AnsyzeHost(ref string pwd)
        {
            IPEndPoint result = null;
            var host = this.ServerHost.Trim();
            if (host.IndexOf("@") > -1)
            {
                var hostParts = host.Split('@');
                pwd = hostParts[0];
                var ip = hostParts[1].Split(':');
                result = new IPEndPoint(IPAddress.Parse(ip[0]), int.Parse(ip[1]));
            }
            else
            {
                var hostParts = host.Split(':');
                result = new IPEndPoint(IPAddress.Parse(hostParts[0]), int.Parse(hostParts[1]));
            }
            return result;
        }

        public static RedisConfiguration GetConfig(string sectionName)
        {
            RedisConfiguration section = (RedisConfiguration)ConfigurationManager.GetSection(sectionName);
            if (section == null)
                throw new ConfigurationErrorsException("Section " + sectionName + " is not found.");
            return section;
        }
        /// <summary>
        /// 可写的Redis链接地址
        /// </summary>
        [ConfigurationProperty(nameof(ServerHost), DefaultValue = "127.0.0.1:6379", IsRequired = false)]
        public string ServerHost
        {
            get
            {
                return (string)base["ServerHost"];
            }
            set
            {
                base["ServerHost"] = value;
            }
        }


         

        /// <summary>
        /// 最大读链接数
        /// </summary>
        [ConfigurationProperty(nameof(MaxPoolSize), IsRequired = false, DefaultValue = 100)]
        public int MaxPoolSize
        {
            get
            {
                int _maxPoolSize = (int)base["MaxPoolSize"];
                return _maxPoolSize > 0 ? _maxPoolSize :100;
            }
            set
            {
                base["MaxPoolSize"] = value;
            }
        }


        /// <summary>
        /// 自动重启
        /// </summary>
        [ConfigurationProperty("AutoStart", IsRequired = false, DefaultValue = true)]
        public bool AutoStart
        {
            get
            {
                return (bool)base["AutoStart"];
            }
            set
            {
                base["AutoStart"] = value;
            }
        }



        /// <summary>
        /// 本地缓存到期时间，单位:秒
        /// </summary>
        [ConfigurationProperty("LocalCacheTime", IsRequired = false, DefaultValue = 36000)]
        public int LocalCacheTime
        {
            get
            {
                return (int)base["LocalCacheTime"];
            }
            set
            {
                base["LocalCacheTime"] = value;
            }
        }


        /// <summary>
        /// 是否记录日志,该设置仅用于排查redis运行时出现的问题,如redis工作正常,请关闭该项
        /// </summary>
        [ConfigurationProperty("RecordeLog", IsRequired = false, DefaultValue = false)]
        public bool RecordeLog
        {
            get
            {
                return (bool)base["RecordeLog"];
            }
            set
            {
                base["RecordeLog"] = value;
            }
        }

    }
}
