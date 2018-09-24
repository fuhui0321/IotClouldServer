using IotCloudService.Common.Helper;
using IotCloudService.Common.Modes;
using IotCloudService.IotWebServerWebApi.Common;
using IotCloudService.IotWebServerWebApi.Models;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace IotCloudService.IotWebServerWebApi
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ServiceController : ApiController
    {        


        /// <summary>
        /// 根据用户名获取token
        /// </summary>
        /// <param name="staffId"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetToken([FromBody]LoginUserInfo loginUser)
        {
            ResultMsg resultMsg = null;
            int id = 0;

            //判断参数是否合法
            if (string.IsNullOrEmpty(loginUser.Timestamp) || (int.TryParse(loginUser.Timestamp, out id)))
            {
                resultMsg = new ResultMsg();
                resultMsg.StatusCode = (int)StatusCodeEnum.ParameterError;
                resultMsg.Info = StatusCodeEnum.ParameterError.GetEnumText();
                resultMsg.Data = "";
                return HttpResponseExtension.toJson(JsonConvert.SerializeObject(resultMsg));
            }

            UserInfo currUserInfo = UserManagerHelper.CheckUser(loginUser);

            if (currUserInfo == null)
            {
                resultMsg = new ResultMsg();
                resultMsg.StatusCode = (int)StatusCodeEnum.UnknownUserOrPassword;
                resultMsg.Info = StatusCodeEnum.UnknownUserOrPassword.GetEnumText();
                resultMsg.Data = "";
                return HttpResponseExtension.toJson(JsonConvert.SerializeObject(resultMsg));

            }

            //插入缓存
            Token token =(Token)HttpRuntime.Cache.Get(id.ToString());
            if (HttpRuntime.Cache.Get(id.ToString()) == null)
            {
                token = new Token();
                token.StaffId = id;
                token.SignToken = Guid.NewGuid();
                token.ExpireTime = DateTime.Now.AddDays(1);
                HttpRuntime.Cache.Insert(token.StaffId.ToString(), token, null, token.ExpireTime, TimeSpan.Zero);
            }

            //返回token信息
            resultMsg =new ResultMsg();
            resultMsg.StatusCode = (int)StatusCodeEnum.Success;
            resultMsg.Info = getJsonByObject(currUserInfo);
            resultMsg.Data = token;

            return HttpResponseExtension.toJson(JsonConvert.SerializeObject(resultMsg));
        }

        private string getJsonByObject(Object obj)
        {
            //实例化DataContractJsonSerializer对象，需要待序列化的对象类型
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            //实例化一个内存流，用于存放序列化后的数据
            MemoryStream stream = new MemoryStream();
            //使用WriteObject序列化对象
            serializer.WriteObject(stream, obj);
            //写入内存流中
            byte[] dataBytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(dataBytes, 0, (int)stream.Length);
            //通过UTF8格式转换为字符串
            return Encoding.UTF8.GetString(dataBytes);
        }
    }

    
}
