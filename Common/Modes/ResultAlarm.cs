using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Modes
{

    public class AlarmState
    {
        public string uuid_code { get; set; }
        public string AlarmTime { get; set; }
    }
    public class ResultAlarm: ResultBase
    {
        public AlarmState[] AlarmUuidList { get; set; }
    }
}
