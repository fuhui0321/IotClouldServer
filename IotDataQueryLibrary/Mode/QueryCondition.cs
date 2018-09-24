using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.ShardingDataQueryLibrary.Mode
{
    public class QueryCondition
    {
        public string TableName { get; set; }
        public string SelectCondition { get; set; }
        public long offset { get; set; }
        public long count { get; set; }
    }
}
