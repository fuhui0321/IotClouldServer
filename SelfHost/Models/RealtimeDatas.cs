using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IotCloudService.Common;
namespace IotCloudService.Models
{
    public class RealtimeDatas
    {
        public DeviceBase DeviceInfo { get; set; }
        public TagItem[] Datas { get; set; }
    }
}
