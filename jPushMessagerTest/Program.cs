using IotCloudService.IotMessagePushLibrary.JPush;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jPushMessagerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //JPushMessager.PushMessage("", "");
            JPushMessager.PushMessageToUser("", "", "");
        }
    }
}
