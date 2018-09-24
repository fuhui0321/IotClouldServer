using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IotCloudService.Common.Modes;

namespace IotCloudService.Common
{
    public class AlarmDatas
    {

        public DeviceBase DeviceInfo { get; set; }
        public AlarmItemBase[] AlarmList { get; set; }
    }
}
