using IotCloudService.Common.Helper;
using IotCloudService.Common.Modes;
using IotCloudService.ShardingDataQueryLibrary.Mode;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.ShardingDataQueryLibrary.ShardingQueryAlgorithm
{
    public class StatAlgorithm
    {
        private ShardingStatSqlAnalyze objStatSqlAnalyze = new ShardingStatSqlAnalyze();
        string Conn = ConfigurationManager.AppSettings["MySqlConnectString"];
        List<StatQueryCondition> statConditionList = null;
        DataStatParamInfo dataStatParam = null;
        DataStoreTableInfo dataStoreItem = null;

        MySqlConnectHelper mysqlCnn = null;

        ~StatAlgorithm()
        {

            //if (mysqlCnn != null)
            //{
            //    MySqlConnectPoolHelper.getPool().closeConnection(mysqlCnn);
            //    mysqlCnn = null;

            //}


        }

        public void ClearMySQLCnn()
        {

            //if (mysqlCnn != null)
            //{
            //    MySqlConnectPoolHelper.getPool().closeConnection(mysqlCnn);
            //    mysqlCnn = null;

            //}

        }

        public bool InitStatParams(DataStatParamInfo DataStatParam, DataStoreTableInfo DataStoreItem)
        {
            int res = 0;
            QueryResultBase QueryResultObject = new QueryResultBase();

            dataStatParam = DataStatParam;
            dataStoreItem = DataStoreItem;



            


            mysqlCnn = new MySqlConnectHelper(Conn);




            if (mysqlCnn.OpenConnect()==false)
            {
                LoggerManager.Log.Info("MySql连接对象创建失败！\n");
                return false;
            }

            String DbName = $"`[iot]-[{DataStatParam.CompanyCode}]`";
            res = mysqlCnn.SelectDB(DbName);


            //MySqlDataReader companyReader = MySqlHelper.ExecuteReader(Conn, CommandType.Text, "select * from companyinfo", null);

            return true;




        }

        public QueryResultBase GetStatRecordsetData()
        {
            QueryResultBase queryResult = new QueryResultBase();


            statConditionList = objStatSqlAnalyze.GetStatSqlList(dataStatParam, dataStoreItem);

            if (statConditionList == null)
            {
                queryResult.ResultCode = QueryResultCodeEnum.QUERY_ERROR_ANALYZE_SQL;
                queryResult.QueryData = null;

                //return queryResult;
            }
            else if (statConditionList.Count() < 0)
            {
                queryResult.ResultCode = QueryResultCodeEnum.QUERY_ERROR_ANALYZE_SQL;
                queryResult.QueryData = null;
                //return queryResult;
            }else
            {
                StatResultBase statData = new StatResultBase();

                statData.StatRecordset = GetQueryRecordset();

                if (statData.StatRecordset == null)
                {
                    queryResult.ResultCode = QueryResultCodeEnum.QUERY_ERROR_GET_QUERY_RECORDSET_DATA;
                    queryResult.QueryData = null;
                    //return queryResult;

                }else
                {                    
                    queryResult.ResultCode = QueryResultCodeEnum.QUERY_SUCCESS;
                    queryResult.QueryData = statData;
                }
            }



            return queryResult;
        }


        private long GetQueryRecordsetCount()
        {

            long RecordsetCount = 0;

            //if (mysqlCnn == null)
            //{
            //    LoggerManager.Log.Info("没有从MySql连接池获取连接对象！\n");
            //    return -1;
            //}



            //for (int i = 0; i < statConditionList.Count; i++)
            //{
            //    string cmdStr = $"select COUNT(*) from `{queryConditionList[i].TableName}` where `DataTime` {queryConditionList[i].SelectCondition}";
            //    long tempCount = 0;

            //    MySqlDataReader dataReader = mysqlCnn.ExecuteReader(CommandType.Text, cmdStr, null);

            //    while (dataReader.Read())
            //    {
            //        tempCount = dataReader.GetUInt32(0);
            //    }

            //    dataReader.Close();

            //    queryConditionList[i].offset = RecordsetCount;
            //    queryConditionList[i].count = tempCount;

            //    RecordsetCount += tempCount;

            //}

            return RecordsetCount;
        }

        private List<RecordsetDataStat> GetQueryRecordset()
        {

            List<RecordsetDataStat> tableData = new List<RecordsetDataStat>();

            string statSqlPrefix = "";
            string statSqlSuffix = "";

            int RecordsetCount = 0;
            long pageSize = dataStatParam.PageSize;
            

            if (mysqlCnn == null)
            {
                LoggerManager.Log.Info("MySql连接对象为空！\n");
                return null;
            }


            switch (dataStatParam.StatType)
            {
                case STAT_TYPE.STAT_YEAR:
                    statSqlPrefix = $"select year(DataTime) AS 'Year',FieldValue ";
                    statSqlSuffix = "GROUP by YEAR(DataTime)";
                    break;

                case STAT_TYPE.STAT_MONTH:
                    statSqlPrefix = $"select year(DataTime) AS 'Year',month(DataTime) AS 'Month',FieldValue ";
                    statSqlSuffix = "GROUP by YEAR(DataTime),MONTH(DataTime)";
                    break;

                case STAT_TYPE.STAT_QUARTER:
                    statSqlPrefix = $"select year(DataTime) AS 'Year',quarter(DataTime) AS 'Quarter',FieldValue ";
                    statSqlSuffix = "GROUP by YEAR(DataTime),QUARTER(DataTime)";

                    break;

                case STAT_TYPE.STAT_WEEK:
                    statSqlPrefix = $"select year(DataTime) AS 'Year',month(DataTime) AS 'Month',week(DataTime) AS 'Week',FieldValue ";
                    statSqlSuffix = "GROUP by YEAR(DataTime),MONTH(DataTime),WEEK(DataTime)";
                    break;

                case STAT_TYPE.STAT_DAY:
                    statSqlPrefix = $"select year(DataTime) AS 'Year',month(DataTime) AS 'Month',day(DataTime) AS 'Day',FieldValue ";
                    statSqlSuffix = "GROUP by YEAR(DataTime),MONTH(DataTime),DAY(DataTime)";
                    break;

                case STAT_TYPE.STAT_HOUR:
                    statSqlPrefix = $"select year(DataTime) AS 'Year',month(DataTime) AS 'Month',day(DataTime) AS 'Day',hour(DataTime) AS 'Hour',FieldValue ";
                    statSqlSuffix = "GROUP by YEAR(DataTime),MONTH(DataTime),DAY(DataTime),HOUR(DataTime)";
                    break;
            }


            for (int i = 0; i < statConditionList.Count; i++)
            {
                StatQueryCondition tempStatCondition = statConditionList[i];                

                string cmdStr= null ;


                //if (i == statConditionList.Count-1)
                //{
                //    DateTime dtEnd = Convert.ToDateTime(tempStatCondition.EndDate);
                //    dtEnd = dtEnd.AddHours(1);
                //    tempStatCondition.EndDate = $"{dtEnd.Year}-{dtEnd.Month}-{dtEnd.Day} {dtEnd.Hour}:{dtEnd.Minute}:{dtEnd.Second}"; ;

                //}



                cmdStr = statSqlPrefix + $"from `{tempStatCondition.TableName}` where `DataTime` ";
                cmdStr += $"between '{tempStatCondition.StartDate}' and '{tempStatCondition.EndDate}' " + statSqlSuffix;



                //switch (dataStatParam.StatType)
                //{
                //    case STAT_TYPE.STAT_YEAR:
                //        cmdStr = $"select year(DataTime) AS 'Year',FieldValue from `{tempStatCondition.TableName}` where `DataTime` ";
                //        cmdStr += $"between '{tempStatCondition.StartDate}' and '{tempStatCondition.EndDate}'";
                //        cmdStr += $"Order by YEAR(DataTime)";
                //        break;

                //    case STAT_TYPE.STAT_MONTH:
                //        cmdStr = $"select year(DataTime) AS 'Year',month(DataTime) AS 'Month',FieldValue from `{tempStatCondition.TableName}` where `DataTime` ";
                //        cmdStr += $"between '{tempStatCondition.StartDate}' and '{tempStatCondition.EndDate}'";
                //        cmdStr += $"GROUP by YEAR(DataTime),MONTH(DataTime)";
                //        break;

                //    case STAT_TYPE.STAT_QUARTER:
                //        cmdStr = $"select year(DataTime) AS 'Year',quarter(DataTime) AS 'Quarter',FieldValue from `{tempStatCondition.TableName}` where `DataTime` ";
                //        cmdStr += $"between '{tempStatCondition.StartDate}' and '{tempStatCondition.EndDate}'";
                //        cmdStr += $"GROUP by YEAR(DataTime),QUARTER(DataTime)";
                //        break;

                //    case STAT_TYPE.STAT_WEEK:
                //        cmdStr = $"select year(DataTime) AS 'Year',month(DataTime) AS 'Month',week(DataTime) AS 'Week',FieldValue from `{tempStatCondition.TableName}` where `DataTime` ";
                //        cmdStr += $"between '{tempStatCondition.StartDate}' and '{tempStatCondition.EndDate}'";
                //        cmdStr += $"GROUP by YEAR(DataTime),MONTH(DataTime),WEEK(DataTime)";
                //        break;

                //    case STAT_TYPE.STAT_DAY:
                //        cmdStr = $"select year(DataTime) AS 'Year',month(DataTime) AS 'Month',day(DataTime) AS 'Day',FieldValue from `{tempStatCondition.TableName}` where `DataTime` ";
                //        cmdStr += $"between '{tempStatCondition.StartDate}' and '{tempStatCondition.EndDate}'";
                //        cmdStr += $"GROUP by YEAR(DataTime),MONTH(DataTime),DAY(DataTime)";
                //        break;

                //    case STAT_TYPE.STAT_HOUR:
                //        cmdStr = $"select year(DataTime) AS 'Year',month(DataTime) AS 'Month',day(DataTime) AS 'Day',hour(DataTime) AS 'Hour',FieldValue from `{tempStatCondition.TableName}` where `DataTime` ";
                //        cmdStr += $"between '{tempStatCondition.StartDate}' and '{tempStatCondition.EndDate}'";
                //        cmdStr += $"GROUP by YEAR(DataTime),MONTH(DataTime),DAY(DataTime),HOUR(DataTime)";
                //        break;
                //}
                

                MySqlDataReader dataReader = mysqlCnn.ExecuteReader(CommandType.Text, cmdStr, null);

                while (dataReader.Read())
                {
                    RecordsetDataStat newRecordsetData = GetStatRecordData(dataReader);
                    //newRecordsetData.DataTime = dataReader["DataTime"] == null ? "" : ((DateTime)dataReader["DataTime"]).ToString("yyyy-MM-dd hh:mm:ss");
                    ////newRecordsetData.FieldValue = dataReader["FieldValue"] == null ? "" : dataReader["FieldValue"].ToString();

                    //string tempFieldValue = dataReader["FieldValue"] == null ? "" : dataReader["FieldValue"].ToString();

                    //string[] arrayFieldString =  tempFieldValue.Split(',');
                    //float[] arrayFieldValue = new float[arrayFieldString.Length];

                    //for (int fieldIndex = 0; fieldIndex < arrayFieldValue.Length; fieldIndex++)
                    //{
                    //    string tempValue = arrayFieldString[fieldIndex].Trim();
                    //    if (string.IsNullOrEmpty(tempValue))
                    //        arrayFieldValue[fieldIndex] = 0;
                    //    else
                    //    {
                    //        try
                    //        {
                    //            arrayFieldValue[fieldIndex] = float.Parse(tempValue);
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            arrayFieldValue[fieldIndex] = 0;
                    //        }                            
                    //    }

                    //}

                    //newRecordsetData.FieldValue = arrayFieldValue;

                    

                    if (RecordsetCount > 1)
                    {
                        float[] arrayFieldValue = tableData[RecordsetCount-1].FieldValue;

                        for (int fileIndex=0; fileIndex < arrayFieldValue.Length; fileIndex++)
                        {
                            if (newRecordsetData.FieldValue[fileIndex] >= arrayFieldValue[fileIndex])
                            {
                                arrayFieldValue[fileIndex] = newRecordsetData.FieldValue[fileIndex] - arrayFieldValue[fileIndex];
                            }
                            else
                            {
                                LoggerManager.Log.Error($"Company<{dataStatParam.CompanyCode}>,Device <{ dataStatParam.DeviceCode}>,Table <{ dataStatParam.TableName}> 数据统计错误！");
                            }
                        }
                    }

                    tableData.Add(newRecordsetData);
                    RecordsetCount++;
                }

                dataReader.Close();

                //获取统计的最后一条数据记录
                if (i == statConditionList.Count - 1)
                {
                    string lastRecordSqlStr = statSqlPrefix +  $" from `{tempStatCondition.TableName}` where `DataTime` between '{tempStatCondition.StartDate}' and '{tempStatCondition.EndDate}' "; lastRecordSqlStr += "ORDER BY `DataTime` DESC LIMIT 1";
                    
                    MySqlDataReader lastDataReader = mysqlCnn.ExecuteReader(CommandType.Text, lastRecordSqlStr, null);

                    RecordsetDataStat lastRecordsetData = null;


                    while (lastDataReader.Read())
                    {
                        lastRecordsetData = GetStatRecordData(lastDataReader);
                    }

                    lastDataReader.Close();



                    float[] arrayFieldValue = tableData[RecordsetCount - 1].FieldValue;

                    for (int fileIndex = 0; fileIndex < arrayFieldValue.Length; fileIndex++)
                    {
                        if (lastRecordsetData.FieldValue[fileIndex] >= arrayFieldValue[fileIndex])
                        {
                            arrayFieldValue[fileIndex] = lastRecordsetData.FieldValue[fileIndex] - arrayFieldValue[fileIndex];
                        }
                        else
                        {
                            LoggerManager.Log.Error($"Company<{dataStatParam.CompanyCode}>,Device <{ dataStatParam.DeviceCode}>,Table <{ dataStatParam.TableName}> 数据统计错误！");
                        }
                    }
                }                
            }

            return tableData;


        }


        private RecordsetDataStat GetStatRecordData(MySqlDataReader statDataReader)
        {
            RecordsetDataStat newRecordsetData = new RecordsetDataStat();
            //newRecordsetData.DataTime = statDataReader["DataTime"] == null ? "" : ((DateTime)statDataReader["DataTime"]).ToString("yyyy-MM-dd hh:mm:ss");

            switch (dataStatParam.StatType)
            {
                case STAT_TYPE.STAT_YEAR:
                    string tempYear = statDataReader["Year"] == null ? "" : statDataReader["Year"].ToString();
                    newRecordsetData.DataTime = tempYear + "年";
                    break;

                case STAT_TYPE.STAT_MONTH:
                    string tempYearMonth = statDataReader["Year"].ToString();
                    newRecordsetData.DataTime = $"{statDataReader["Year"].ToString()}年{statDataReader["Month"].ToString()}月";


                    break;

                case STAT_TYPE.STAT_QUARTER:
                    newRecordsetData.DataTime = $"{statDataReader["Year"].ToString()}年{statDataReader["Quarter"].ToString()}季度";

                    break;

                case STAT_TYPE.STAT_WEEK:
                    newRecordsetData.DataTime = $"{statDataReader["Year"].ToString()}年{statDataReader["Quarter"].ToString()}季度{statDataReader["Week"].ToString()}周";
                    break;

                case STAT_TYPE.STAT_DAY:
                    newRecordsetData.DataTime = $"{statDataReader["Year"].ToString()}年{statDataReader["Month"].ToString()}月{statDataReader["Day"].ToString()}日";
                    break;

                case STAT_TYPE.STAT_HOUR:
                    newRecordsetData.DataTime = $"{statDataReader["Year"].ToString()}-{statDataReader["Month"].ToString()}-{statDataReader["Day"].ToString()} {statDataReader["Hour"].ToString()}";
                    break;
            }

            string tempFieldValue = statDataReader["FieldValue"] == null ? "" : statDataReader["FieldValue"].ToString();

            string[] arrayFieldString = tempFieldValue.Split(',');
            float[] arrayFieldValue = new float[arrayFieldString.Length];

            for (int fieldIndex = 0; fieldIndex < arrayFieldValue.Length; fieldIndex++)
            {
                string tempValue = arrayFieldString[fieldIndex].Trim();
                if (string.IsNullOrEmpty(tempValue))
                    arrayFieldValue[fieldIndex] = 0;
                else
                {
                    try
                    {
                        arrayFieldValue[fieldIndex] = float.Parse(tempValue);
                    }
                    catch (Exception ex)
                    {
                        arrayFieldValue[fieldIndex] = 0;
                    }
                }

            }

            newRecordsetData.FieldValue = arrayFieldValue;

            return newRecordsetData;

        }


        private List<RecordsetDataStat> CalculateStatRecordset(List<RecordsetDataStat> StatRecordsetData)
        {
            List<RecordsetDataStat> StatRecordsetResult = new List<RecordsetDataStat>();

            //DataStatParamInfo dataStatParam = null;
            //DataStoreTableInfo dataStoreItem = null;

            if (dataStatParam.StatType == STAT_TYPE.STAT_YEAR)
                return StatRecordsetData;



            switch (dataStatParam.StatType)
            {
                case STAT_TYPE.STAT_YEAR:
                    break;
                case STAT_TYPE.STAT_QUARTER:
                    break;
                case STAT_TYPE.STAT_MONTH:
                    break;
                case STAT_TYPE.STAT_WEEK:
                    break;
                case STAT_TYPE.STAT_DAY:
                    break;
                case STAT_TYPE.STAT_HOUR:
                    break;

            }

            return StatRecordsetResult;
        }

        private int CalculateStatReocrdCount()
        {
            int statCount = 0;

            DateTime dtStart = Convert.ToDateTime(dataStatParam.StartDate);
            DateTime dtEnd = Convert.ToDateTime(dataStatParam.EndDate);

            TimeSpan t3 = dtEnd - dtStart;

            switch (dataStatParam.StatType)
            {
                case STAT_TYPE.STAT_YEAR:

                    statCount = dtEnd.Year - dtStart.Year + 1;
                    break;
                case STAT_TYPE.STAT_QUARTER:
                    break;
                case STAT_TYPE.STAT_MONTH:                 

                    statCount = (dtEnd.Year - dtStart.Year -1)*12  + (12-dtStart.Month+ dtEnd.Month);

                    break;
                case STAT_TYPE.STAT_WEEK:
                    break;
                case STAT_TYPE.STAT_DAY:

                    statCount = (int)t3.TotalDays +1;
                    break;
                case STAT_TYPE.STAT_HOUR:
                    statCount = (int)t3.TotalHours +1;
                    break;

            }




            return statCount;
        }

        private List<RecordsetDataStat> GetYearStatRecordset(List<RecordsetDataStat> StatRecordsetData)
        {
            List<RecordsetDataStat> StatRecordsetResult = new List<RecordsetDataStat>();

            DateTime dtStart = Convert.ToDateTime(dataStatParam.StartDate);
            DateTime dtEnd = Convert.ToDateTime(dataStatParam.EndDate);


            

            return StatRecordsetResult;

        }
    }
}
