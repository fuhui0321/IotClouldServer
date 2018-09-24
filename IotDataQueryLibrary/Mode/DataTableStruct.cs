using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.ShardingDataQueryLibrary.Mode
{

    public enum SPLITE_TABLE_TYPE
    {
        SPLITE_DEFAULT,
        SPLITE_YEAR,
        SPLITE_MONTH,
        SPLITE_WEEK,
        SPLITE_DAY
    };

    public class DataTableStruct
    {
        String TableNamePrefix { get; set; }
        String DataType { get; set; }
        SPLITE_TABLE_TYPE SpliteTableType { get; set; }
        List<DataField> FieldList { get; set; }

    }
}
