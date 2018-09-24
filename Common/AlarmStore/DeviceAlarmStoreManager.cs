using IotCloudService.Common.Helper;
using IotCloudService.Common.Modes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.AlarmStore
{
    public class DeviceAlarmStoreManager
    {
        private DeviceInfoEx _parentDeviceInfo = null;
        private string _alarmTableName;
        private static string Conn = null;


        public DeviceAlarmStoreManager(DeviceInfoEx deviceInfo)
        {
            _parentDeviceInfo = deviceInfo;

            _alarmTableName = $"History-Alarm-[{_parentDeviceInfo.CompanyCode}-{_parentDeviceInfo.DeviceCode}]";
        }

        public bool InitDeviceAlarmStoreManager(string CompanyCnnString)
        {
            Conn = CompanyCnnString;
            string createAlarmTableSql = $"CREATE TABLE IF NOT EXISTS `{_alarmTableName}`" +
                "( `Id` INTEGER PRIMARY KEY AUTO_INCREMENT " +
                ",`AlarmDate` datetime,`RecoverDate` datetime, `AlarmName` varchar(255)" +
                ",`AlarmLevel` int(11),`AlarmType` int(11) ,`AlarmSpanTime` int(11) ,`DeviceType` varchar(255) , `DeviceName` varchar(255)" +
                ",`AlarmCondition` varchar(1000), `AlarmHelp` varchar(255) " +
                ",`Reserved1` varchar(255),`Reserved2` varchar(255), `Reserved3` varchar(255), `Reserved4` varchar(255), `Reserved5` varchar(255) )";


            Common.Helper.MySqlHelper.ExecuteNonQuery(Conn, CommandType.Text, createAlarmTableSql, null);
            

            return true;
        }

        public List<AlarmInfo> QueryHistoryAlarm(AlarmQueryParam queryParam)
        {
            List<AlarmInfo> queryAlarmList = new List<AlarmInfo>();

            string querySql = $"select * from `{_alarmTableName}` where AlarmDate between '{queryParam.StartDate}' and '{queryParam.EndDate}'";

            if (string.IsNullOrEmpty(queryParam.AlarmName) == false)
            {
                querySql += $" and AlarmName LIKE ‘%{queryParam.AlarmName}%'";
            }

            querySql += $" order by 'AlarmDate' desc";


            try
            {
                MySqlDataReader queryAlarmReader = Common.Helper.MySqlHelper.ExecuteReader(Conn, CommandType.Text, querySql, null);
                while (queryAlarmReader.Read())
                {
                    AlarmInfo tempAlarmInfo = new AlarmInfo();

                    tempAlarmInfo.AlarmDate = queryAlarmReader["AlarmDate"] == null ? "" : ((DateTime)queryAlarmReader["AlarmDate"]).ToString("yyyy-MM-dd hh:mm:ss");
                    tempAlarmInfo.RecoveryDate = queryAlarmReader["RecoverDate"] == null ? "" : ((DateTime)queryAlarmReader["RecoverDate"]).ToString("yyyy-MM-dd hh:mm:ss"); //(string)queryAlarmReader["RecoveryDate"];
                    tempAlarmInfo.AlarmName = queryAlarmReader["AlarmName"] == null ? "" :queryAlarmReader["AlarmName"].ToString();//(string)queryAlarmReader["AlarmName"];
                    tempAlarmInfo.AlarmLevel = queryAlarmReader["AlarmLevel"] == null ? 1 : (int)queryAlarmReader["AlarmLevel"];
                    tempAlarmInfo.AlarmType = queryAlarmReader["AlarmType"] == null ? ALARM_TYPE.ALARM_OCCUR : (ALARM_TYPE)(int)queryAlarmReader["AlarmType"];

                    long temp = int.Parse(queryAlarmReader["AlarmSpanTime"].ToString());

                    tempAlarmInfo.AlarmSpanTime = queryAlarmReader["AlarmSpanTime"] == null ? 0 : int.Parse(queryAlarmReader["AlarmSpanTime"].ToString());
                   
                    tempAlarmInfo.DeviceType = queryAlarmReader["DeviceType"] == null ? "" : queryAlarmReader["DeviceType"].ToString();
                    tempAlarmInfo.DeviceName = queryAlarmReader["DeviceName"] == null ? "" :queryAlarmReader["DeviceName"].ToString();
                    tempAlarmInfo.AlarmCondition = queryAlarmReader["AlarmCondition"] == null ? "" : queryAlarmReader["AlarmCondition"].ToString();
                    tempAlarmInfo.AlarmHelp = queryAlarmReader["AlarmHelp"] == null ? "" :queryAlarmReader["AlarmHelp"].ToString();
                    tempAlarmInfo.Reserved1 = queryAlarmReader["Reserved1"] == null ? "" : queryAlarmReader["Reserved1"].ToString();
                    tempAlarmInfo.Reserved2 = queryAlarmReader["Reserved2"] == null ? "" : queryAlarmReader["Reserved2"].ToString();
                    tempAlarmInfo.Reserved3 = queryAlarmReader["Reserved3"] == null ? "" :queryAlarmReader["Reserved3"].ToString();
                    tempAlarmInfo.Reserved4 = queryAlarmReader["Reserved4"] == null ? "" :queryAlarmReader["Reserved4"].ToString();
                    tempAlarmInfo.Reserved5 = queryAlarmReader["Reserved5"] == null ? "" :queryAlarmReader["Reserved5"].ToString();

                    queryAlarmList.Add(tempAlarmInfo);
                }
            }
            catch (Exception ex)
            {
                LoggerManager.Log.Error($"{_parentDeviceInfo.CompanyCode}-{_parentDeviceInfo.DeviceCode}:故障查询失败！,SQL = {querySql}");
            }




            return queryAlarmList;
        }

        public async Task WriteAlarmListToDB(AlarmListInfo alarmListInfo)
        {


            await Task.Run(() =>
            {
                string insertSQL = $"Insert into `{_alarmTableName}` values ";

                for (int i = 0; i < alarmListInfo.AlarmList.Count(); i++)
                {
                    AlarmInfo temiAlarmItem = alarmListInfo.AlarmList[i];
                    string alarmSql;

                    alarmSql = $"(null,'{temiAlarmItem.AlarmDate}','{temiAlarmItem.RecoveryDate}',";
                    alarmSql += $"'{temiAlarmItem.AlarmName}',{temiAlarmItem.AlarmLevel},";
                    alarmSql += $"{(int)temiAlarmItem.AlarmType},{temiAlarmItem.AlarmSpanTime},";
                    alarmSql += $"'{temiAlarmItem.DeviceType}','{temiAlarmItem.DeviceName}',";
                    alarmSql += $"'{temiAlarmItem.AlarmCondition}','{temiAlarmItem.AlarmHelp}',";
                    alarmSql += $"'{temiAlarmItem.Reserved1}','{temiAlarmItem.Reserved2}',";
                    alarmSql += $"'{temiAlarmItem.Reserved3}','{temiAlarmItem.Reserved4}',";
                    alarmSql += $"'{temiAlarmItem.Reserved5}'),";

                    insertSQL += alarmSql;
                }

                insertSQL = insertSQL.Remove(insertSQL.Length - 1, 1);

                try
                {
                    Common.Helper.MySqlHelper.ExecuteNonQuery(Conn, CommandType.Text, insertSQL, null);
                }
                catch (Exception ex)
                {
                    LoggerManager.Log.Error($"{_parentDeviceInfo.CompanyCode}-{_parentDeviceInfo.DeviceCode}:向数据库写故障数据失败！,SQL = {insertSQL}");
                }
            });
            
            
            

        }




    }
}
