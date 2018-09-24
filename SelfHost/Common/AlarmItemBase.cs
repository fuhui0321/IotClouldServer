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

namespace IotCloudService.Common
{
    public class AlarmItemBase
    {
        public string AlarmTime { get; set; }
        public string AlarmName { get; set; }
        public string AlarmLevel { get; set; }
        public string AlarmType { get; set; }

    }
}
