using IotCloudService.Common.Helper;
using IotCloudService.Common.Modes;
using IotCloudService.IotWebServerWebApi.Common;
using IotCloudService.IotWebServerWebApi.Models;
using IotCloudService.ShardingDataQueryLibrary;
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
    public class IotDataQueryController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage QueryHistoryData([FromBody] DataQueryParamObject QueryParams)
        {
            
            //DataQueryParamObject queryParam = null;
            QueryResultBase queryResult = null;
            

            if (QueryParams == null)
            {
                queryResult = new QueryResultBase();
                queryResult.ResultCode = QueryResultCodeEnum.QUERY_ERROR_PARAM;


                return HttpResponseExtension.toJson(JsonConvert.SerializeObject(queryResult));
            }
            IotDataQueryInterface dataQueryInterfaceObject = new IotDataQueryInterface();
            queryResult = dataQueryInterfaceObject.HandleQueryInterface(QueryParams);

            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(queryResult));

                       
        }

        [HttpPost]
        public HttpResponseMessage QueryHistoryDataExtend([FromBody] DataQueryParamObject QueryParams)
        {

            //DataQueryParamObject queryParam = null;
            QueryResultBase queryResult = null;


            if (QueryParams == null)
            {
                queryResult = new QueryResultBase();
                queryResult.ResultCode = QueryResultCodeEnum.QUERY_ERROR_PARAM;


                return HttpResponseExtension.toJson(JsonConvert.SerializeObject(queryResult));
            }
            IotDataQueryInterface dataQueryInterfaceObject = new IotDataQueryInterface();
            queryResult = dataQueryInterfaceObject.HandleQueryInterfaceExtend(QueryParams);

            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(queryResult));


        }

        [HttpPost]
        public HttpResponseMessage StatQueryData([FromBody] DataStatParamInfo StatParams)
        {

            //DataQueryParamObject queryParam = null;
            QueryResultBase queryResult = null;


            if (StatParams == null)
            {
                queryResult = new QueryResultBase();
                queryResult.ResultCode = QueryResultCodeEnum.QUERY_ERROR_PARAM;

                return HttpResponseExtension.toJson(JsonConvert.SerializeObject(queryResult));
            }
            IotDataStatInterface dataStatInterfaceObject = new IotDataStatInterface();
            queryResult = dataStatInterfaceObject.HandleStatInterface(StatParams);

            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(queryResult));


        }

        [HttpGet]
        public HttpResponseMessage GetDataStoreConfig(string CompanyCode, string DeviceCode)
        {

            QueryResultBase queryResult = null;
            

           List<DataStoreTableInfo> deviceDataStoreTableConfig =
                CompanyManagerHelper.GetDeviceDataStoreConfig(CompanyCode,DeviceCode);
            

            if (deviceDataStoreTableConfig == null)
            {
                queryResult = new QueryResultBase();
                queryResult.ResultCode = QueryResultCodeEnum.QUERY_ERROR_GET_DEVICE_STORE_CONFIG; 

            }

            queryResult = new QueryResultBase();
            queryResult.ResultCode = QueryResultCodeEnum.QUERY_SUCCESS;
            queryResult.QueryData = deviceDataStoreTableConfig;




            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(queryResult));



        }
    }
}
