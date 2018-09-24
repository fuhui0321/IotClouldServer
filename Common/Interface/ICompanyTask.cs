using IotCloudService.Common.Helper;
using IotCloudService.Common.Modes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Interface
{
    public interface ICompanyTask
    {
        void InitializeTask(CompanyHelper companyInfo);
        void RunTask();
        void StopTask(); 
    }
}
