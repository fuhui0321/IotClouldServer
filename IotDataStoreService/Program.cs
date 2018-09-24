using IotCloudService.Common.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotDataStoreService
{
    class Program
    {
        static void Main(string[] args)
        {
            MySqlConnectPoolHelper.getPool().InitMySqlConnectPool();
            CompanyManagerHelper.Initialize();
        }
    }
}
