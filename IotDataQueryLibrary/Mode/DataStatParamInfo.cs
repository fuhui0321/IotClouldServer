using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.ShardingDataQueryLibrary.Mode
{
    public enum STAT_TYPE
    {
        STAT_YEAR,
        STAT_QUARTER,//quarter
        STAT_MONTH,
        STAT_WEEK,
        STAT_DAY,
        STAT_HOUR
    };

    public enum DATA_TYPE
    {
        DATA_SUM,
        DATA_CUSTOM_ACC
    };

    public class DataStatParamInfo: DataQueryParamObject
    {
        
        public DATA_TYPE DataType { get; set; }
        public STAT_TYPE StatType { get; set; }
    }
}
