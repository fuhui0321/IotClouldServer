using IotCloudService.Common.Modes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Modes
{
    public class AlarmListInfo
    {
        public DeviceBase DeviceInfo { set; get; }
        public List<AlarmInfo> AlarmList { set; get; }
    }
}
