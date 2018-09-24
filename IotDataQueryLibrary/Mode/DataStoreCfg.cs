using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.ShardingDataQueryLibrary.Mode
{

    
    public class FieldInfo
    {
        public String Name { get; set; }
        //public String Tag { get; set; }
        public int Type { get; set; }
        public String Unit { get; set; }

    }

    
    public class DataStoreTable
    {
        public FieldInfo[] FieldList { get; set; }                           
        public SPLITE_TABLE_TYPE SpliteTableType;
    }

    public class DataStoreConfig
    {
        public String CompanyCode  { get; set; }
        public String DeviceCode { get; set; }
        public String DefaultTableName { get; set; }
        public SPLITE_TABLE_TYPE SpliteTableType { get; set; }
        public String FieldInfo { get; set; }
        public int SaveInterval { get; set; }
        public int StoreTime { get; set; }
        public String StoreCondition { get; set; }
        public String DataType { get; set; }
        public int BufferTime { get; set; }
        public int BufferSize { get; set; }

        public String DataStoreKey { get; set; }



    }
}
