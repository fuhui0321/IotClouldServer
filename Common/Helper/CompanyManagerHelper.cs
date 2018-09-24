
using IotCloudService.Common.AlarmStore;
using IotCloudService.Common.Interface;
using IotCloudService.Common.Modes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IotCloudService.Common.Helper
{
    public class CompanyManagerHelper
    {

        private static string Conn = ConfigurationManager.AppSettings["MySqlConnectString"];        
        private static Dictionary<string, CompanyHelper> _companyMap = new Dictionary<string, CompanyHelper>();

       

        private static bool _ThreadExitFalg = true;


        ~CompanyManagerHelper()
        {

            _ThreadExitFalg = false;
        }

        public static bool Initialize()
        {
            InitializeCompanyManager();
            UpdateAllCompanyConfigToRedis();




            return true;
        }

        public static Dictionary<string, CompanyHelper> GetCompanyMap()
        {
            return _companyMap;
        }

        //public static int AddCompanyTask(ICompanyTask NewCompanyTask)
        //{

        //    return 0;
        //}

        //public static void StartCompanyTaskList()
        //{
        //    foreach (var item in _companyMap)
        //    {
        //        ((CompanyHelper)item.Value).StartCompanyTaskList();
        //    }

        //}

        //public static void StopCompanyTaskList()
        //{
        //    foreach (var item in _companyMap)
        //    {
        //        ((CompanyHelper)item.Value).StopCompanyTaskList();
        //    }

        //}


        public static void StartDeviceStatusThread()
        {
            Thread t = new Thread(DeviceStatusHandler);
            t.Start();

        }

        public static void StartAlarmStoreService()
        {
            foreach (var item in _companyMap)
            {
                ((CompanyHelper)item.Value).StartAlarmStoreService();
            }

        }

        public static void StopAlarmStoreService()
        {

            foreach (var item in _companyMap)
            {
                ((CompanyHelper)item.Value).StopAlarmStoreService();
            }

        }

        public static void StartDataStoreService()
        {
            foreach (var item in _companyMap)
            {
                ((CompanyHelper)item.Value).StartDataStoreService();
            }

        }

        public static void StopDataStoreService()
        {

            foreach (var item in _companyMap)
            {
                ((CompanyHelper)item.Value).StopDataStoreService();
            }

        }

        public static bool Uninitialize()
        {
            return true;
        }

        public static bool CheckLoginUser(LoginUserInfo loginUser)
        {
            //CompanyHelper companyObject = GetCompanyByCode(loginUser.CompanyCode);

            return true;
                                            
        }

        

        public static DeviceStatusBase[] GetDeviceStatusList(string companyCode,string userPhone)
        {
            CompanyHelper tempCompany = GetCompanyByCode(companyCode);

            if (tempCompany == null)
                return null;

            

            string[] tempDeviceNames = UserManagerHelper.GetDeviceList(userPhone);



            return tempCompany.GetDeviceStat(tempDeviceNames);            

        }

        public static List<DataStoreTableInfo> GetDeviceDataStoreConfig(string companyCode, string deviceCode)
        {
            CompanyHelper tempCompany = GetCompanyByCode(companyCode);

            if (tempCompany == null)
            {
                return null;
            }

            return tempCompany.GetDeviceDataStoreConfig(deviceCode);


        }

        public static DataStoreTableInfo GetDeviceDataStoreConfigItem(string companyCode, string deviceCode,string tableName)
        {
            CompanyHelper tempCompany = GetCompanyByCode(companyCode);

            if (tempCompany == null)
            {
                return null;
            }

            return tempCompany.GetDeviceDataStoreConfigItem(deviceCode,tableName);

        }

        public static bool CheckDeviceCode(DeviceBase deviceCheck)
        {
            CompanyHelper tempCompany = GetCompanyByCode(deviceCheck.CompanyCode);

            if (tempCompany == null)
            {
                LoggerManager.Log.Info($"系统中不存在[{deviceCheck.CompanyCode}]的公司代码！\n");
                return false;
            }

            if (tempCompany.CheckDeviceCode(deviceCheck) == false)
            {
                LoggerManager.Log.Info($"系统中不存在[{deviceCheck.DeviceCode}]的设备代码！\n");
                return false;
            }

           

            return true;
        }

        private static bool InitializeCompanyManager()
        {
            MySqlDataReader companyReader = MySqlHelper.ExecuteReader(Conn, CommandType.Text, "select * from companyinfo", null);

            while (companyReader.Read())
            {
                CompanyInfoEx companyItem = new CompanyInfoEx();
                companyItem.Redis = new RedisInfo();

                companyItem.CompanyCode = (string)companyReader["CompanyCode"];
                companyItem.CompanyName = (string)companyReader["CompanyName"];
                companyItem.DeviceCount = (int)companyReader["DeviceCount"];

                companyItem.Redis.Host = (string)companyReader["Redis_IP"];
                companyItem.Redis.Port = (int)companyReader["Redis_Port"];
                companyItem.Redis.Passowrd = (string)companyReader["Redis_Password"];
                companyItem.Redis.DB = (int)companyReader["Redis_DB"];

                companyItem.Database = new DBInfo();
                companyItem.Database.host = (string)companyReader["DB_IP"];

                
                companyItem.Database.port = (int)companyReader["DB_Port"];
                companyItem.Database.userName = (string)companyReader["DB_UserName"];
                companyItem.Database.password = (string)companyReader["DB_UserPassword"];                
                CompanyHelper tempCompanyItem = new CompanyHelper(companyItem);

                

                _companyMap.Add(companyItem.CompanyCode, tempCompanyItem);
            }


            return true;
        }

        private static bool UpdateAllCompanyConfigToRedis()
        {
            foreach (var item in _companyMap)
            {
                ((CompanyHelper)item.Value).UpdateCompanyConfigToRedis();
            }

            return true;
        }

        private static CompanyHelper GetCompanyByCode(string CompanyCode)
        {
            CompanyHelper companyObject = null;

            if (_companyMap.ContainsKey(CompanyCode)) // True 
            {
                companyObject = _companyMap[CompanyCode];
            }           

            return companyObject;
        }

        private static void  DeviceStatusHandler()
        {

            while(_ThreadExitFalg == true)
            {
                foreach (var item in _companyMap)
                {
                    ((CompanyHelper)item.Value).UpdateAllDeviceStatus();
                }

                Thread.Sleep(1000);
                

            }
            
            
        }

        public static List<AlarmInfo> QueryHistoryAlarm(AlarmQueryParam QueryParams)
        {
            CompanyHelper companyObject = GetCompanyByCode(QueryParams.CompanyCode);
            if (companyObject == null)
            {
                LoggerManager.Log.Error($"没有需要查询历史故障的公司[{QueryParams.CompanyCode}]");
                return null;
            }
            
            return companyObject.QueryHistoryAlarm(QueryParams);

        }

        public static List<AlarmInfo> QueryRealtimeAlarm(AlarmQueryParam QueryParams)
        {
            CompanyHelper companyObject = GetCompanyByCode(QueryParams.CompanyCode);
            if (companyObject == null)
            {
                LoggerManager.Log.Error($"没有需要查询实时故障的公司[{QueryParams.CompanyCode}]");
                return null;
            }

            return companyObject.QueryRealtimeAlarm(QueryParams);
        }


        public static List<AlarmInfoEx> QueryRealtimeAlarmEx(RealtimeAlarmQueryParam QueryParams)
        {

            CompanyHelper companyObject = GetCompanyByCode(QueryParams.companyCode);
            if (companyObject == null)
            {
                LoggerManager.Log.Error($"没有需要查询实时故障的公司[{QueryParams.companyCode}]");
                return null;
            }

            return companyObject.QueryRealtimeAlarmEx(QueryParams);

        }





    }
}
