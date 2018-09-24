using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Modes
{
    public enum ONLINE_STATE
    {
        ON_LINE,
        OFF_LINE
    };


    public class DeviceStateInfo
    {
        private DateTime _mLastUpdateTime;
        private ONLINE_STATE _mOnlineState;

        public DeviceBase DeviceInfo { get; set; }

        public DeviceGPSInfo DeviceGPS { get; set; }

        public ONLINE_STATE OnlineState
        { get; set; }

        public string DeviceTicket { get; set; }

        public int OfflineTimeout { get; set; }


        public void CheckOnline()
        {

        }
        
    }
}
