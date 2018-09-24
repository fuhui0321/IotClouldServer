using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.ShardingDataQueryLibrary.Mode
{
    public class DeviceDataStoreManager
    {
        public String CompanyCode { set; get; }
        public String DeviceCode { set; get; }
        public String DeviceName { set; get; }

        public List<DeviceDataStoreTable> DataStoreConfig { set; get; }
    }

    public class DeviceDataStoreTable
    {
        public String DataTableName { set; get; }
        public List<FieldInfo> FieldList { set; get; }

    }
}
