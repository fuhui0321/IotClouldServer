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
    public class IotGetRtdbTag : ApiController
    {
        [HttpGet]
        public ResultBase GetTagValues([FromBody]RealtimeDatas GetRtDatas)
        {
            ResultBase res = new ResultBase();

            return res;
        }

        public ResultBase SetTagValues([FromBody]RealtimeDatas SetRtDatas)
        {
            ResultBase res = new ResultBase();

            return res;
        }
    }

}
