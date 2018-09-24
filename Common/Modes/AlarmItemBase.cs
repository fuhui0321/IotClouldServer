/** 
*  @brief  AlarmItemBase
*  @author Fu Hui
*  @version 1.0
*  @date   2017-07-01
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Modes
{

    public enum ALARM_TYPE
    {
        ALRAM_RECOVERY,
        ALARM_OCCUR
    };

    public enum ALARM_LEVEL
    {
        ALRAM_LEVEL0,
        ALRAM_LEVEL1,
        ALRAM_LEVEL2,
        ALRAM_LEVEL3,
        ALRAM_LEVEL4
    };
    public class AlarmItemBase
    {
        public string AlarmUUID { get; set; }
        public string AlarmDate { get; set; }
        public string AlarmName { get; set; }
        public ALARM_LEVEL AlarmLevel { get; set; }
        public ALARM_TYPE AlarmType { get; set; }
        public string DeviceType { get; set; }
        public string DeviceName { get; set; }
        public string AlarmCondition { get; set; }
        public string AlarmHelp { get; set; }
        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }
        public string Reserved3 { get; set; }
        public string Reserved4 { get; set; }
        public string Reserved5 { get; set; }

    }
}
