using IotCloudService.Common.Modes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.AlarmStore
{
    public class AlarmQueryParam: QueryConditionBase
    {
        public string AlarmName { set; get; }        
    }
}
