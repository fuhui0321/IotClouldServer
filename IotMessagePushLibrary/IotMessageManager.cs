using IotCloudService.IotMessagePushLibrary.JPush;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.IotMessagePushLibrary
{
    public class IotMessageManager
    {
        

        public static void Initialize()
        {

        }

        public static int PushMessageToUser(string pushDeviceID, string pushTitle,string pushMessager)
        {
            

            return 0;
        }

        public static int PushMessageToGroup(string pushTag,string pushTitle, string pushMessager)
        {
            return 0;
        }

        public static int PushMessageToBroadcast(string pushTitle, string pushMessager)
        {

            JPushMessager.PushMessage(pushTitle, pushMessager);
            return 0;
        }
    }
}
