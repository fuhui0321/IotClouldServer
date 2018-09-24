using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Modes
{
    public class RedisInfo
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Passowrd { get; set; }

        public int DB { get; set; }
    }
}
