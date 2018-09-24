using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Modes
{
    public enum LOGIN_STATUS
    {
        USER_LOGIN,
        USER_LOGOUT
    }
    public class UserInfo
    {
        public string CompanyCode { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }               
        public string AuthLevel { get; set; }
        public string[] DeviceList { get; set; }
        //public LOGIN_STATUS LoginStatus { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class LoginUserInfo
    {
        public string PhoneNumber { get; set; }
        public string PasswordMD5 { get; set; }
        public string Timestamp { get; set; }

    }
}
