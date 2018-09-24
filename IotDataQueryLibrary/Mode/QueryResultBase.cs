using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.ShardingDataQueryLibrary.Mode
{

    public enum QueryResultCodeEnum
    {
       QUERY_SUCCESS,       
       QUERY_ERROR_NO_DATA_STORE_CONFIG,
       QUERY_ERROR_ANALYZE_SQL,
       QUERY_ERROR_GET_QUERY_RECORDSET_COUNT,       
       QUERY_ERROR_GET_QUERY_RECORDSET_DATA,
       QUERY_ERROR_PARAM,
       QUERY_ERROR_GET_DEVICE_STORE_CONFIG,
       QUERY_ERROR_DB_SQL


    }
    public class QueryResultBase
    {
        public QueryResultCodeEnum ResultCode { get; set; }          
        public object QueryData { get; set; }        
    }
}
