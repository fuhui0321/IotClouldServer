using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Net.Http;
using System.Web.Cors;
using System.Web.Http.Cors;

using IotCloudService.Common.Helper;
using IotCloudService.Common.Modes;
using IotCloudService.Common.Redis;
using Newtonsoft.Json;
using IotCloudService.IotWebServerWebApi.Models;
using IotCloudService.IotWebServerWebApi.Common;
using IotCloudService.MqttClientHelper;
using System.Threading.Tasks;
using static IotCloudService.IotWebServerWebApi.IotUploadController;
using IotCloudService.IotWebServerWebApi.Helper;
using IotCloudService.IotMessagePushLibrary.JPush;

namespace IotCloudService.IotWebServerWebApi
{




    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class IotClientController : ApiController
    {

        [HttpGet]
        public bool Login(string CompanyCode, string UserName, string Password)
        {
            bool res = true;

            if (UserName != "Test1234")
            {
                res = false;
            }


            return res;
        }

        [HttpGet]
        public HttpResponseMessage GetDeviceStatus(string CompanyCode, string UserPhone)
        {
            ResultMsg resultMsg = null;
            bool res = true;

            DeviceStatusBase[] tempDeviceStatusList = CompanyManagerHelper.GetDeviceStatusList(CompanyCode, UserPhone);

            resultMsg = new ResultMsg();
            resultMsg.StatusCode = (int)StatusCodeEnum.Success;
            resultMsg.Info = "";
            resultMsg.Data = tempDeviceStatusList;



            LoggerManager.Log.Info(JsonConvert.SerializeObject(resultMsg) + "\n");

            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(resultMsg));




            //return tempDeviceStatusList;
        }

        


