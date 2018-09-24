using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Modes
{
    public enum DEVICE_DATA_RECORD_STATUS
    {
        DEVICE_RECORD_START,
        DEVICE_RECORD_STOP
    };
    public class DeviceDataRecordStatus
    {
        public DeviceBase DeviceInfo { get; set; }
        public DEVICE_DATA_RECORD_STATUS DataRecordStatus { get; set; }
    }
}
