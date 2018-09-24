using IotCloudService.Common;
using IotCloudService.Common.Helper;
using IotCloudService.Common.Modes;
using IotCloudService.Common.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Web.Http.Cors;

namespace IotCloudService.WebApi
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class IotUploadAlarmController : ApiController
    {
        [HttpPost]
        public ResultBase UploadAlarmDatas([FromBody]AlarmDatas UploadAlarmDatas)
        {
            ResultAlarm res = new ResultAlarm();

            LoggerManager.Log.Info("Upload alarm datas！\n");

            if (UploadAlarmDatas == null)
            {
                res.IsSuccess = false;
                return res;
            }


            if (CompanyManagerHelper.CheckDeviceCode(UploadAlarmDatas.DeviceInfo) == false)
            {
                res.IsSuccess = false;
                return res;
            }



            try
            {

                string AlarmJsonString = JsonConvert.SerializeObject(UploadAlarmDatas);
                //JsonSerializer serializer = new JsonSerializer();
                //StringReader sr = new StringReader(AlarmJsonString);
                //object o = serializer.Deserialize(new JsonTextReader(sr), typeof(AlarmItemBase));

                //var ja = JArray.Parse(AlarmJsonString);



                var client = RedisManager.GetClient();
                

                client.Select(1);
                string RedisHashName = $"[AlarmList]-[{UploadAlarmDatas.DeviceInfo.CompanyCode}]";
                client.RPush(RedisHashName, AlarmJsonString);


                //client.Select(0);
                ////更新实时故障的状态
                //string DataRedisHashName = $"[{UploadAlarmDatas.DeviceInfo.CompanyCode}]-[{UploadAlarmDatas.DeviceInfo.DeviceCode}]"; ;
                ////Dictionary<string, string> TagDictionary = UploadDatas.Datas.ToDictionary(x => x.TagName, y => y.TagValue);
                //var alarmUUID = Guid.NewGuid().ToString();
                //client.HSet(DataRedisHashName, $"AlarmUpdateCode", alarmUUID);
            }
            catch (Exception ex)
            {
                LoggerManager.Log.Info("更新Resis数据库出错！\n");
                res.IsSuccess = false;
                return res;
            }


            try
            {


                AlarmState[] temp_uuid_list = new AlarmState[UploadAlarmDatas.AlarmList.Length];

                             
                

                for (int i = 0; i < UploadAlarmDatas.AlarmList.Length; i++)
                {
                    temp_uuid_list[i] = new AlarmState();

                    temp_uuid_list[i].uuid_code = UploadAlarmDatas.AlarmList[i].AlarmUUID;
                    temp_uuid_list[i].AlarmTime = UploadAlarmDatas.AlarmList[i].AlarmDate;


                }
                res.AlarmUuidList = temp_uuid_list;

            }
            catch
            {

            }



                return res;

        }
    }
}
