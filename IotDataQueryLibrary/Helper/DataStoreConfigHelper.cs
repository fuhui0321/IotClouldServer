using IotCloudService.Common.Helper;
using IotCloudService.Common.Redis;
using IotCloudService.ShardingDataQueryLibrary.Mode;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IotCloudService.ShardingDataQueryLibrary.Helper
{
    public class DataStoreConfigHelper
    {

        private static Dictionary<string, DataStoreConfig> mapDataStoreConfig = new Dictionary<string, DataStoreConfig>();
        private static string Conn = ConfigurationManager.AppSettings["MySqlConnectString"];
        private static string RedisTableName = "DataStoreManager";
        private static string RedisDeviceTableName = "DeviceManagerConfig";

        DataStoreConfigHelper()
        {

        }

        public void InitDataStoreConfigHelper()
        {

        }

        private void ReadDataStoreConfigFromRedis()
        {

            var client = RedisManager.GetClient();
            string[] arrayDataStoreCfgJson;
            arrayDataStoreCfgJson = client.HMGet(RedisTableName);

        }


        static public DeviceDataStoreManager GetDeviceDataStoreConfig(string companyCode, string deviceCode)
        {

            DeviceDataStoreManager DataStoreItem = null;

            string DataStoreKey = $"[{companyCode}]-[{deviceCode}]";
            string DataStoreJson;
            var client = RedisManager.GetClient();

            if (client == null)
                return DataStoreItem;

            client.Select(1);

            DataStoreJson = client.HGet(RedisDeviceTableName, DataStoreKey);

            client.Select(0);

            if (string.IsNullOrEmpty(DataStoreJson))
                return DataStoreItem;

            try
            {
                DataStoreItem = JsonConvert.DeserializeObject<DeviceDataStoreManager>(DataStoreJson);
            }
            catch (Exception ex)
            {
                DataStoreItem = null;
            }


            return DataStoreItem;

        }


            static public DataStoreTable GetDataStoreConfig(string companyCode,string deviceCode,string tableName)
        {
            DataStoreTable DataStoreItem = null;
            string DataStoreKey = $"[{companyCode}]-[{deviceCode}]-[{tableName}]";
            string DataStoreJson;
            var client = RedisManager.GetClient();

            if (client == null)
                return DataStoreItem;

            client.Select(1);

            DataStoreJson = client.HGet(RedisTableName,DataStoreKey);

            client.Select(0);

            if (string.IsNullOrEmpty(DataStoreJson))
                return DataStoreItem;

            try
            {
                DataStoreItem = JsonConvert.DeserializeObject<DataStoreTable>(DataStoreJson);
            }
            catch(Exception ex)
            {
                DataStoreItem = null;
            }

            
                        
            return DataStoreItem;

        }



    }
}
