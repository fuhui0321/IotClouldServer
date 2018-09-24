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

using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;
using IotCloudService.Common.Modes;

namespace IotCloudService.Common.AlarmStore
{
    public class AlarmStoreManager
    {
        
        private CompanyHelper _parentCompanyHelper = null;
        private int RedisDB;
        private RedisClient _redisClient;
        private bool _ThreadExitFalg = false;
        private Thread _threadHandle;
        private string _companyAlarmListName;
        private RealtimeAlarmHelper _realtimeAlarmHelper = new RealtimeAlarmHelper();

        public bool Initialize(CompanyHelper ParentCompanyHelper)
        {
           
            _parentCompanyHelper = ParentCompanyHelper;

            RedisDB = _parentCompanyHelper.GetCompanyInfo().Redis.DB;
            _redisClient = RedisManager.GetClient();
            _redisClient.Select(RedisDB);
            _ThreadExitFalg = true;
            _companyAlarmListName = $"[AlarmList]-[{_parentCompanyHelper.GetCompanyInfo().CompanyCode}]";

            _realtimeAlarmHelper.InitRealtimeAlarmHelper(ParentCompanyHelper.GetCompanyInfo().CompanyCode,
                ParentCompanyHelper.GetCompanyDatabaseName());


            return true;
        }

        

        public void StartAlarmStoreTask()
        {
            _ThreadExitFalg = false;
            _threadHandle  = new Thread(AlarmStoreHandler);
            _threadHandle.Start();

        }

        public  void StopAlarmStoreTask()
        {
            _ThreadExitFalg = true;
            _threadHandle.Join();
        }

        private void AlarmStoreHandler()
        {
            while (_ThreadExitFalg == false)
            {
                long alarmCount = _redisClient.LLen(_companyAlarmListName);

                if (alarmCount > 0)
                {
                    string[] tempAlarmList = _redisClient.LRange(_companyAlarmListName, 0, alarmCount - 1);
                    tempAlarmList = _redisClient.LRange(_companyAlarmListName, 0, alarmCount-1);
                    AlarmListHandler(tempAlarmList);
                    _redisClient.LTrim(_companyAlarmListName, alarmCount,  -1);

                    Thread.Sleep(100);
                }
                else
                {
                    Thread.Sleep(1000);
                }

                


            }


        }


