using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IotCloudService.Common;

namespace IotCloudService.Models
{
    public class AlarmDatas
    {

        public DeviceBase DeviceInfo { get; set; }
        public AlarmItemBase[] Datas { get; set; }
    }
}
