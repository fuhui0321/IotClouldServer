using IotCloudService.Common.Helper;
using IotCloudService.Common.Modes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IotCloudService.Common.Modes.DataStoreTableInfo;

namespace IotCloudService.Common.DataStore
{
    public class DataSotreItem
    {
        private DataStoreConfigInfo _dataStoreConfig = null;
        private DataStoreCondition _dataStoreCondition = null;
        private DateTime _LastSaveTime = DateTime.Now;
        private DateTime _LastWriteDBTime = DateTime.Now;
        private int _BufferSize = 0;

        private string _tableNamePrefix;
        private string _currentTableName = "";

       
        private string _insertDataStoreSql;
        
        private DataStoreManager _parentDataStoreManager = null;


        public DataSotreItem(DataStoreManager parentDataStoreManager,
            DataStoreConfigInfo dataStoreConfig)
        {
            _dataStoreConfig = dataStoreConfig;
            _parentDataStoreManager = parentDataStoreManager;
            //_dataStoreItemCnn = connString;
        }

        public bool InitializeDataStoreItem()
        {
            _dataStoreCondition = new DataStoreCondition();
            if (_dataStoreCondition.InitializeConditionList(_parentDataStoreManager,_dataStoreConfig.StoreCondition) == false)
            {               
                return false;
            }

            _tableNamePrefix = $"[{_dataStoreConfig.DeviceCode}]-[{_dataStoreConfig.TableInfo.TableName}]";


            //增加需要读取的TagName
            for(int i =0;i < _dataStoreConfig.TableInfo.FieldList.Length; i++)
            {
                _parentDataStoreManager.InsertTagName(_dataStoreConfig.TableInfo.FieldList[i].TagName);
            }
            




            return true;

        }


        public int DataStoreItemHandler()
        {
            int nAddItemCount = -1;

            DateTime tempNowTime = DateTime.Now;
           

            if (IsSaveFlag(tempNowTime) == false)
                return 0;

            if (CheckStoreTableName(tempNowTime) == true)
            {



            }
            InsertDataItem(tempNowTime);



            if (IsWriteDBFlag(tempNowTime) == true)
            {
                WriteBufferToDB();
            }

            return nAddItemCount;

        }

        int InsertDataItem(DateTime NowTime)
        {
            string tempFieldValue = "";           


            if (_BufferSize > 0)
	        {
		        tempFieldValue += ",";
	        }

            tempFieldValue += "(null,'";
	        tempFieldValue += NowTime.ToString("yyyy-MM-dd HH:mm:ss");
	        tempFieldValue += "','";

	        for (int i = 0; i< _dataStoreConfig.TableInfo.FieldList.Count(); i++)
	        {
                DataStoreFieldInfo tempField = _dataStoreConfig.TableInfo.FieldList[i];
                string tempTagValue = _parentDataStoreManager.ReadTagValue(tempField.TagName);                
                tempFieldValue += tempTagValue + ",";
	        }


            tempFieldValue += "') ";
            _insertDataStoreSql += tempFieldValue;
            _BufferSize++;

	        return 0;
        }

        private bool IsSaveFlag(DateTime NowTime)
        {


            
            bool tempConditionResult;

            TimeSpan ts1 = new TimeSpan(NowTime.Ticks);
            TimeSpan ts2 = new TimeSpan(_LastSaveTime.Ticks);
            TimeSpan ts3 = ts1.Subtract(ts2).Duration();

            

            if ((ts3.TotalSeconds < _dataStoreConfig.SaveInterval))
            {
                return false;
            }


            tempConditionResult = _dataStoreCondition.GetConditionResult();
            if (tempConditionResult == false)
            {
                return false;
            }

            _LastSaveTime = NowTime;

            return true;
        }

        public bool IsWriteDBFlag(DateTime NowTime)
        {
                     

            TimeSpan ts1 = new TimeSpan(NowTime.Ticks);
            TimeSpan ts2 = new TimeSpan(_LastWriteDBTime.Ticks);
            TimeSpan ts3 = ts1.Subtract(ts2).Duration();


            if ((ts3.TotalSeconds < _dataStoreConfig.BufferTime) ||
                (_BufferSize > _dataStoreConfig.BufferSize))
            {
                _LastWriteDBTime = NowTime;
                return true;
            }


            return false;


        }


        private bool CheckStoreTableName(DateTime NowTime)
        { 

            string tempTableName = "";            

	        switch (_dataStoreConfig.TableInfo.SpliteTableType)
	        {
	        case SPLITE_TABLE_TYPE.SPLITE_DEFAULT:
		        tempTableName = _tableNamePrefix;
		
		        break;
	        case SPLITE_TABLE_TYPE.SPLITE_MONTH:
  
                tempTableName = $"{_tableNamePrefix}-{ NowTime.ToString("yyyy-MM")}";
		
		        break;
	        case SPLITE_TABLE_TYPE.SPLITE_YEAR:

                    //strftime(tempDate, 50, "-%Y", p);
                    //tempTableName = _mDataTableStruct.TableNamePrefix + tempDate;
                tempTableName = $"{_tableNamePrefix}-{ NowTime.ToString("yyyy")}";


                    break;
	        }

	        if (string.Equals(_currentTableName, tempTableName) == false )
	        {


                WriteBufferToDB();

		        if (CreateNewDataStoreTable(tempTableName) == 0)
		        {
                    _currentTableName = tempTableName;

			        return true;
		        }
	        }

	        return false;

        }

        private int WriteBufferToDB()
        {
            int nRes = 0;
            string WriteSql;
            


            if (_BufferSize <= 0)
                return nRes;

            WriteSql = "insert into `[DataStore]-";
            WriteSql += _currentTableName;
            WriteSql += "` values ";
            WriteSql += _insertDataStoreSql;


            try
            {
                nRes = Common.Helper.MySqlHelper.ExecuteNonQuery(_parentDataStoreManager.GetDBConnectString(), CommandType.Text, WriteSql, null);

                _insertDataStoreSql = "";
                _BufferSize = 0;
                nRes = 0;
            }
            catch (Exception ex)
            {
                LoggerManager.Log.Error($"{_dataStoreConfig.CompanyCode}-{_dataStoreConfig.DeviceCode}:向数据库写存储数据失败！,SQL = {WriteSql}");
                nRes = -1;
            }    

            return nRes;
        }

        public int CreateNewDataStoreTable(string NewTableName)
        {
            string SQL_CreatTable;
            int nRes = 0;

            SQL_CreatTable = "CREATE TABLE IF NOT EXISTS `[DataStore]-" + NewTableName +
                "` (`Id` INTEGER PRIMARY KEY AUTO_INCREMENT,`DataTime` datetime DEFAULT NULL ,`FieldValue` varchar(5000))";


            try
            {
                Common.Helper.MySqlHelper.ExecuteNonQuery(_parentDataStoreManager.GetDBConnectString(), CommandType.Text, SQL_CreatTable, null);
                nRes = 0;
            }
            catch (Exception ex)
            {
                LoggerManager.Log.Error($"{_dataStoreConfig.CompanyCode}-{_dataStoreConfig.DeviceCode}:创建数据存储表失败！，,SQL = {SQL_CreatTable}");
                nRes = -1;
            }
            
            return nRes;
        }

    }

    
}
