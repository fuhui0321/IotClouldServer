using IotCloudService.Common.AlarmStore;
using IotCloudService.Common.Modes;
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
    public class DeviceManager
    {        
        private static string Conn = ConfigurationManager.AppSettings["MySqlConnectString"];
        Dictionary<string, DeviceHelper> _mapDevices = new Dictionary<string, DeviceHelper>();
        CompanyInfoEx _companyInfo;


        public void InitializeDeviceManager(CompanyInfoEx companyInfo)
        {
            _companyInfo = companyInfo;

            MySqlDataReader companyReader = MySqlHelper.ExecuteReader(Conn, CommandType.Text, $"select * from deviceconfig where CompanyCode='{companyInfo.CompanyCode}'", null);

            while (companyReader.Read())
            {
                DeviceInfoEx deviceItem = new DeviceInfoEx();

                deviceItem.CompanyCode = (string)companyReader["CompanyCode"];
                deviceItem.DeviceCode = (string)companyReader["DeviceCode"];
                deviceItem.GPS = (string)companyReader["DeviceGPS"];

                deviceItem.DeviceName = (string)companyReader["DeviceName"];
                deviceItem.DeviceType = (int)companyReader["DeviceType"];
                


                if (companyReader["DeviceTypeName"].Equals(DBNull.Value) == true)
                {
                    deviceItem.DeviceTypeName = "";
                }
                else
                {
                    deviceItem.DeviceTypeName = (string)companyReader["DeviceTypeName"];
                }


                if (companyReader["Address"].Equals(DBNull.Value) == true)
                {
                    deviceItem.Address = "";
                }
                else
                {
                    deviceItem.Address = (string)companyReader["Address"];
                }

               



                DeviceHelper tempCompanyItem = new DeviceHelper(deviceItem, _companyInfo);

                tempCompanyItem.InitializeDataStoreManager();
                _mapDevices.Add(deviceItem.DeviceCode, tempCompanyItem);
            }

        }



        public int InitializeAlarmStore(string CompanyCnnString)
        {
            foreach (var item in _mapDevices)
            {
                ((DeviceHelper)item.Value).InitializeAlarmStoreManager(CompanyCnnString);

            }
            return 0;
        }

        
        
        public bool CheckDevice(DeviceBase objDevice)
        {
            if (IsExist(objDevice) == false)
            {
                return false;
            }

            DeviceHelper tempDevice = _mapDevices[objDevice.DeviceCode];

            tempDevice.UpdateDeviceDataUploadTime();




            return true;
        }

        public DeviceStatusBase GetDeviceStatus(string DeviceCode)
        {
            DeviceStatusBase deviceStatus = null;

            deviceStatus = (_mapDevices[DeviceCode]).GetDeviceStatus();

            return deviceStatus;
            
        }

        public List<DataStoreTableInfo> GetDeviceDataStoreConfig(string deviceCode)
        {
            if (_mapDevices.ContainsKey(deviceCode))
            {
                return _mapDevices[deviceCode].GetDeviceDataStoreConfig();
            }

            return null;
        }

        public DataStoreTableInfo GetDeviceDataStoreConfigItem(string deviceCode,string tableName)
        {
            if (_mapDevices.ContainsKey(deviceCode))
            {
                return _mapDevices[deviceCode].GetDeviceDataStoreConfigItem(tableName);
            }

            return null;

        }

        public bool UpdateAllDeviceStatus()
        {
            foreach (var item in _mapDevices)
            {
                ((DeviceHelper)item.Value).UpdateDeviceStatus();              

            }
            return true;
        }

        private bool IsExist(DeviceBase objDevice)
        {

            return _mapDevices.ContainsKey(objDevice.DeviceCode);
        }

        public bool UpdateAllDeviceConfigToRedis()
        {

            foreach (var item in _mapDevices)
            {
                ((DeviceHelper)item.Value).UpdateDeviceConfigToRedis();

            }

            return true;
        }

        public int WriteDeviceAlarmListToDB(AlarmListInfo deivceAlarmList)
        {
            if (_mapDevices.ContainsKey(deivceAlarmList.DeviceInfo.DeviceCode) == true)
            {
                DeviceHelper deviceItem = _mapDevices[deivceAlarmList.DeviceInfo.DeviceCode];

                if (deviceItem.WriteDeviceAlarmListToDB(deivceAlarmList) !=0)
                {
                    LoggerManager.Log.Error($"公司[{deivceAlarmList.DeviceInfo.CompanyCode}]-设备[{deivceAlarmList.DeviceInfo.DeviceCode}],写故障数据失败！");

                }                
            }
            else
            {
                LoggerManager.Log.Error($"没有查询到公司[{deivceAlarmList.DeviceInfo.CompanyCode}]的设备[{deivceAlarmList.DeviceInfo.DeviceCode}]！");
            }


            return 0;
        }

        private DeviceHelper GetDeviceByCode(string deviceCode)
        {
            DeviceHelper deviceObject = null;

            if (_mapDevices.ContainsKey(deviceCode)) // True 
            {
                deviceObject = _mapDevices[deviceCode];
            }

            return deviceObject;

        }

        public List<AlarmInfo> QueryHistoryAlarm(AlarmQueryParam queryParam)
        {

            DeviceHelper deviceObject = GetDeviceByCode(queryParam.DeviceCode);

            if (deviceObject == null)
            {
                LoggerManager.Log.Error($"没有需要查询历史故障的设备[{queryParam.CompanyCode}-{queryParam.DeviceCode}]");
                return null;
            }

            return deviceObject.QueryHistoryAlarm(queryParam);

        }

        public void StartDataStoreService()
        {
            foreach (var item in _mapDevices)
            {
                ((DeviceHelper)item.Value).StartDeviceDataStoreService();

            }

        }

        public void StopDataStoreService()
        {
            foreach (var item in _mapDevices)
            {
                ((DeviceHelper)item.Value).StoptDeviceDataStoreService();

            }

        }


    }
}
