using IotCloudService.Common.AlarmStore;
using IotCloudService.Common.DataStore;
using IotCloudService.Common.Modes;
using IotCloudService.Common.Redis;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IotCloudService.Common.Modes.DataStoreTableInfo;

namespace IotCloudService.Common.Helper
{
    public class DeviceHelper
    {

        string Conn = ConfigurationManager.AppSettings["MySqlConnectString"];
        DeviceStatusBase _deviceStatus = new DeviceStatusBase();
        DeviceInfoEx _deviceInfo;
        DeviceConfig _deviceConfig = new DeviceConfig();
        string _deviceRedisHashName;
        CompanyInfoEx _parentCompanyInfo;

        String companyRedisTableName;
        String companyConnStr;
        private DeviceAlarmStoreManager _deviceAlarmStoreManager = null;
        Dictionary<String, DataStoreTableInfo> _DataStoreMap = new Dictionary<string, DataStoreTableInfo>();

        private DataStoreManager _deviceDataStoreManager = new DataStoreManager();

        public DeviceHelper(DeviceInfoEx deviceInfo, CompanyInfoEx companyInfo)
        {
            _deviceInfo = deviceInfo;

            _deviceStatus.DeviceName = _deviceInfo.DeviceName;
            _deviceStatus.DeviceCode = _deviceInfo.DeviceCode;
            _deviceStatus.DeviceType = _deviceInfo.DeviceTypeName;
            _deviceStatus.DeviceAddress = _deviceInfo.Address;
            _deviceStatus.GPS = _deviceInfo.GPS;

            _deviceStatus.ConnectStatus = CONNECT_STATUS.OFF_LINE;
            _deviceStatus.UpdateTimestamp = 0;

            _deviceRedisHashName = $"[{deviceInfo.CompanyCode}]-[{deviceInfo.DeviceCode}]";

            _parentCompanyInfo = companyInfo;

            companyRedisTableName = $"[IotConfig]-[{_deviceInfo.CompanyCode}]";

            _deviceAlarmStoreManager = new DeviceAlarmStoreManager(deviceInfo);


        }



        public bool InitializeDataStoreManager()
        {
            String cmdSQL = $"select * from `datastoreconfig` where CompanyCode = '{_deviceInfo.CompanyCode}' and DeviceCode = '{_deviceInfo.DeviceCode}'";

            MySqlDataReader dataStoreConfigReader = MySqlHelper.ExecuteReader(Conn, CommandType.Text, cmdSQL, null);

            while (dataStoreConfigReader.Read())
            {
                DataStoreTableInfo dataStoreTableItem = new DataStoreTableInfo();
                DataStoreConfigInfo dataStoreConfigItem = new DataStoreConfigInfo();
                
                dataStoreTableItem.TableName = (string)dataStoreConfigReader["DefaultTableName"];               
                dataStoreTableItem.SpliteTableType = (SPLITE_TABLE_TYPE)(int)dataStoreConfigReader["SpliteType"];
                string fieldInfoJson = (string)dataStoreConfigReader["FieldInfo"];

                dataStoreTableItem.FieldList = JsonConvert.DeserializeObject<DataStoreFieldInfo[]>(fieldInfoJson);


                dataStoreConfigItem.TableInfo = dataStoreTableItem;

                dataStoreConfigItem.CompanyCode = (string)dataStoreConfigReader["CompanyCode"];
                dataStoreConfigItem.DeviceCode = (string)dataStoreConfigReader["DeviceCode"];
                dataStoreConfigItem.SaveInterval = (int)dataStoreConfigReader["SaveInterval"];
                dataStoreConfigItem.StoreTime = (int)dataStoreConfigReader["StoreTime"];
                //dataStoreConfigItem.DataType = (string)dataStoreConfigReader["DataType"];
                dataStoreConfigItem.StoreCondition = dataStoreConfigReader["StoreCondition"].ToString() ;
                dataStoreConfigItem.BufferTime = (int)dataStoreConfigReader["BufferTime"];
                dataStoreConfigItem.BufferSize = (int)dataStoreConfigReader["BufferSize"];


                _deviceDataStoreManager.AddDataStoreItem(dataStoreConfigItem);

                _DataStoreMap.Add(dataStoreTableItem.TableName, dataStoreTableItem);
            }

            return true;

        }

