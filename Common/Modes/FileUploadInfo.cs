using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Modes
{
    public class FileUploadInfo
    {
        public int ID { set; get; }
        public String DateTime { set; get; }
        public String Content { set; get; }
        public String UserName { set; get; }
        public string[] FilePathList { set; get; }
    }
}
