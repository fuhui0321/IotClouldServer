using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Modes
{
    //public enum DEVICE_STATUS
    //{
    //    DEVICE_STOP,
    //    DEVICE_RUN,
    //    DEVICE_FAULT
    //};


    //public class DeviceInfo :  DeviceBase
    //{





    //    public DEVICE_STATUS DeviceStatus { get; set; }



    //}



    public class DeviceConfig 
    {


        public long UpdateTimeout = 5;

        public string RuntimeTagName = "DeviceInfo\\Device1\\DAQServiceStartCount";

        public string RunStatusTagName = "DeviceInfo\\Device1\\DAQServiceHeartbeatLive";

        public string Temperature = "DeviceInfo\\Device1\\CPUTemperature";


    }
}