        private void AlarmListHandler(string[] AlarmList)
        {

            //更新设备的Alarm状态
            Dictionary<string, DeviceBase> updateAlarmDeviceInfo = new Dictionary<string, DeviceBase>();

            //////////////////////////////////////////////////////////
            //add by fuhui 2018-08-10
            //声明MySql 实时数据库内存表的更新条件
            Dictionary<string, AlarmInfoEx> occurAlarmList = new Dictionary<string, AlarmInfoEx>();
            List<string> recoveryAlarmNameList = new List<string>();
            List<string> recoveryDeviceCodeList = new List<string>();
            // end by fuhui
            ///////////////////////////////////////////////////////////////////////////////////

            for (int i =0;i < AlarmList.Count(); i++)            {
                AlarmListInfo companyAlarm = JsonConvert.DeserializeObject<AlarmListInfo>(AlarmList[i]);
                AnalzeAlarmList(companyAlarm);

                string tempUpdateAlarmKey = $"{ companyAlarm.DeviceInfo.CompanyCode}-{ companyAlarm.DeviceInfo.DeviceCode}";

                if (!updateAlarmDeviceInfo.ContainsKey(tempUpdateAlarmKey))
                {

                    updateAlarmDeviceInfo.Add(tempUpdateAlarmKey, companyAlarm.DeviceInfo);

                }

                //////////////////////////////////////////////////////////
                //add by fuhui 2018-08-10
                //增加内存表的更新条件
                string tempAlarmDeviceCode = companyAlarm.DeviceInfo.DeviceCode;
                for (int j = 0; j < companyAlarm.AlarmList.Count(); j++)
                {
                    string tempAlarmKey = $"{tempAlarmDeviceCode}-{companyAlarm.AlarmList[j].AlarmName}";
                    AlarmInfoEx tempAlarmObject = new AlarmInfoEx();
                    
                    tempAlarmObject.DeviceCode = tempAlarmDeviceCode;
                    tempAlarmObject.AlarmInfoObject = companyAlarm.AlarmList[j];



                    if (companyAlarm.AlarmList[j].AlarmType == ALARM_TYPE.ALARM_OCCUR)//增加实时故障信息
                    {
                        if (!occurAlarmList.ContainsKey(tempAlarmKey))
                        {

                            occurAlarmList.Add(tempAlarmKey, tempAlarmObject);

                        }
                        else
                        {
                            occurAlarmList[tempAlarmKey] = tempAlarmObject;
                        }

                    }
                    else if (companyAlarm.AlarmList[j].AlarmType == ALARM_TYPE.ALRAM_RECOVERY) //删除实时故障信息
                    {
                        if (!recoveryAlarmNameList.Contains(tempAlarmObject.AlarmInfoObject.AlarmName))
                        {

                            recoveryAlarmNameList.Add(tempAlarmObject.AlarmInfoObject.AlarmName);

                        }

                        if (!recoveryDeviceCodeList.Contains(tempAlarmObject.DeviceCode))
                        {

                            recoveryDeviceCodeList.Add(tempAlarmObject.DeviceCode);

                        }

                    }


                }
                //end by fuhui
                ///////////////////////////////////////////



            }

            if (AlarmList.Count() > 0)
            {

                var client = RedisManager.GetClient();

                //更新公司实时故障的状态
                string DevictRedisHashName = "DeviceStatusTable";
                var alarmUUID = Guid.NewGuid().ToString();
                client.HSet(DevictRedisHashName, $"[{_parentCompanyHelper.GetCompanyInfo().CompanyCode}]-AlarmUpdateCode", alarmUUID);

                foreach (KeyValuePair<string, DeviceBase> pair in updateAlarmDeviceInfo)
                {
                    
                    
                    //更新实时故障的状态
                    string DataRedisHashName = $"[{ pair.Value.CompanyCode}]-[{ pair.Value.DeviceCode}]"; 
                    alarmUUID = Guid.NewGuid().ToString();
                    client.HSet(DataRedisHashName, $"AlarmUpdateCode", alarmUUID);
                }




                //更新MySQL数据库中实时故障数据内存表

            }

           

            if (occurAlarmList.Count() > 0)
            {
                _realtimeAlarmHelper.InsertRealtimeAlarm(occurAlarmList);
            }


            if (recoveryDeviceCodeList.Count() > 0)
            {
                _realtimeAlarmHelper.DeleteRealtimeAlarm(recoveryAlarmNameList, recoveryDeviceCodeList);
            }

        }


