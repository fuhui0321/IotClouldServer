using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Modes
{
    public class InsertParamsBase
    {
        public String CompanyCode { set; get; }
        public String DeviceCode { set; get; }
    }

    public class InsertParamsFileUpload: InsertParamsBase
    {
        public FileUploadInfo FileUploadObject { set; get; }
    }
}
