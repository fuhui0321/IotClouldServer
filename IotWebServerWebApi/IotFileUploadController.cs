using IotCloudService.Common.Helper;
using IotCloudService.IotWebServerWebApi.Common;
using IotCloudService.IotWebServerWebApi.Helper;
using IotCloudService.IotWebServerWebApi.Models;
using IotCloudService.ShardingDataQueryLibrary.Mode;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace IotCloudService.IotWebServerWebApi
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class IotFileUploadController : ApiController
                 
    {

        //static readonly string UploadFolder = "/UpLoad/ApiUpload";
        //static readonly string ServerUploadFolder = HttpContext.Current.Server.MapPath(UploadFolder);


        [HttpPost]
        public HttpResponseMessage UploadTest()
        {
            QueryResultBase queryResult = null;

            queryResult = new QueryResultBase();
            queryResult.ResultCode = QueryResultCodeEnum.QUERY_ERROR_GET_DEVICE_STORE_CONFIG;

            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(queryResult));
        }

        [HttpPost]
        public HttpResponseMessage UploadTest1()
        {
            QueryResultBase queryResult = null;

            queryResult = new QueryResultBase();
            queryResult.ResultCode = QueryResultCodeEnum.QUERY_ERROR_GET_DEVICE_STORE_CONFIG;

            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(queryResult));
        }


        [HttpPost]
        public async Task<FileResult> UploadSingleFile()
        {
            String CompanyCode = "";
            String DeviceCode = "";
            String UserPhone = "";

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var streamProvider = new RenamingMultipartFormDataStreamProvider(FileServerHelper.GetUploadFileRoot(),
                CompanyCode, DeviceCode, UserPhone);
            await Request.Content.ReadAsMultipartAsync(streamProvider);



            return new FileResult
            {
                FileNames = streamProvider.FileData.Select(entry => entry.LocalFileName)
            };
        }


        public class RenamingMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
        {
            public string Root { get; set; }
            public string CompanyCode { get; set; }
            public string DeviceCode { get; set; }
            public string UserPhone { get; set; }






            public RenamingMultipartFormDataStreamProvider(string root, string companyCode,
            string deviceCode, string userPhone)
            : base(root)
            {
            Root = root;
            CompanyCode = companyCode;
            DeviceCode = deviceCode;
            UserPhone = userPhone;
        }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            string filePath = headers.ContentDisposition.FileName;

            string localFileName = $"{CompanyCode}_{DeviceCode}_{UserPhone}_{Guid.NewGuid()}";


            //// Multipart requests with the file name seem to always include quotes.
            //if (filePath.StartsWith(@"""") && filePath.EndsWith(@""""))
            //    filePath = filePath.Substring(1, filePath.Length - 2);

            var filename = Path.GetFileName(filePath);
            var extension = Path.GetExtension(filePath);
            var contentType = headers.ContentType.MediaType;

            return localFileName;
        }

    }
}
}
