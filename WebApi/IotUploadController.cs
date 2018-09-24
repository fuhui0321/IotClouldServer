using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Net.Http;

using IotCloudService.Common;
using IotCloudService.Common.Redis;
using IotCloudService.Common.Helper;
using IotCloudService.Common.Modes;

namespace IotCloudService.WebApi
{


    public class IotUploadController : ApiController
    {
        [HttpPost]
        public ResultBase UploadReadtimeDatas([FromBody]RealtimeDatas UploadDatas)
        {
            ResultBase res = new ResultBase();

            


            long currentTicks = DateTime.Now.Ticks;

            if (UploadDatas == null)
            {
                LoggerManager.Log.Error("Upload realtime datas error: <UploadDatas  == NULL>！\n");
                res.IsSuccess = false;
                return res;
            }

           


            if (CompanyManagerHelper.CheckDeviceCode(UploadDatas.DeviceInfo) == false)
            {
                LoggerManager.Log.Error("Upload realtime datas error: <check DeviceInfo is false>！\n");
                res.IsSuccess = false;
                return res;
            }

            try
            {
                var client = RedisManager.GetClient();
                string RedisHashName = $"[{UploadDatas.DeviceInfo.CompanyCode}]-[{UploadDatas.DeviceInfo.DeviceCode}]"; ;
                Dictionary<string, string> TagDictionary = UploadDatas.Datas.ToDictionary(x => x.TagName, y => y.TagValue);
                client.HMSet(RedisHashName, TagDictionary);

            }
            catch(Exception ex)
            {
                LoggerManager.Log.Info("更新Resis数据库出错！\n");
                res.IsSuccess = false;
                return res;
            }


            LoggerManager.Log.Info($"Upload realtime datas ！{(DateTime.Now.Ticks-currentTicks)/10000}\n");

            return res;
        }


        [HttpPost]
        public ResultBase UploadDeviceStatus([FromBody]RealtimeDatas UploadDatas)
        {
            ResultBase res = new ResultBase();

            LoggerManager.Log.Info("Upload realtime datas ！\n");            



            if (CompanyManagerHelper.CheckDeviceCode(UploadDatas.DeviceInfo) == false)
            {
                LoggerManager.Log.Error("Upload realtime datas error: <check DeviceInfo is false>！\n");
                res.IsSuccess = false;
                return res;
            }

            try
            {
                var client = RedisManager.GetClient();
                string RedisHashName = $"[{UploadDatas.DeviceInfo.CompanyCode}]-[{UploadDatas.DeviceInfo.DeviceCode}]"; ;
                Dictionary<string, string> TagDictionary = UploadDatas.Datas.ToDictionary(x => x.TagName, y => y.TagValue);
                client.HMSet(RedisHashName, TagDictionary);

            }
            catch (Exception ex)
            {
                LoggerManager.Log.Info("更新Resis数据库出错！\n");
                res.IsSuccess = false;
                return res;
            }

            return res;
        }

        [HttpPost]
        public ResultBase UploadDataRecordStatus([FromBody]DeviceDataRecordStatus UploadDataRecord)
        {
            ResultBase res = new ResultBase();

            LoggerManager.Log.Info("Upload realtime datas ！\n");



            if (CompanyManagerHelper.CheckDeviceCode(UploadDataRecord.DeviceInfo) == false)
            {
                LoggerManager.Log.Error("Upload realtime datas error: <check DeviceInfo is false>！\n");
                res.IsSuccess = false;
                return res;
            }

            try
            {
                //var client = RedisManager.GetClient();
                //string RedisHashName = $"[{UploadDatas.DeviceInfo.CompanyCode}]-[{UploadDatas.DeviceInfo.DeviceCode}]"; ;
                //Dictionary<string, string> TagDictionary = UploadDatas.Datas.ToDictionary(x => x.TagName, y => y.TagValue);
                //client.HMSet(RedisHashName, TagDictionary);

            }
            catch (Exception ex)
            {
                LoggerManager.Log.Info("更新Resis数据库出错！\n");
                res.IsSuccess = false;
                return res;
            }

            return res;
        }

        [HttpPost]
        public ResultBase UploadVPNStatus([FromBody]DeviceVPNStatus UploadVPNStatus)
        {
            ResultBase res = new ResultBase();

            LoggerManager.Log.Info("Upload realtime datas ！\n");



            if (CompanyManagerHelper.CheckDeviceCode(UploadVPNStatus.DeviceInfo) == false)
            {
                LoggerManager.Log.Error("Upload realtime datas error: <check DeviceInfo is false>！\n");
                res.IsSuccess = false;
                return res;
            }

            try
            {
                //var client = RedisManager.GetClient();
                //string RedisHashName = $"[{UploadVPNStatus.DeviceInfo.CompanyCode}]-[{UploadDatas.DeviceInfo.DeviceCode}]"; ;
                //Dictionary<string, string> TagDictionary = UploadDatas.Datas.ToDictionary(x => x.TagName, y => y.TagValue);
                //client.HMSet(RedisHashName, TagDictionary);

            }
            catch (Exception ex)
            {
                LoggerManager.Log.Info("更新Resis数据库出错！\n");
                res.IsSuccess = false;
                return res;
            }

            return res;
        }







    }
}
