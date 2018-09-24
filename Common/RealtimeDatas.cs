using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IotCloudService.Common;
using IotCloudService.Common.Modes;


namespace IotCloudService.Common
{
    public class RealtimeDatas
    {
        public DeviceBase DeviceInfo { get; set; }
        public TagItem[] Datas { get; set; }
    }
}
