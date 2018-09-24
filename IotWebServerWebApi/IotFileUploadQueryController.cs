using IotCloudService.Common.Helper;
using IotCloudService.Common.Modes;
using IotCloudService.IotWebServerWebApi.Common;
using IotCloudService.IotWebServerWebApi.Models;
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
    public class IotFileUploadQueryController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage QueryData([FromBody] QueryConditionBase QueryParams)
        {
            ResultMsg resultMsg = null;
            List<FileUploadInfo> queryFileUploadInfoList = null;
            bool res = true;


            FileUploadInfoQueryHelper tempQueryHelper = new FileUploadInfoQueryHelper();
            tempQueryHelper.Initialize(QueryParams.CompanyCode, QueryParams.DeviceCode);

            queryFileUploadInfoList = tempQueryHelper.QueryFileUploadInfo(QueryParams);

           

            resultMsg = new ResultMsg();
            resultMsg.StatusCode = (int)StatusCodeEnum.Success;
            resultMsg.Info = "";
            resultMsg.Data = queryFileUploadInfoList;

            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(resultMsg));
            
        }

        [HttpPost]
        public HttpResponseMessage InsertData([FromBody] InsertParamsFileUpload InsertParams)
        {
            ResultMsg resultMsg = null;
            List<FileUploadInfo> queryFileUploadInfoList = null;
            bool res = true;


            FileUploadInfoQueryHelper tempQueryHelper = new FileUploadInfoQueryHelper();
            tempQueryHelper.Initialize(InsertParams.CompanyCode, InsertParams.DeviceCode);

            res = tempQueryHelper.InsertFileUploadInfo(InsertParams.FileUploadObject);

            if (res == true)
            {
                resultMsg = new ResultMsg();
                resultMsg.StatusCode = (int)StatusCodeEnum.Success;
                resultMsg.Info = "";
                
            }
            else
            {
                resultMsg = new ResultMsg();
                resultMsg.StatusCode = (int)StatusCodeEnum.InsertRecordError;
                resultMsg.Info = StatusCodeEnum.InsertRecordError.GetEnumText(); ;
                resultMsg.Data = queryFileUploadInfoList;

            }



           

            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(resultMsg));
            
        }

        [HttpPost]
        public HttpResponseMessage DeleteData([FromBody] QueryConditionBase QueryParams)
        {
            ResultMsg resultMsg = null;
            List<FileUploadInfo> queryFileUploadInfoList = null;
            bool res = true;


            FileUploadInfoQueryHelper tempQueryHelper = new FileUploadInfoQueryHelper();
            tempQueryHelper.Initialize(QueryParams.CompanyCode, QueryParams.DeviceCode);

            queryFileUploadInfoList = tempQueryHelper.QueryFileUploadInfo(QueryParams);



            resultMsg = new ResultMsg();
            resultMsg.StatusCode = (int)StatusCodeEnum.Success;
            resultMsg.Info = "";
            resultMsg.Data = queryFileUploadInfoList;

            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(resultMsg));



        }

    }
}
