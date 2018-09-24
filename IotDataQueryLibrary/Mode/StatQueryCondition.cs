using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.ShardingDataQueryLibrary.Mode
{
    public class StatQueryCondition
    {
        public string TableName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public long offset { get; set; }
        public long count { get; set; }
    }
}
