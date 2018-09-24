using IotCloudService.Common.Modes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.DataStore
{
    public class DataStoreConfigInfo
    {
        public DataStoreTableInfo TableInfo { set; get; }


        public string CompanyCode { set; get; }
        public string DeviceCode { set; get; }

        //数据存储间隔（单秒）
        public int SaveInterval { set; get; }
        //数据保存时间（秒）
        public int StoreTime { set; get; }
        public String DataType { set; get; }

        public int BufferTime { set; get; }
        public int BufferSize { set; get; }

        public string StoreCondition { set; get; }
    }
}
