/** 
*  @brief  RedisBase
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

namespace IotCloudService.Common.Redis
{
    public abstract class RedisBase : IDisposable
    {
        protected virtual  RedisClient Client { get; private set; }
        private bool _disposed = false;
        protected RedisBase()
        {
            Client = RedisManager.GetClient();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    Client.Dispose();
                    Client = null;
                }
            }
            this._disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 保存数据DB文件到硬盘
        /// </summary>
        public void Save()
        {
            Client.Save();
        }
        /// <summary>
        /// 异步保存数据DB文件到硬盘
        /// </summary>
        public void SaveAsync()
        {
            Client.SaveAsync();
        }

    }
}
