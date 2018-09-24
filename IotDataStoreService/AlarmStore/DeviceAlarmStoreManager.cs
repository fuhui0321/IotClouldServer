using IotCloudService.Common.Helper;
using IotCloudService.Common.Modes;
using IotCloudService.IotDataStoreService.Mode;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.IotDataStoreService.AlarmStore
{
    public class DeviceAlarmStoreManager
    {
        private DeviceInfoEx _parentDeviceInfo = null;
        private string _alarmTableName;
        private static string Conn = ConfigurationManager.AppSettings["MySqlConnectString"];


        DeviceAlarmStoreManager(DeviceInfoEx deviceInfo)
        {
            _parentDeviceInfo = deviceInfo;

            _alarmTableName = $"RT-Alarm-[{_parentDeviceInfo.CompanyCode}- {_parentDeviceInfo.DeviceCode}]";
        }

        public bool InitDevicealarmStoreManager()
        {
            string createAlarmTableSql = $"CREATE TABLE IF NOT EXISTS `{_alarmTableName}`" +
                "( `Id` INTEGER PRIMARY KEY AUTO_INCREMENT " +
                ",`AlarmDate` datetime,,`RecoverDate` datetime, `AlarmName` varchar(255)" +
                ",`AlarmLevel` int(11),`AlarmType` int(11) ,`DeviceType` varchar(255) , `DeviceName` varchar(255)" +
                ",`AlarmCondition` varchar(1000), `AlarmHelp` varchar(255) " +
                ",`Reserved1` varchar(255),`Reserved2` varchar(255), `Reserved3` varchar(255), `Reserved4` varchar(255), `Reserved5` varchar(255) )";


            Common.Helper.MySqlHelper.ExecuteNonQuery(Conn, CommandType.Text, createAlarmTableSql, null);



            return true;
        }

        public async void WriteAlarmListToDB(AlarmListInfo alarmListInfo)
        {
            string insertSQL = $"Insert into `{_alarmTableName}` values ";

            for (int i = 0; i < alarmListInfo.AlarmList.Count(); i++)
            {
                AlarmInfo temiAlarmItem = alarmListInfo.AlarmList[i];
                string alarmSql;

                alarmSql = $"(null,'{temiAlarmItem.AlarmDate}','{temiAlarmItem.RecoveryDate}',";
                alarmSql += $"'{temiAlarmItem.AlarmName}',{temiAlarmItem.AlarmLevel},";
                alarmSql += $"{temiAlarmItem.AlarmLevel},{temiAlarmItem.AlarmType},";
                alarmSql += $"'{temiAlarmItem.DeviceType}','{temiAlarmItem.DeviceName}',";
                alarmSql += $"'{temiAlarmItem.AlarmCondition}','{temiAlarmItem.AlarmHelp}',";
                alarmSql += $"'{temiAlarmItem.Reserved1}','{temiAlarmItem.Reserved2}',";
                alarmSql += $"'{temiAlarmItem.Reserved3}','{temiAlarmItem.Reserved4}',";
                alarmSql += $"'{temiAlarmItem.Reserved5}'),";

                insertSQL += alarmSql;
            }

            insertSQL.Remove(insertSQL.Length - 1, 1);

            try
            {
                Common.Helper.MySqlHelper.ExecuteNonQuery(Conn, CommandType.Text, insertSQL, null);
            }
            catch(Exception ex)
            {
                LoggerManager.Log.Error("向数据库写故障失败！");
            }
            





        }




    }
}
