using IotCloudService.Common.AlarmStore;
using IotCloudService.Common.Interface;
using IotCloudService.Common.Modes;
using IotCloudService.Common.Redis;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Helper
{
    public class CompanyHelper
    {
        private DeviceManager _deviceManager = new DeviceManager();       
        private CompanyInfoEx _companyInfo ;
        private static string Conn = ConfigurationManager.AppSettings["MySqlConnectString"];
        string companyRedisTableName;
        private AlarmStoreManager _alarmStoreManager = new AlarmStoreManager();

        private string CompanyCnn  ;
        private string CompanyDatabaseName ;

       


        public CompanyHelper(CompanyInfoEx companyInfo)
        {

            _companyInfo = companyInfo;

            companyRedisTableName = $"[IotConfig]-[{_companyInfo.CompanyCode}]";
                        
            CompanyDatabaseName = $"[iot]-[{_companyInfo.CompanyCode}]";


            CompanyCnn = $"Database='{CompanyDatabaseName}';Data Source='{_companyInfo.Database.host}';";
            CompanyCnn += $"User Id='{_companyInfo.Database.userName}';Password='{_companyInfo.Database.password}';charset='utf8';pooling=true";

            InitializeCompanyHelper();

            InitializeAlarmStoreManager();

        }


        public string GetCnnString()
        {
            return Conn;
        }

        public int InitializeAlarmStoreManager()
        {

            _alarmStoreManager.Initialize(this);


            return 0;
        }
        


        public String GetRedisTableName()
        {
            return companyRedisTableName;
        }

        public String GetCompanyDatabaseName()
        {
            return CompanyCnn;
        }        

        private void InitializeCompanyHelper()
        {
            _deviceManager.InitializeDeviceManager(_companyInfo);

            _deviceManager.InitializeAlarmStore(CompanyCnn);

            InitializeCompanyDatabase();

        }

        private int InitializeCompanyDatabase()
        {
            int nRes = 0;
            string createCompanyDBSql = $"CREATE DATABASE IF NOT EXISTS `{CompanyDatabaseName}`";

            nRes = MySqlHelper.ExecuteNonQuery(Conn, CommandType.Text, createCompanyDBSql, null);

            return 0;
        }


        public bool CheckLoginUser(LoginUserInfo loginUser)
        {

            return true;//UserManagerHelper.CheckUser(loginUser);
        }

        public DeviceStatusBase[] GetDeviceStat(string[] deviceCodeList)
        {
            //string[] deviceCodeList = UserManager.GetUserObject(loginUser.UserName).DeviceList;
            DeviceStatusBase[] deviceStatusList = new DeviceStatusBase[deviceCodeList.Length];

            for (int i=0; i < deviceStatusList.Length; i++)
            {
                deviceStatusList[i] = _deviceManager.GetDeviceStatus(deviceCodeList[i]);   
            }

            return deviceStatusList;

        }

        public List<DataStoreTableInfo> GetDeviceDataStoreConfig(string deviceCode)
        {
            return _deviceManager.GetDeviceDataStoreConfig(deviceCode);
        }

        public DataStoreTableInfo GetDeviceDataStoreConfigItem(string deviceCode,string tableName)
        {
            return _deviceManager.GetDeviceDataStoreConfigItem(deviceCode, tableName);
        }


        public bool CheckDeviceCode(DeviceBase deviceCheck)
        {
            return _deviceManager.CheckDevice(deviceCheck);
            
        }

        public bool UpdateAllDeviceStatus()
        {
            _deviceManager.UpdateAllDeviceStatus();
            return true;
        }


        public bool UpdateCompanyConfigToRedis()
        {
            var client = RedisManager.GetClient();
            
            if (client == null)
            {
                return false;
            }

            client.Select(_companyInfo.Redis.DB);             
            client.HSet(companyRedisTableName, "CompanyCode", _companyInfo.CompanyCode);
            client.HSet(companyRedisTableName, "CompanyName", _companyInfo.CompanyName);
            client.HSet(companyRedisTableName, "DeviceCount", _companyInfo.DeviceCount);


            _deviceManager.UpdateAllDeviceConfigToRedis();

            return true;
        }

        public CompanyInfoEx GetCompanyInfo()
        {
            return _companyInfo;
        }

        public void StartAlarmStoreService()
        {
            _alarmStoreManager.StartAlarmStoreTask();
        }

        public void StopAlarmStoreService()
        {
            _alarmStoreManager.StopAlarmStoreTask();
        }

        public void StartDataStoreService()
        {
            _deviceManager.StartDataStoreService();
        }

        public void StopDataStoreService()
        {
            _deviceManager.StopDataStoreService();
        }



        public int WriteCompanyAlarmListToDB(AlarmListInfo deivceAlarmList)
        {
            _deviceManager.WriteDeviceAlarmListToDB(deivceAlarmList);
            return 0;
        }


        public List<AlarmInfo> QueryHistoryAlarm(AlarmQueryParam queryParam)
        {
            return _deviceManager.QueryHistoryAlarm(queryParam);

        }

        public List<AlarmInfo> QueryRealtimeAlarm(AlarmQueryParam queryParam)
        {
            return _alarmStoreManager.GetDeviceRTAlarmInfo(queryParam.CompanyCode,queryParam.DeviceCode);            
        }

        public List<AlarmInfoEx> QueryRealtimeAlarmEx(RealtimeAlarmQueryParam queryParam)
        {
            return _alarmStoreManager.GetDeviceRTAlarmInfoEx(queryParam);
        }





    }
}
