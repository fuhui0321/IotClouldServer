using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Net.Http;

using IotCloudService.Models;

namespace IotCloudService
{


    public class IotUploadController : ApiController
    {

        public string UploadReadtimeDatas([FromBody]RealtimeDatas UploadDatas)
        {

            return "OK";
        }

        

        public string GetDatas()
        {
            return "OK";
        }




    }
}
