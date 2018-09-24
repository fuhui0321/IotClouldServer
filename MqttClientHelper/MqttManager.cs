using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using IotCloudService.Common.Helper;
using System.Net;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace IotCloudService.MqttClientHelper
{
    public class MqttManager
    {
         static MqttClient _client;
         static string mqttHost;
         static int mqttPort;
         static string mqttPassword;




        static MqttManager()
        {
            CreateManager();
        }


        static private void CreateManager()
        {
            initManager();

            connectMqtt();


        }

        static private void initManager()
        {
            //IPEndPoint result = null;
            var host = ConfigHelper.MqttEndPoint.Trim();
            if (host.IndexOf("@") > -1)
            {
                var hostParts = host.Split('@');
                mqttPassword = hostParts[0];
                var ip = hostParts[1].Split(':');

                mqttHost = ip[0];
                mqttPort = int.Parse(ip[1]);


            }
            else
            {
                var hostParts = host.Split(':');
                mqttHost = hostParts[0];
                mqttPort = int.Parse(hostParts[1]);

            }


            _client = new MqttClient(mqttHost);            

        }

        static private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {

            string msg = System.Text.Encoding.Default.GetString(e.Message);
           
        }

        static private void client_ConnectionClosedEventHandler(object sender, EventArgs e)
        {

            //string msg = System.Text.Encoding.Default.GetString(e.Message);

        }

        static private int disconnectMqtt()
        {
            try
            {
              _client.Disconnect();

            }
            catch (Exception ex)
            {

            }

            return 0;

        }

        //static public bool isConnect()
        //{
        //    bool bRes = true;

        //    bRes =_client.IsConnected;

        //    if (bRes)


        //    return bRes;
        //}

        static private int connectMqtt()
        {
            try
            {
                
                string clientId = Guid.NewGuid().ToString();

                _client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
                _client.ConnectionClosed += client_ConnectionClosedEventHandler;

                _client.Connect(clientId);
                
            }
            catch(Exception ex)
            {

            }
           

            return 0;
        }

        static public void client_MqttMsgPublist(string Topic,string Message)
        {

            bool bRes = true;

            bRes = _client.IsConnected;

            if(bRes == false)
            {
                disconnectMqtt();
                connectMqtt();
            }


            _client.Publish(Topic, Encoding.UTF8.GetBytes(Message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }

    }
}
