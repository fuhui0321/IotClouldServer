using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.AlarmStore
{
    public class RealtimeAlarmQueryParam
    {
        public string companyCode;
        public string deviceCode;
        public List<int> alarmLevel;
        public int pageSize;
        public int pageNum;
    }
}
