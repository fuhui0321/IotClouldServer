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
    public class RealtimeAlarmHelper
    {

        private string _alarmTableName;
        private static string Conn = null;


        public bool InitRealtimeAlarmHelper(string companyCode,string CompanyCnnString)
        {

            _alarmTableName = $"Realtime-Alarm-[{companyCode}]";

            Conn = CompanyCnnString;
            string createAlarmTableSql = $"CREATE TABLE IF NOT EXISTS `{_alarmTableName}`" +
                "( `Id` INTEGER PRIMARY KEY AUTO_INCREMENT ,`DeviceCode` varchar(255)" +
                ",`AlarmDate` datetime, `AlarmName` varchar(255)" +
                ",`AlarmLevel` int(11),`AlarmType` int(11) ,`AlarmSpanTime` int(11) ,`DeviceType` varchar(255) , `DeviceName` varchar(255)" +
                ",`AlarmCondition` varchar(1000), `AlarmHelp` varchar(255) " +
                ",`Reserved1` varchar(255),`Reserved2` varchar(255), `Reserved3` varchar(255), `Reserved4` varchar(255), `Reserved5` varchar(255) ) ENGINE=MEMORY";

            try
            {

                int res = Common.Helper.MySqlHelper.ExecuteNonQuery(Conn, CommandType.Text, createAlarmTableSql, null);
            }
            catch(Exception ex)
            {
                LoggerManager.Log.Info($"[{_alarmTableName}]MySql实时故障内存表创建失败,ErrInfo={ex.ToString()}");
            }

            return true;


        }

        public bool DeleteRealtimeAlarm(List<string>alarmNameList,List<string>deviceCodeList)
        {

           

            


            string strAlarmNameList = "" ;
            string strDeviceCodeList = "";


            for (int i = 0; i < alarmNameList.Count(); i++)
            {

                strAlarmNameList = strAlarmNameList + $"'{alarmNameList[i] }',";                
            }

            strAlarmNameList = strAlarmNameList.Remove(strAlarmNameList.Length - 1, 1);

            for (int i = 0; i < deviceCodeList.Count(); i++)
            {

                strDeviceCodeList = strDeviceCodeList + $"'{deviceCodeList[i] }',";
            }

            strDeviceCodeList = strDeviceCodeList.Remove(strDeviceCodeList.Length - 1, 1);


            string deleteSQL = $" DELETE FROM `{_alarmTableName}` WHERE AlarmName IN ({strAlarmNameList}) AND DeviceCode IN ({strDeviceCodeList})";

            try
            {
                Common.Helper.MySqlHelper.ExecuteNonQuery(Conn, CommandType.Text, deleteSQL, null);
            }
            catch (Exception ex)
            {
                LoggerManager.Log.Error($"{_alarmTableName}:向数据库写删除实时故障数据失败！,SQL = {deleteSQL}");
            }


            return true;
        }


        public bool InsertRealtimeAlarm(Dictionary<string, AlarmInfoEx> addRtAlarm)
        {
            string insertSQL = $"Insert into `{_alarmTableName}` values ";

            foreach (KeyValuePair<string, AlarmInfoEx> item in addRtAlarm)
            {

                string alarmSql;

                alarmSql = $"(null,'{item.Value.DeviceCode}','{item.Value.AlarmInfoObject.AlarmDate}',";
                alarmSql += $"'{item.Value.AlarmInfoObject.AlarmName}',{item.Value.AlarmInfoObject.AlarmLevel},";
                alarmSql += $"{(int)item.Value.AlarmInfoObject.AlarmType},{item.Value.AlarmInfoObject.AlarmSpanTime},";
                alarmSql += $"'{item.Value.AlarmInfoObject.DeviceType}','{item.Value.AlarmInfoObject.DeviceName}',";
                alarmSql += $"'{item.Value.AlarmInfoObject.AlarmCondition}','{item.Value.AlarmInfoObject.AlarmHelp}',";
                alarmSql += $"'{item.Value.AlarmInfoObject.Reserved1}','{item.Value.AlarmInfoObject.Reserved2}',";
                alarmSql += $"'{item.Value.AlarmInfoObject.Reserved3}','{item.Value.AlarmInfoObject.Reserved4}',";
                alarmSql += $"'{item.Value.AlarmInfoObject.Reserved5}'),";

                insertSQL += alarmSql;

            }

            insertSQL = insertSQL.Remove(insertSQL.Length - 1, 1);

            try
            {
                Common.Helper.MySqlHelper.ExecuteNonQuery(Conn, CommandType.Text, insertSQL, null);
            }
            catch (Exception ex)
            {
                LoggerManager.Log.Error($"{_alarmTableName}:向数据库写增加实时故障数据失败！,SQL = {insertSQL}");
            }


            return true;
        }

    }
}
