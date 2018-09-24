using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.ShardingDataQueryLibrary.Mode
{
    public class QueryResultExtend
    {
        public long SumRecordCount { get; set; }
        public long SumPageCount { get; set; }
        public long CurrentPageIndex { get; set; }
        public List<RecordsetDataExtend> QueryRecordset { get; set; }
    }
}
