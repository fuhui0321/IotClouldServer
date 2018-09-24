using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.ShardingDataQueryLibrary.Mode
{
    public class StatResultBase
    {
        public long SumRecordCount { get; set; }
        public List<RecordsetDataStat> StatRecordset { get; set; }
    }
}