        [HttpGet]
        public HttpResponseMessage GetCompanyAlarmUpdateState(string CompanyCode)
        {

            ResultMsg resultMsg = null;
            bool res = true;
            string strCompanyAlarmUpdateUuid = "";

            LoggerManager.Log.Info("Get Company Alarm Update State！\n");

            try
            {
                var client = RedisManager.GetClient();
                string RedisHashName = $"DeviceStatusTable";
                string keyAlarmUpdate = $"[{CompanyCode}]-AlarmUpdateCode";
                // Dictionary<string, string> TagDictionary = UploadDatas.Datas.ToDictionary(x => x.TagName, y => y.TagValue);
                strCompanyAlarmUpdateUuid = client.HGet(RedisHashName, keyAlarmUpdate);

            }
            catch (Exception ex)
            {

                //return res;
            }

            resultMsg = new ResultMsg();
            resultMsg.StatusCode = (int)StatusCodeEnum.Success;
            resultMsg.Info = "";
            resultMsg.Data = strCompanyAlarmUpdateUuid;

           // LoggerManager.Log.Info(JsonConvert.SerializeObject(resultMsg) + "\n");

            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(resultMsg));
        }

        [HttpGet]
        public HttpResponseMessage WriteTagValue(string CompanyCode, string DeviceCode, string TagName, string TagValue)
        {

            ResultMsg resultMsg = null;

            LoggerManager.Log.Info("Write tag value！\n");

            string mqttTopic = CompanyCode + "/" + DeviceCode + "/00";
            TagItem[] tagDatas = new TagItem[1];
            tagDatas[0] = new TagItem();
            tagDatas[0].TagName = TagName;
            tagDatas[0].TagValue = TagValue;

            string mqttMessage = JsonConvert.SerializeObject(tagDatas);
            MqttManager.client_MqttMsgPublist(mqttTopic, mqttMessage);//"Company1/Device001/00"


            resultMsg = new ResultMsg();
            resultMsg.StatusCode = (int)StatusCodeEnum.Success;
            resultMsg.Info = "";
            resultMsg.Data = "";
            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(resultMsg));



        }





        [HttpGet]
        public HttpResponseMessage ReadTagValues(string CompanyCode, string DeviceCode, string TagList)
        {
            string[] TagValues = new string[0];
            ResultMsg resultMsg = null;

            LoggerManager.Log.Info("Read tag value！\n");

            if (TagList == null)
            {
                resultMsg = new ResultMsg();
                resultMsg.StatusCode = (int)StatusCodeEnum.ParameterError;
                resultMsg.Info = StatusCodeEnum.ParameterError.GetEnumText();
                resultMsg.Data = "";
                return HttpResponseExtension.toJson(JsonConvert.SerializeObject(resultMsg));
            }


            string[] strTagNameArray = TagList.Split(','); //字符串转数组



            try
            {
                var client = RedisManager.GetClient();
                string RedisHashName = $"[{CompanyCode}]-[{DeviceCode}]"; 
                // Dictionary<string, string> TagDictionary = UploadDatas.Datas.ToDictionary(x => x.TagName, y => y.TagValue);
                TagValues = client.HMGet(RedisHashName, strTagNameArray);

            }
            catch (Exception ex)
            {

                //return res;
            }


            resultMsg = new ResultMsg();
            resultMsg.StatusCode = (int)StatusCodeEnum.Success;
            resultMsg.Info = "";
            resultMsg.Data = TagValues;
            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(resultMsg));



        }

        public HttpResponseMessage RebootDevice(string CompanyCode, string DeviceCode)
        {           
            ResultMsg resultMsg = null;

            LoggerManager.Log.Info($"Company<{CompanyCode}>-Device<{DeviceCode}> reboot.......！\n");

            string mqttTopic = CompanyCode + "/" + DeviceCode + "/Reboot";                        
            MqttManager.client_MqttMsgPublist(mqttTopic, "");


            resultMsg = new ResultMsg();
            resultMsg.StatusCode = (int)StatusCodeEnum.Success;
            resultMsg.Info = "";
            resultMsg.Data = "";
            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(resultMsg));            

        }

        public HttpResponseMessage OpenDeviceVPN(string CompanyCode, string DeviceCode)
        {
            
            ResultMsg resultMsg = null;

            LoggerManager.Log.Info($"Company<{CompanyCode}>-Device<{DeviceCode}> open VPN.......！\n");

            string mqttTopic = CompanyCode + "/" + DeviceCode + "/OpenVPV";
            MqttManager.client_MqttMsgPublist(mqttTopic, "");


            resultMsg = new ResultMsg();
            resultMsg.StatusCode = (int)StatusCodeEnum.Success;
            resultMsg.Info = "";
            resultMsg.Data = "";
            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(resultMsg));




            

        }

        public HttpResponseMessage CloseDeviceVPN(string CompanyCode, string DeviceCode)
        {
            ResultMsg resultMsg = null;

            LoggerManager.Log.Info($"Company<{CompanyCode}>-Device<{DeviceCode}> close VPN.......！\n");

            string mqttTopic = CompanyCode + "/" + DeviceCode + "/CloseVPN";
            MqttManager.client_MqttMsgPublist(mqttTopic, "");


            resultMsg = new ResultMsg();
            resultMsg.StatusCode = (int)StatusCodeEnum.Success;
            resultMsg.Info = "";
            resultMsg.Data = "";
            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(resultMsg));

        }

        public HttpResponseMessage StartDeviceDataRecord(string CompanyCode, string DeviceCode)
        {
            ResultMsg resultMsg = null;

            LoggerManager.Log.Info($"Company<{CompanyCode}>-Device<{DeviceCode}> start data record.......！\n");

            string mqttTopic = CompanyCode + "/" + DeviceCode + "/StartDataRecord";
            MqttManager.client_MqttMsgPublist(mqttTopic, "");


            resultMsg = new ResultMsg();
            resultMsg.StatusCode = (int)StatusCodeEnum.Success;
            resultMsg.Info = "";
            resultMsg.Data = "";
            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(resultMsg));

        }

        public HttpResponseMessage StopDeviceDataRecord(string CompanyCode, string DeviceCode)
        {
            ResultMsg resultMsg = null;

            LoggerManager.Log.Info($"Company<{CompanyCode}>-Device<{DeviceCode}> stop data record.......！\n");

            string mqttTopic = CompanyCode + "/" + DeviceCode + "/StopDataRecord";
            MqttManager.client_MqttMsgPublist(mqttTopic, "");


            resultMsg = new ResultMsg();
            resultMsg.StatusCode = (int)StatusCodeEnum.Success;
            resultMsg.Info = "";
            resultMsg.Data = "";
            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(resultMsg));

        }
    }
}