        public int InitializeAlarmStoreManager(string CompanyCnnString)
        {
            companyConnStr = CompanyCnnString;
            _deviceAlarmStoreManager.InitDeviceAlarmStoreManager(CompanyCnnString);
            _deviceDataStoreManager.InitializeDataStoreManager(companyConnStr, _parentCompanyInfo, _deviceInfo);
            return 0;
        }

        public DeviceStatusBase GetDeviceStatus()
        {

            var client = RedisManager.GetClient();
            DeviceStatusBase currDeviceStatus = null;

            if (client == null)
            {
                return null;
            }

            try
            {

                string DeviceStatusJsonString = client.HGet("DeviceStatusTable", _deviceRedisHashName);

                currDeviceStatus = JsonConvert.DeserializeObject<DeviceStatusBase>(DeviceStatusJsonString);


                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                long lTime = ((long)currDeviceStatus.UpdateTimestamp * 10000000);
                TimeSpan toNow = new TimeSpan(lTime);
                DateTime targetDt = dtStart.Add(toNow);


                currDeviceStatus.UpdateTime = targetDt.ToString(); 

            }
            catch(Exception ex)
            {

            }


                return currDeviceStatus;
        }

        public List<DataStoreTableInfo> GetDeviceDataStoreConfig()
        {
            List<DataStoreTableInfo> pList = new List<DataStoreTableInfo>();
            pList = _DataStoreMap.Values.ToList<DataStoreTableInfo>();

            return pList;

        }

        public DataStoreTableInfo GetDeviceDataStoreConfigItem(String tableName)
        {
             if (_DataStoreMap.ContainsKey(tableName))
            {
                return _DataStoreMap[tableName];
            }
            else
            {
                return null;
            }

        }


        public void UpdateDeviceDataUploadTime()
        {
            _deviceStatus.UpdateTimestamp = Convert.ToInt32((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
            _deviceStatus.ConnectStatus = CONNECT_STATUS.ON_LINE;


        }


        public void UpdateDeviceStatus()
        {


            var client = RedisManager.GetClient();

            if (client == null)
            {
                return;
            }

            try
            {

                int nowTimestamp = Convert.ToInt32((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);

                if ((nowTimestamp - _deviceStatus.UpdateTimestamp) > _deviceConfig.UpdateTimeout)
                {
                    _deviceStatus.ConnectStatus = CONNECT_STATUS.OFF_LINE;

                    LoggerManager.Log.Info($"设备{_deviceRedisHashName}处于离线状态！\n");
                    
                }
                else
                {
                    _deviceStatus.ConnectStatus = CONNECT_STATUS.ON_LINE;

                   // _deviceStatus.RunStatus = (DEVICE_RUN_STATUS)int.Parse(client.HGet(_deviceRedisHashName, _deviceConfig.RunStatusTagName));
                    //_deviceStatus.RuntimeCount = uint.Parse(client.HGet(_deviceRedisHashName, _deviceConfig.RuntimeTagName));
                    //_deviceStatus.Temperature = int.Parse(client.HGet(_deviceRedisHashName, _deviceConfig.Temperature));
                }


                //Update deivce status to redis;

                string DeviceStatusJsonString = JsonConvert.SerializeObject(_deviceStatus);
                client.HSet("DeviceStatusTable", _deviceRedisHashName, DeviceStatusJsonString);






             }
            catch(Exception ex)
            {
                LoggerManager.Log.Error($"更新设备{_deviceRedisHashName}状态信息出错！\n");

            }

            return;




        }

        public bool UpdateDeviceConfigToRedis()
        {
            var client = RedisManager.GetClient();

            if (client == null)
            {
                return false;
            }

            client.Select(_parentCompanyInfo.Redis.DB);

            string deviceDataStoreConfigJson = JsonConvert.SerializeObject(_DataStoreMap);
            client.HSet(companyRedisTableName, $"[DataStoreConfig]-[{_deviceInfo.DeviceCode}]", deviceDataStoreConfigJson);            

            return true;
        }

        public int WriteDeviceAlarmListToDB(AlarmListInfo deivceAlarmList)
        {
            _deviceAlarmStoreManager.WriteAlarmListToDB(deivceAlarmList);
            return 0;
        }

        public List<AlarmInfo> QueryHistoryAlarm(AlarmQueryParam queryParam)
        {
            return _deviceAlarmStoreManager.QueryHistoryAlarm(queryParam);

        }


        public void StartDeviceDataStoreService()
        {
            _deviceDataStoreManager.StartDataStoreTask();

        }

        public void StoptDeviceDataStoreService()
        {
            _deviceDataStoreManager.StartDataStoreTask();

        }


    }
}
