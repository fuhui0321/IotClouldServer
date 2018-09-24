using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Modes
{


    public enum DEVICE_VPV_STATUS
    {
        DEVICE_VPN_OPEN,
        DEVICE_VPN_CLOSE
    };
    public class DeviceVPNStatus
    {
        public DeviceBase DeviceInfo { get; set; }
        public DEVICE_VPV_STATUS VPNStatus { get; set; }
}
}
