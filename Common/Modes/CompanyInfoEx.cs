using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Modes
{
    public class CompanyInfoEx:CompanyBase
    {
        public int DeviceCount { get; set; }
        
        public RedisInfo Redis { get; set; }

        public DBInfo Database { get; set; }

        public String DBConnectString { get; set; }
    }
}
