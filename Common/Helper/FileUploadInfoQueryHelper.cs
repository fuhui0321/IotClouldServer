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
    public class FileUploadInfoQueryHelper
    {
        private String companyCode;
        private String deviceCode;
        private String dbName;
        private String tableName;
        //private static string Conn = ConfigurationManager.AppSettings["MySqlConnectString"];

        public bool Initialize(String CompanyCode, String DeviceCode)
        {
            int res = 0;

            companyCode = CompanyCode;
            deviceCode = DeviceCode;
                        
            dbName = $"[iot]-[{CompanyCode}]";
            tableName = $"file_upload_info_[{DeviceCode}]";



            return true;        
        }
        public List<FileUploadInfo> QueryFileUploadInfo(QueryConditionBase queryInfo)
        {
            int res = 0;
            List<FileUploadInfo> tempFileUploadInfoList = new List<FileUploadInfo>();

            String quserSql = GenerateQuerySql(queryInfo);

            MySqlConnectHelper mysqlCnn = MySqlConnectPoolHelper.getPool().getConnection();
            res = mysqlCnn.SelectDB(dbName);

            MySqlDataReader dataReader = mysqlCnn.ExecuteReader(CommandType.Text, quserSql, null);

            while (dataReader.Read())
            {

                FileUploadInfo tempFileUploadInfo = new FileUploadInfo();

                tempFileUploadInfo.ID = (int)dataReader["ID"];
                tempFileUploadInfo.DateTime = (string)dataReader["DateTime"];
                tempFileUploadInfo.Content = (string)dataReader["Content"];
                tempFileUploadInfo.UserName = (string)dataReader["UserName"];
                string tempDeviceList = (string)dataReader["FilePathList"];
                tempFileUploadInfo.FilePathList = tempDeviceList.Split(',');

                tempFileUploadInfoList.Add(tempFileUploadInfo);


            }

            dataReader.Close();            
            MySqlConnectPoolHelper.getPool().closeConnection(mysqlCnn);
            mysqlCnn = null;

            return tempFileUploadInfoList;
        }

        public bool InsertFileUploadInfo(FileUploadInfo newFileUploadInfo)
        {
            int res = 0;
            String currentDate = DateTime.Now.ToLocalTime().ToString();
            String FilePathList = string.Join(",", newFileUploadInfo.FilePathList);
            String deleteSql = $"insert into {tableName} VALUES(NULL,'{currentDate}','{newFileUploadInfo.Content}','{newFileUploadInfo.UserName}','{FilePathList}')";

            MySqlConnectHelper mysqlCnn = MySqlConnectPoolHelper.getPool().getConnection();
            res = mysqlCnn.SelectDB(dbName);

            res = mysqlCnn.ExecuteNonQuery(CommandType.Text, deleteSql, null);

            MySqlConnectPoolHelper.getPool().closeConnection(mysqlCnn);
            mysqlCnn = null;

            if (res > 0)
            {
                return true;
            }

            return false; 
        }

        public bool DeleteFileUploadInfo(FileUploadInfo deleteFileUploadInfo)
        {
            int res = 0;

            String deleteSql = $"delete from {tableName} where id={deleteFileUploadInfo.ID}";

            MySqlConnectHelper mysqlCnn = MySqlConnectPoolHelper.getPool().getConnection();
            res = mysqlCnn.SelectDB(dbName);

            res = mysqlCnn.ExecuteNonQuery(CommandType.Text, deleteSql, null);

            MySqlConnectPoolHelper.getPool().closeConnection(mysqlCnn);
            mysqlCnn = null;

            if (res >0)
            {
                return true;
            }

            
            return false;
        }

        private String GenerateQuerySql(QueryConditionBase queryInfo)
        {
            String querySql = null;

            if (String.IsNullOrEmpty(queryInfo.StartDate) || String.IsNullOrEmpty(queryInfo.EndDate))
            {
                querySql = $"select top 20 * from `{tableName}`";
            }
            else
            {
                querySql = $"select * from `{tableName}` where between `DataTime` between  '{queryInfo.StartDate}' and '{queryInfo.EndDate}'";

            }


            


            return querySql;

        }
    }
}
