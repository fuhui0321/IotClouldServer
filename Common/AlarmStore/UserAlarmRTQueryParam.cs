using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IotCloudService.Common.Modes;

namespace IotCloudService.Common.AlarmStore
{
    public class UserAlarmRTQueryParam
    {
        public String UserName { set; get; }        
        public int PageNumber { set; get; }
    }
}
