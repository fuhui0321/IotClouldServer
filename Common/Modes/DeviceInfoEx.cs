using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Modes
{
    public class DeviceInfoEx:DeviceBase
    {
        public string DeviceName { set; get; }
        public string Address { set; get; }
        public int DeviceType { set; get; }
        public string DeviceTypeName { set; get; }
    }
}
