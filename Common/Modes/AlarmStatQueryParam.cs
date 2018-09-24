using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IotCloudService.Common.Modes
{
    
    public class AlarmStatQueryParam
    {
        public string CompanyCode;
        public string[] DeviceCode;
        public string[] AlarmStatName;
        
    }

    public class AlarmStatQueryResponse
    {
        public string[] AlarmStatValue;
    }
}
