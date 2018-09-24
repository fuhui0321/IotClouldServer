using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Modes
{
    public class ResultBase
    {
        public ResultBase()
        {
            this.IsSuccess = true;
        }
        public bool IsSuccess { get; set; }
        
        public string ErrorDetail { get; set; }

        public string ErrorMessage { get; set; }
    }
}
