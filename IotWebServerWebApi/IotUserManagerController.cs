using IotCloudService.Common.Helper;
using IotCloudService.Common.Modes;
using IotCloudService.IotWebServerWebApi.Common;
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
using System.Web.Http.Results;

namespace IotCloudService.IotWebServerWebApi
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class IotUserManagerController : ApiController
    {


        [HttpGet]
        public HttpResponseMessage GetAllUserList(String CompanyCode)
        {
            QueryResultBase queryResult = new QueryResultBase();

            queryResult.QueryData = UserManagerHelper.GetAllUserList(CompanyCode);
            queryResult.ResultCode = QueryResultCodeEnum.QUERY_SUCCESS; 


            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(queryResult));
        }

        [HttpPost]
        public HttpResponseMessage DeleteUser(UserInfo DeleteUser)
        {
            QueryResultBase queryResult = new QueryResultBase();

            bool Res = UserManagerHelper.DeleteUser(DeleteUser);

            if (Res == true)
            {
                queryResult.ResultCode = QueryResultCodeEnum.QUERY_SUCCESS;
            }
            else
            {
                queryResult.ResultCode = QueryResultCodeEnum.QUERY_ERROR_DB_SQL;
            }
           


            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(queryResult));
        }

        [HttpPost]
        public HttpResponseMessage InsertUser(UserInfo NewUser)
        {
            QueryResultBase queryResult = new QueryResultBase();

            bool Res = UserManagerHelper.AddUser(NewUser);

            if (Res == true)
            {
                queryResult.ResultCode = QueryResultCodeEnum.QUERY_SUCCESS;
            }
            else
            {
                queryResult.ResultCode = QueryResultCodeEnum.QUERY_ERROR_DB_SQL;
            }


            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(queryResult));
        }

        [HttpPost]
        public HttpResponseMessage UpdateUser(UserInfo UpdateUser)
        {
            QueryResultBase queryResult = new QueryResultBase();

            bool Res = UserManagerHelper.ModifyUser(UpdateUser);

            if (Res == true)
            {
                queryResult.ResultCode = QueryResultCodeEnum.QUERY_SUCCESS;
            }
            else
            {
                queryResult.ResultCode = QueryResultCodeEnum.QUERY_ERROR_DB_SQL;
            }


            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(queryResult));
        }
    }
}