        private void AnalzeAlarmList(AlarmListInfo companyAlarmList)
        {
            AlarmListInfo toDbAlarmListInfo = new AlarmListInfo();
            toDbAlarmListInfo.DeviceInfo = new DeviceBase();
            toDbAlarmListInfo.AlarmList = new List<AlarmInfo>();


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

                        alarmItem.AlarmSpanTime = CalculateAlarmSpanTime(companyAlarmList.DeviceInfo.CompanyCode,
                            companyAlarmList.DeviceInfo.DeviceCode, alarmItem);

                        toDbAlarmListInfo.AlarmList.Add(alarmItem);                       
                    }
                }
                else
                {
                    if (alarmItem.AlarmType == ALARM_TYPE.ALARM_OCCUR)
                    {

                        AlarmInfo occurAlarmItem = JsonConvert.DeserializeObject<AlarmInfo>(alarmMessage);
                        occurAlarmItem.RecoveryDate = alarmItem.AlarmDate;
                        occurAlarmItem.AlarmSpanTime = CalculateAlarmSpanTime(companyAlarmList.DeviceInfo.CompanyCode,
                            companyAlarmList.DeviceInfo.DeviceCode, occurAlarmItem);


                        toDbAlarmListInfo.AlarmList.Add(occurAlarmItem);

                        string alarmValue = JsonConvert.SerializeObject(alarmItem);
                        _redisClient.HSet(deviceAlarmKey, alarmItem.AlarmName, alarmValue);
                    }
                    else
                    {                     
                        AlarmInfo occurAlarmItem = JsonConvert.DeserializeObject<AlarmInfo>(alarmMessage);
                        occurAlarmItem.RecoveryDate = alarmItem.AlarmDate;
                        occurAlarmItem.AlarmType = ALARM_TYPE.ALRAM_RECOVERY;

                        occurAlarmItem.AlarmSpanTime = CalculateAlarmSpanTime(companyAlarmList.DeviceInfo.CompanyCode,
                           companyAlarmList.DeviceInfo.DeviceCode, occurAlarmItem);

                        toDbAlarmListInfo.AlarmList.Add(occurAlarmItem);
                        _redisClient.HDel(deviceAlarmKey, alarmItem.AlarmName);


                    }

                }
                
            }

            if (toDbAlarmListInfo.AlarmList.Count >0)
            {
                _parentCompanyHelper.WriteCompanyAlarmListToDB(toDbAlarmListInfo);
            }

            

            

        }

        public void UpdateCompanyDeviceAlarmUUID()
        {

        }

        public List<AlarmInfo> GetDeviceRTAlarmInfo(string companyCode, string deivceCode)
        {
            RedisClient _redisClient = RedisManager.GetClient();
            _redisClient.Select(RedisDB);
            List<AlarmInfo> rtAlarmList = new List<AlarmInfo>();

            string deviceAlarmKey = $"RT-ALARM-[{companyCode}-{deivceCode}]";

            
            Dictionary<string,string> alarmMsgList = _redisClient.HGetAll(deviceAlarmKey);

            foreach (var item in alarmMsgList)
            {
                AlarmInfo tempAlarmItem = null;
                string tempAlarmMessage = (string)item.Value;

                tempAlarmItem = JsonConvert.DeserializeObject<AlarmInfo>(tempAlarmMessage);

                rtAlarmList.Add(tempAlarmItem);
            }

            return rtAlarmList;
        }


        public List<AlarmInfoEx> GetDeviceRTAlarmInfoEx(RealtimeAlarmQueryParam queryParam)
        {
            List<AlarmInfoEx> rtAlarmList = new List<AlarmInfoEx>();

            string sqlRealtimeQuery = $"select * from `realtime-alarm-[{queryParam.companyCode}]` where ";

            if (queryParam.deviceCode != "")
            {
                sqlRealtimeQuery = sqlRealtimeQuery + $"DeviceCode = '{queryParam.deviceCode}' and ";
            }


            string tempAlarmLevelList = string.Join(",", queryParam.alarmLevel.ToArray());

            sqlRealtimeQuery = sqlRealtimeQuery + $"AlarmLevel in ({tempAlarmLevelList})";


            try
            {
                MySqlDataReader queryAlarmReader = Common.Helper.MySqlHelper.ExecuteReader(_parentCompanyHelper.GetCompanyDatabaseName(), CommandType.Text, sqlRealtimeQuery, null);

                while (queryAlarmReader.Read())
                {
                    AlarmInfoEx tempAlarmInfo = new AlarmInfoEx();

                    tempAlarmInfo.AlarmInfoObject = new AlarmInfo();

                    tempAlarmInfo.DeviceCode = queryAlarmReader["DeviceCode"] == null ? "" : queryAlarmReader["DeviceCode"].ToString();//(string)queryAlarmReader["AlarmName"];
                    tempAlarmInfo.AlarmInfoObject.AlarmDate = queryAlarmReader["AlarmDate"] == null ? "" : ((DateTime)queryAlarmReader["AlarmDate"]).ToString("yyyy-MM-dd hh:mm:ss");
                    //tempAlarmInfo.RecoveryDate = queryAlarmReader["RecoverDate"] == null ? "" : ((DateTime)queryAlarmReader["RecoverDate"]).ToString("yyyy-MM-dd hh:mm:ss"); //(string)queryAlarmReader["RecoveryDate"];
                    tempAlarmInfo.AlarmInfoObject.AlarmName = queryAlarmReader["AlarmName"] == null ? "" : queryAlarmReader["AlarmName"].ToString();//(string)queryAlarmReader["AlarmName"];
                    tempAlarmInfo.AlarmInfoObject.AlarmLevel = queryAlarmReader["AlarmLevel"] == null ? 1 : (int)queryAlarmReader["AlarmLevel"];
                    tempAlarmInfo.AlarmInfoObject.AlarmType = queryAlarmReader["AlarmType"] == null ? ALARM_TYPE.ALARM_OCCUR : (ALARM_TYPE)(int)queryAlarmReader["AlarmType"];

                    //long temp = int.Parse(queryAlarmReader["AlarmSpanTime"].ToString());

                    //tempAlarmInfo.AlarmInfoObject.AlarmSpanTime = queryAlarmReader["AlarmSpanTime"] == null ? 0 : int.Parse(queryAlarmReader["AlarmSpanTime"].ToString());

                    tempAlarmInfo.AlarmInfoObject.DeviceType = queryAlarmReader["DeviceType"] == null ? "" : queryAlarmReader["DeviceType"].ToString();
                    tempAlarmInfo.AlarmInfoObject.DeviceName = queryAlarmReader["DeviceName"] == null ? "" : queryAlarmReader["DeviceName"].ToString();
                    tempAlarmInfo.AlarmInfoObject.AlarmCondition = queryAlarmReader["AlarmCondition"] == null ? "" : queryAlarmReader["AlarmCondition"].ToString();
                    tempAlarmInfo.AlarmInfoObject.AlarmHelp = queryAlarmReader["AlarmHelp"] == null ? "" : queryAlarmReader["AlarmHelp"].ToString();
                    tempAlarmInfo.AlarmInfoObject.Reserved1 = queryAlarmReader["Reserved1"] == null ? "" : queryAlarmReader["Reserved1"].ToString();
                    tempAlarmInfo.AlarmInfoObject.Reserved2 = queryAlarmReader["Reserved2"] == null ? "" : queryAlarmReader["Reserved2"].ToString();
                    tempAlarmInfo.AlarmInfoObject.Reserved3 = queryAlarmReader["Reserved3"] == null ? "" : queryAlarmReader["Reserved3"].ToString();
                    tempAlarmInfo.AlarmInfoObject.Reserved4 = queryAlarmReader["Reserved4"] == null ? "" : queryAlarmReader["Reserved4"].ToString();
                    tempAlarmInfo.AlarmInfoObject.Reserved5 = queryAlarmReader["Reserved5"] == null ? "" : queryAlarmReader["Reserved5"].ToString();

                    rtAlarmList.Add(tempAlarmInfo);
                }
            }
            catch (Exception ex)
            {
                LoggerManager.Log.Error($"{queryParam.companyCode}:实时故障查询失败！,SQL = {sqlRealtimeQuery}");
            }














            return rtAlarmList;
        }


        private int CalculateAlarmSpanTime(string companyCode,string deivceCode,AlarmInfo AlarmItem)
        {
            int nRes = 0;
            try
            {

                TimeSpan ts1 = new TimeSpan(Convert.ToDateTime(AlarmItem.AlarmDate).Ticks);
                TimeSpan ts2 = new TimeSpan(Convert.ToDateTime(AlarmItem.RecoveryDate).Ticks);
                TimeSpan ts3 = ts1.Subtract(ts2).Duration();

                nRes = ts3.Seconds;
                 
            }
            catch(Exception ex )
            {
                LoggerManager.Log.Info($"[{companyCode}][{deivceCode}]故障时间计算错误,AlarmInfo={JsonConvert.SerializeObject(AlarmItem)}");
            }

            return nRes;

        }











    }
}
