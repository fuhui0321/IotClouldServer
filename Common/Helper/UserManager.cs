using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;

using IotCloudService.Common.Modes;
using System.Configuration;
using System.Data;
using System.Runtime.Serialization.Json;
using System.IO;
using IotCloudService.Common.Redis;

using System.Web.Security;

namespace IotCloudService.Common.Helper
{
    public class UserManager
    {
        private static List<UserInfo> UserList = new List<UserInfo>();        
        private static Dictionary<string, UserInfo> mapUser = new Dictionary<string, UserInfo>();

        private static string Conn = ConfigurationManager.AppSettings["MySqlConnectString"];
        private static string redisUserInfoKey = "UserInfoTable";
        private static RSAHelper.RSAKey keyPair = RSAHelper.GetRASKey();
                                                     

        UserManager()
        {            
            //ReadUserInfoFromDB();
            //UpdateUserToRedis();
        }

        public static void InitUserManager()
        {
            ReadUserInfoFromDB();
            UpdateUserToRedis();

        }


        private static void ReadUserInfoFromDB()
        {
            MySqlDataReader userReader = MySqlHelper.ExecuteReader(Conn, CommandType.Text, "select * from user_info", null);

            while (userReader.Read())
            {
                UserInfo userItem = new UserInfo();

                userItem.CompanyCode = userReader.GetString(1);
                userItem.UserName = userReader.GetString(2);
                userItem.Password = userReader.GetString(3);
                //userItem.PasswordMD5 = userReader.GetString(4) + "";
                userItem.AuthLevel = string.IsNullOrEmpty(userReader.GetString(5)) == true?"" : userReader.GetString(5);


                string tempDeviceList = string.IsNullOrEmpty(userReader.GetString(6)) == true ? "" : userReader.GetString(6);
                userItem.DeviceList = tempDeviceList.Split(',');

                userItem.PhoneNumber = string.IsNullOrEmpty(userReader.GetString(7)) == true ? "" : userReader.GetString(7);

                UserList.Add(userItem);

                string userKey = $"[{ userItem.CompanyCode}]-[{ userItem.PhoneNumber}]";

                mapUser.Add(userKey,userItem);

            }
        }

        private static void UpdateUserToRedis()
        {
            Dictionary<string, string> redisUser = new Dictionary<string, string>();

            if (UserList.Count < 1)
            {
                return;
            }


            for (int i = 0; i < UserList.Count; i++)
            {
                string userKey = $"[{UserList[i].CompanyCode}]-[{UserList[i].UserName}]";
                string userInfo = getJsonByObject(UserList[i]);



                mapUser.Add(userKey, UserList[i]);
                redisUser.Add(userKey, userInfo);
            }

            var client = RedisManager.GetClient();
            client.HMSet(redisUserInfoKey, redisUser);

        }

        public string GetRasKey()
        {
            return keyPair.PublicKey;
        }

        public static bool CheckUser(LoginUserInfo loginUser)
        {
            UserInfo tempUser;

            if (mapUser.ContainsKey(loginUser.PhoneNumber) == true)
            {
                tempUser = mapUser[loginUser.PhoneNumber];

                //if (tempUser.LoginStatus == LOGIN_STATUS.USER_LOGIN)
                //{
                //    //用户已经登录
                //    return false;
                //}
               
            }
            else
            {
                return false;
            }
            

            string signatureMd5 = FormsAuthentication.HashPasswordForStoringInConfigFile(tempUser.PhoneNumber + tempUser.Password + loginUser.Timestamp,"MD5").ToLower();



            if (signatureMd5 == loginUser.PasswordMD5.ToLower())
            {
                return true;
            }
          


            return false;
        }

        public ResultLog Login(string CompanyCode,string UserName,string PasswrodMd5)
        {
            ResultLog loginResult = new ResultLog();
            string userKey = $"[{CompanyCode}]-[{UserName}]";


            if (isLogin(userKey) == true)
            {
                //该用户已经登录
            }

            UserInfo loginUser = CheckUserID(userKey);
            if (loginUser == null)
            {
                loginResult.IsSuccess = false;
                loginResult.ErrorMessage = "该用户不存在";
                //该用户不存在
                return loginResult;
            }

            if (VerifyUserPassword(userKey,PasswrodMd5,loginUser.Password) == false)
            {
                loginResult.IsSuccess = false;
                loginResult.ErrorMessage = "用户名或密码不正确";
                return loginResult;
            }

            loginResult.IsSuccess = true;
            

            return loginResult;
        }

        public bool Logout(UserInfo objUser)
        {
            return true;
        }

        public static string[] GetDeviceList(string userKey)
        {
            return mapUser[userKey].DeviceList;
        }


        private bool isLogin(string userKey)
        {
            //string userKey = $"[{CompanyCode}]-[{UserName}]";

            //UserInfo loginUser =  mapLoginUser[userKey];

            //if (loginUser != null)
            //{
            //    return true;
            //}


            return false;

        }

        private UserInfo CheckUserID(string userKey)
        {
            //string userKey = $"[{CompanyCode}]-[{UserName}]";
            var client = RedisManager.GetClient();
            string strUser = client.HGet(redisUserInfoKey,userKey);

            UserInfo currUser = new UserInfo();
            currUser = (UserInfo)getObjectByJson(strUser, (object)currUser);



            if ((currUser == null) )
                return null;

            return currUser;
        }

        private bool VerifyUserPassword(string userKey, string PasswordMd5,string verifyPassword)
        {
            //string userKey = $"[{CompanyCode}]-[{UserName}]";

            string userPassowrd = RSAHelper.DecryptString(userKey, keyPair.PublicKey);

            if(userPassowrd != verifyPassword)
            {
                return false;
            }


            return true;
        }

        private string CreateKey()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            string priKey = rsa.ToXmlString(true);
            string pubKey = rsa.ToXmlString(false);

            return "";
        }

        private static string getJsonByObject(Object obj)
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

        private  Object getObjectByJson(string jsonString, Object obj)
        {
            //实例化DataContractJsonSerializer对象，需要待序列化的对象类型
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            //把Json传入内存流中保存
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            // 使用ReadObject方法反序列化成对象
            return serializer.ReadObject(stream);
        }

        
        public UserInfo GetUserObject(string UserName)
        {
            return mapUser[UserName];
        }
    }
}
