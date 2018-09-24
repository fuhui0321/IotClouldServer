using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Net.Http;
using System.Web.Cors;
using System.Web.Http.Cors;
using IotCloudService.MqttClientHelper;
using IotCloudService.Common.Helper;
using IotCloudService.Common.Modes;
using IotCloudService.Common.Redis;
using Newtonsoft.Json;

namespace IotCloudService.WebApi
{




    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class IotClientController : ApiController           
    {

        [HttpGet]
        public bool Login(string CompanyCode,string UserName,string Password)
        {
            bool res = true;

            if (UserName != "Test1234")
            {
                res = false;
            }


            return res;
        }

        [HttpGet]
        public bool WriteTagValue(string CompanyCode,string DeviceCode,string TagName,string TagValue)
        {
            bool bSucceed = false;

            LoggerManager.Log.Info("Write tag value！\n");

            string mqttTopic = CompanyCode + "/" + DeviceCode + "/00";
            TagItem[] tagDatas = new TagItem[1];
            tagDatas[0] = new TagItem();
            tagDatas[0].TagName = TagName;
            tagDatas[0].TagValue = TagValue;

            string mqttMessage = JsonConvert.SerializeObject(tagDatas);
            MqttManager.client_MqttMsgPublist(mqttTopic, mqttMessage);//"Company1/Device001/00"

            return bSucceed;
        }


        [HttpPost]
        public string[] GetTagValues([FromBody]string[] TagNames)
        {
            string[] TagValues = new string[5];

            TagValues[0] = "1";
            TagValues[1] = "1";
            TagValues[2] = "1";
            TagValues[3] = "1";
            TagValues[4] = "1";


            return TagValues;
        }

        [HttpGet]
        public string[] ReadTagValues(string CompanyCode,string DeviceCode,string TagList)
        {
            string[] TagValues = new string[0];


            LoggerManager.Log.Info("Read tag value！\n");

            if (TagList == null)
            {
                return TagValues;
            }


            string[] strTagNameArray = TagList.Split(','); //字符串转数组
                 


            try
            {
                var client = RedisManager.GetClient();
                string RedisHashName = $"[{CompanyCode}]-[{DeviceCode}]"; ;
                // Dictionary<string, string> TagDictionary = UploadDatas.Datas.ToDictionary(x => x.TagName, y => y.TagValue);
                TagValues = client.HMGet(RedisHashName, strTagNameArray);

            }
            catch (Exception ex)
            {
               
                //return res;
            }



            return TagValues;
        }

    }
}
