using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.ShardingDataQueryLibrary.Mode
{

    public enum DATA_QUERY_TYPE
    {
        HISTORY_QUERY,
        ALARM_QUERY
    };
    public class DataQueryParamObject
    {

        public String CompanyCode { get; set; }
        public String DeviceCode { get; set; }
        public String TableName { get; set; }
        public String StartDate { get; set; }
        public String EndDate { get; set; }
       
        public int RecordOffset { get; set; }
        public int PageSize { get; set; }

        public bool isFirstQuery { get; set; }

        public DATA_QUERY_TYPE DataQueryType { get; set; }
    }
}
