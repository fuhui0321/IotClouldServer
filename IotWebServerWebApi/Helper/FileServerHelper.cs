using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.IotWebServerWebApi.Helper
{
    public class FileServerHelper
    {

        //private static FileServerHelper FileServerHelper = null;//池管理对象
        private static String UploadFilePath = ConfigurationManager.AppSettings["FileServerPath"];
        
        public static bool Initialize()
        {
            //如果路径不存在，创建路径  
            //if (!Directory.Exists(UploadFilePath))
            //    Directory.CreateDirectory(UploadFilePath);

            return true;
        }

        public static string GetUploadFileRoot()
        {
            return UploadFilePath;
        }





    }
}
