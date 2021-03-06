﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Modes
{
  
    public class AlarmInfo
    {
        public String AlarmDate { set; get; }
        public String RecoveryDate { set; get; }
        public String AlarmName { set; get; }
        public String AlarmUUID { set; get; }
        public int AlarmLevel { set; get; }
        public ALARM_TYPE AlarmType { set; get; }
        public String DeviceType { set; get; }
        public String DeviceName { set; get; }
        public String AlarmCondition { set; get; }
        public String AlarmHelp { set; get; }
        public long   AlarmSpanTime { set; get; }
        public String Reserved1 { set; get; }
        public String Reserved2 { set; get; }
        public String Reserved3 { set; get; }
        public String Reserved4 { set; get; }
        public String Reserved5 { set; get; }
       



    }

    public class AlarmInfoEx
    {
        public String DeviceCode { set; get; }

        public AlarmInfo AlarmInfoObject { set; get; }
              
    }
}
