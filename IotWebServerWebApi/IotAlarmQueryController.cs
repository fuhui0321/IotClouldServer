using IotCloudService.Common.AlarmStore;
using IotCloudService.Common.Helper;
using IotCloudService.Common.Modes;
using IotCloudService.Common.Redis;
using IotCloudService.IotWebServerWebApi.Common;
using IotCloudService.IotWebServerWebApi.Models;
using IotCloudService.ShardingDataQueryLibrary.Mode;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace IotCloudService.IotWebServerWebApi
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class IotAlarmQueryController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage QueryHistoryAlarm([FromBody] AlarmQueryParam QueryParams)
        {
            QueryResultBase queryResult = new QueryResultBase();

            List<AlarmInfo> queryAlarm = CompanyManagerHelper.QueryHistoryAlarm(QueryParams);

            queryResult = new QueryResultBase();
            queryResult.ResultCode = QueryResultCodeEnum.QUERY_SUCCESS;
            queryResult.QueryData = queryAlarm;


            //git test            




            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(queryResult));
        }

        [HttpPost]
        public HttpResponseMessage QueryRealtimeAlarm([FromBody] AlarmQueryParam QueryParams)
        {

            QueryResultBase queryResult = new QueryResultBase();

            List<AlarmInfo> queryAlarm = CompanyManagerHelper.QueryRealtimeAlarm(QueryParams);

            queryResult = new QueryResultBase();
            queryResult.ResultCode = QueryResultCodeEnum.QUERY_SUCCESS;
            queryResult.QueryData = queryAlarm;
            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(queryResult));
        }

        [HttpPost]
        public HttpResponseMessage GetDeviceAlarmStat([FromBody] AlarmStatQueryParam QueryParams)
        {

            ResultMsg resultMsg = null;
            bool res = true;

            //string[] tempAlarmStatList = ;

            List<AlarmStatQueryResponse> AlarmStatDataList = new List<AlarmStatQueryResponse>();


            LoggerManager.Log.Info("Get device alarm stat data！\n");

            try
            {
                var client = RedisManager.GetClient();
                foreach (string deviceID in QueryParams.DeviceCode)
                {
                    string RedisHashName = $"[{QueryParams.CompanyCode}]-[{deviceID}]";

                    AlarmStatQueryResponse tempAlarmStatData = new AlarmStatQueryResponse();

                    tempAlarmStatData.AlarmStatValue = client.HMGet(RedisHashName, QueryParams.AlarmStatName);

                    AlarmStatDataList.Add(tempAlarmStatData);
                }
                
                
                //string keyAlarmUpdate = $"[{CompanyCode}]-AlarmUpdateCode";
                // Dictionary<string, string> TagDictionary = UploadDatas.Datas.ToDictionary(x => x.TagName, y => y.TagValue);
                

            }
            catch (Exception ex)
            {

                //return res;
            }

            resultMsg = new ResultMsg();
            resultMsg.StatusCode = (int)StatusCodeEnum.Success;
            resultMsg.Info = "";
            resultMsg.Data = AlarmStatDataList;

            // LoggerManager.Log.Info(JsonConvert.SerializeObject(resultMsg) + "\n");

            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(resultMsg));
        }



        [HttpPost]
        public HttpResponseMessage QueryRealtimeAlarmEx([FromBody]RealtimeAlarmQueryParam RealtimeAlarmQueryParams)
        {

            QueryResultBase queryResult = new QueryResultBase();

            List<AlarmInfoEx> queryAlarm = CompanyManagerHelper.QueryRealtimeAlarmEx(RealtimeAlarmQueryParams);

            //List<AlarmInfo> testAlarm = new List<AlarmInfo>();

            //if (queryAlarm.Count() >0)
            //{
            //    testAlarm.Add(queryAlarm[0].AlarmInfoObject);

            //    queryResult = new QueryResultBase();
            //    queryResult.ResultCode = QueryResultCodeEnum.QUERY_SUCCESS;
            //    queryResult.QueryData = testAlarm;
            //    return HttpResponseExtension.toJson(JsonConvert.SerializeObject(queryResult));
            //}

            

            queryResult = new QueryResultBase();
            queryResult.ResultCode = QueryResultCodeEnum.QUERY_SUCCESS;
            queryResult.QueryData = queryAlarm;
            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(queryResult));
        }
    }
}
