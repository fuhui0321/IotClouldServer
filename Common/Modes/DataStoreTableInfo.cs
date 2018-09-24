using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Modes
{
    public class DataStoreTableInfo
    {
        public enum SPLITE_TABLE_TYPE
        {
            SPLITE_DEFAULT,
            SPLITE_YEAR,
            SPLITE_MONTH,
            SPLITE_WEEK,
            SPLITE_DAY
        };
        public DataStoreFieldInfo[] FieldList { get; set; }
        public SPLITE_TABLE_TYPE SpliteTableType { get; set; }
        public String TableName { get; set; }

        


    }
}
