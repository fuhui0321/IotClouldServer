using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.ShardingDataQueryLibrary.Mode
{

    public enum FIELD_TYPE
    {
        FIELD_BOOL,
        FIELD_INT,
        FIELD_FLOAT,
        FIELD_STRING
    };
    public class DataField
    {
        String TagName { get; set; }
        String FieldName { get; set; }
        FIELD_TYPE FieldType { get; set; }
        String FieldUnit { get; set; }


    }
}
