using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Modes
{
    public enum CONNECT_STATUS
    {
        ON_LINE,
        OFF_LINE
    };

    public enum DEVICE_RUN_STATUS
    {
        DEVICE_STOP,
        DEVICE_RUN,
        DEVICE_FAULT
    };



    

    public class DeviceStatusBase
    {
        public string DeviceName { get; set; }
        public string DeviceCode { get; set; }
        public UInt32 RuntimeCount { get; set; }
        public CONNECT_STATUS ConnectStatus { get; set; }
        public int    Temperature { get; set; }
        public DEVICE_RUN_STATUS RunStatus { get; set; }
        public string DeviceType { get; set; }
        public string DeviceAddress { get; set; }
        public string GPS { get; set; }
        public int UpdateTimestamp { get; set; }

        public string UpdateTime { get; set; }

    }
}
