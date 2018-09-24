using IotCloudService.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IotCloudService.Common.Helper;
using CSRedis;
using IotCloudService.Common.Redis;
using System.Threading;
using IotCloudService.IotDataStoreService.Mode;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;

namespace IotCloudService.IotDataStoreService.AlarmStore
{
    public class AlarmStoreManager
    {

        private static string Conn = ConfigurationManager.AppSettings["MySqlConnectString"];

        private static Dictionary<string, CompanyHelper> _companyMap = null;
        private int RedisDB;
        private RedisClient _redisClient;
        private bool _ThreadExitFalg = false;
        private Thread _threadHandle;
        private string _companyAlarmListName;
        private CompanyHelper _companyObject = null;
        public bool Initialize()
        {

            _companyMap = CompanyManagerHelper.GetCompanyMap();

            _redisClient = RedisManager.GetClient();
            _ThreadExitFalg = true;
            _companyAlarmListName = $"[AlarmList]-{_companyObject.GetCompanyInfo().CompanyCode}"; 


            return true;
        }

        private void initDeviceAlarmDBTable()
        {
            foreach (var item in _companyMap)
            {
                CompanyHelper companyItem =item.Value;            

                ((CompanyHelper)item.Value).UpdateAllDeviceStatus();
            }
        }


        private void StartAlarmStoreTask()
        {

            _threadHandle  = new Thread(AlarmStoreHandler);
            _threadHandle.Start();

        }

        private void StopAlarmStoreTask()
        {
            _ThreadExitFalg = false;
            _threadHandle.Join();
        }

        private void AlarmStoreHandler()
        {
            while (_ThreadExitFalg == true)
            {
                long alarmCount = _redisClient.LLen(_companyAlarmListName);

                if (alarmCount > 0)
                {
                    string[] tempAlarmList = _redisClient.LRange(_companyAlarmListName, 0, alarmCount - 1);
                    _redisClient.LRem(_companyAlarmListName, alarmCount, tempAlarmList);

                }
                else
                {
                    Thread.Sleep(10000);
                }

                


            }


        }


        private void AlarmListHandler(string[] AlarmList)
        {
            for (int i =0;i < AlarmList.Count(); i++)            {
                AlarmListInfo companyAlarm = JsonConvert.DeserializeObject<AlarmListInfo>(AlarmList[i]);
                AnalzeAlarmList(companyAlarm);

            }

        }


        private void AnalzeAlarmList(AlarmListInfo companyAlarmList)
        {
            AlarmListInfo toDbAlarmListInfo = new AlarmListInfo();
            toDbAlarmListInfo.DeviceInfo = companyAlarmList.DeviceInfo;

            for (int i =0;i < companyAlarmList.AlarmList.Count();i++)
            {
                AlarmInfo alarmItem = companyAlarmList.AlarmList[i];
                string deviceAlarmKey = $"RT-ALARM-[{companyAlarmList.DeviceInfo.CompanyCode}-{companyAlarmList.DeviceInfo.DeviceCode}]";

                string alarmMessage = _redisClient.HGet(deviceAlarmKey, alarmItem.AlarmName);

                if (alarmMessage == null)
                {
                    if (alarmItem.AlarmType == ALARM_TYPE.ALARM_OCCUR)
                    {
                        string alarmValue = JsonConvert.SerializeObject(alarmItem);
                        _redisClient.HSet(deviceAlarmKey, alarmItem.AlarmName, alarmValue);
                    }
                    else
                    {
                        alarmItem.RecoveryDate = alarmItem.AlarmDate;
                        toDbAlarmListInfo.AlarmList.Add(alarmItem);                       
                    }
                }
                else
                {
                    if (alarmItem.AlarmType == ALARM_TYPE.ALARM_OCCUR)
                    {

                        AlarmInfo occuralarmItem = JsonConvert.DeserializeObject<AlarmInfo>(alarmMessage);
                        toDbAlarmListInfo.AlarmList.Add(alarmItem);

                        string alarmValue = JsonConvert.SerializeObject(alarmItem);
                        _redisClient.HSet(deviceAlarmKey, alarmItem.AlarmName, alarmValue);
                    }
                    else
                    {                     
                        AlarmInfo occuralarmItem = JsonConvert.DeserializeObject<AlarmInfo>(alarmMessage);
                        occuralarmItem.RecoveryDate = alarmItem.AlarmDate;
                        toDbAlarmListInfo.AlarmList.Add(occuralarmItem);
                        _redisClient.HDel(deviceAlarmKey, alarmItem.AlarmName);


                    }

                }
                
            }

            WriteAlarmListToDB(toDbAlarmListInfo);

        }


        



       

        
    }
}
