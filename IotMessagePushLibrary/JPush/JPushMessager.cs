using Jiguang.JPush;
using Jiguang.JPush.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.IotMessagePushLibrary.JPush
{
    public class JPushMessager
    {
        private static String app_key = "06a6e94a60b9a6b9e75ef978";
        private static String master_secret = "7290e55da5dca001b8e866d9";
       
        public JPushMessager()
        {

        }

        public static int PushMessageToUser(string pushDeviceID, string pushTitle, string pushMessager)
        {
            JPushClient jpush_client = new JPushClient(app_key, master_secret);

            

            Audience audience = new Audience();
           
            audience.Alias = new List<string>(new string[] { "13995583303" });           


            PushPayload pushPayload = new PushPayload()
            {
                Platform = "android",
                Audience = audience,


                Notification = new Notification()
                {
                    Alert = "hello jpush",
                    Android = new Android()
                    {
                        Alert = "android alert",
                        Title = "title"
                    },
                    IOS = new IOS()
                    {
                        Alert = "ios alert",
                        Badge = "+1"
                    }
                },
                Message = new Message()
                {
                    Title = "message title",
                    Content = "message content",
                   
                }
            };           
            var response = jpush_client.SendPush(pushPayload);

            Console.WriteLine(response.Content);






            return 0;
        }

        public static int PushMessageToGroup(string pushTag, string pushTitle, string pushMessager)
        {
            JPushClient jpush_client = new JPushClient(app_key, master_secret);
            var registrationId = "12145125123151";
            var devicePayload = new DevicePayload()
            {
                Alias = "alias1",
                Mobile = "12300000000",
                Tags = new Dictionary<string, object>()
                {
                    { "add", new List<string>() { "tag1", "tag2" } },
                    { "remove", new List<string>() { "tag3", "tag4" } }
                }
            };
            var response = jpush_client.Device.UpdateDeviceInfo(registrationId, devicePayload);

            

            Console.WriteLine(response.Content);

            return 0;
        }




        public static void PushMessage(string pushTitle, string pushMessager)
        {
            JPushClient jpush_client = new JPushClient(app_key, master_secret);

            PushPayload pushPayload = new PushPayload()
            {
                Platform = "android",
                Audience = "all",
                Notification = new Notification()
                {
                    Alert = "hello jpush",
                    Android = new Android()
                    {
                        Alert = "android alert",
                        Title = "title"
                    },
                    IOS = new IOS()
                    {
                        Alert = "ios alert",
                        Badge = "+1"
                    }
                },
                Message = new Message()
                {
                    Title = "message title",
                    Content = "message content",
                    Extras = new Dictionary<string, string>()
                    {
                        ["key1"] = "value1"
                    }
                }
            };
            var response = jpush_client.SendPush(pushPayload);
            Console.WriteLine(response.Content);

        }


    }
}
