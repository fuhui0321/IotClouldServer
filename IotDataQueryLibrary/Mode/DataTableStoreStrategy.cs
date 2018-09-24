using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.ShardingDataQueryLibrary.Mode
{


    struct DATA_FIELD
    {
        string TagName;
        string FieldName;
        FIELD_TYPE FieldType;
        string FieldUnit;

    };
    class DataTableStoreStrategy
    {

        String CompanyCode;
        String DeviceCode;
        String DefaultTableName;
        int SpliteType;
        String FieldInfo;
        String DataType;


    }
}
