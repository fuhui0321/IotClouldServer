/** 
*  @brief  RedisSubscribe_Imps
*  @author Fu Hui
*  @version 1.0
*  @date   2017-07-01
*/

#define debug


//using Soaring.EMNMS.Contract.Args;
//using Soaring.EMNMS.Contract.Common;
//using Soaring.EMNMS.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
//using System.ServiceModel;
//using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Redis
{
    public class RedisSubscribe_Imps : IDisposable
    {
        //private IJustInDataCallback _client;
        private RedisPubSubServer _subServer;
        private bool _disposed = false;
        private string _companyCode;
        private string _stationCode;
        private string _channelKey;
        private string _clientKey;
        public EventHandler OnErrored;

        public string ClientKey
        {
            get
            {
                return _clientKey;
            }

            private set
            {
                _clientKey = value;
            }
        }

        //public RedisSubscribe_Imps(IJustInDataCallback client, string clientKey, string companyCode, string stationCode)
        //{
        //    this._client = client;
        //    this.ClientKey = clientKey;
        //    this._companyCode = companyCode;
        //    this._stationCode = stationCode;
        //    this._channelKey = $"{companyCode}_{stationCode}_{Helper.ChannelRandString}";
        //    HandleChannel();
        //    AttachMessage();
        //}
        //private void HandleChannel()
        //{
        //    var chan = this._client as ICommunicationObject;
        //    if (chan == null) return;
        //    chan.Closed += (s, e) => this.OnErrored?.Invoke(this, EventArgs.Empty);
        //    chan.Faulted += (s, e) => this.OnErrored?.Invoke(this, EventArgs.Empty);

        //}
        private  void AttachMessage()
        {
            //_subServer = RedisManager.GetPubServer(OnMessage, this._channelKey);
            //_subServer.Start();
        }

        //private void OnMessage(string channel, string msg)
        //{
        //    try
        //    {
        //        LoggerMng.Log.Info($"Send:Channel={channel},Message={msg}");
        //        var arg = Helper.DeSerialize<IssuedInstructionArgs>(msg);
        //        _client.IssuedInstruction(arg);

        //    }
        //    catch (Exception exp)
        //    { 
        //        LoggerMng.Log.Error(MethodInfo.GetCurrentMethod().Name, exp);
        //        OnErrored?.Invoke(this, EventArgs.Empty);
        //    }
        //}
        public void Dispose()
        {
            //Dispose(true);
            GC.SuppressFinalize(this);
            GC.Collect();

        }

        //private void Dispose(bool disposing)
        //{

        //    if (!this._disposed)
        //    {
        //        if (disposing)
        //        {

        //            _subServer?.Stop();
        //            _subServer?.Dispose();

        //        }
        //        _subServer = null;
        //        _client = null;
        //        _disposed = true;

        //    }
        //}
    }
}
