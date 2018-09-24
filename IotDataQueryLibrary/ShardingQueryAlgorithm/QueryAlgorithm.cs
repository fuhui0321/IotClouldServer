using IotCloudService.Common.Helper;
using IotCloudService.Common.Modes;
using IotCloudService.ShardingDataQueryLibrary.Mode;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.ShardingDataQueryLibrary.ShardingQueryAlgorithm
{
    public class QueryAlgorithm
    {
        private ShardingQuerySqlAnalyze objQuerySqlAnalyze = new ShardingQuerySqlAnalyze();

        List<QueryCondition> queryConditionList = null;
        DataQueryParamObject dataQueryParam = null;
        DataStoreTableInfo dataStoreItem = null;

        MySqlConnectHelper mysqlCnn = null;

         ~QueryAlgorithm()
        {

            if (mysqlCnn != null)
            {
                MySqlConnectPoolHelper.getPool().closeConnection(mysqlCnn);
                mysqlCnn = null;

            }


        }

        public void ClearMySQLCnn()
        {

            if (mysqlCnn != null)
            {
                MySqlConnectPoolHelper.getPool().closeConnection(mysqlCnn);
                mysqlCnn = null;

            }

        }



        public void InitQueryParams(DataQueryParamObject DataQueryParam, DataStoreTableInfo DataStoreItem)
        {
            int res = 0;
            QueryResultBase QueryResultObject = new QueryResultBase();

            dataQueryParam = DataQueryParam;
            dataStoreItem = DataStoreItem;

            mysqlCnn = MySqlConnectPoolHelper.getPool().getConnection();

            String DbName = $"`[iot]-[{DataQueryParam.CompanyCode}]`";

            res = mysqlCnn.SelectDB(DbName);





        }

        public QueryResultBase GetQueryRecordsetData(bool queryExtend = false)
        {
            QueryResultBase queryResult = new QueryResultBase();
            long RecordsetCount = 0;
            //QueryResult queryData = new QueryResult();


            queryConditionList = objQuerySqlAnalyze.GetQuerySqlList(dataQueryParam, dataStoreItem);

            if (queryConditionList == null)
            {
                queryResult.ResultCode = QueryResultCodeEnum.QUERY_ERROR_ANALYZE_SQL;
                queryResult.QueryData = null;

                return queryResult;
            }
            else if (queryConditionList.Count() < 0)
            {
                queryResult.ResultCode = QueryResultCodeEnum.QUERY_ERROR_ANALYZE_SQL;
                queryResult.QueryData = null;
                return queryResult;
            }
            


            //if (dataQueryParam.isFirstQuery == true)
            //{
                RecordsetCount = GetQueryRecordsetCount();

                if (RecordsetCount < 0)
                {
                    queryResult.ResultCode = QueryResultCodeEnum.QUERY_ERROR_GET_QUERY_RECORDSET_COUNT;
                    queryResult.QueryData = null;
                    return queryResult;
                }
 //           }

          

            if (queryExtend == false)
            {
                QueryResult queryData = new QueryResult();
                queryData.SumRecordCount = RecordsetCount;
                queryData.QueryRecordset = GetQueryRecordset();

                if (queryData.QueryRecordset == null)
                {
                    queryResult.ResultCode = QueryResultCodeEnum.QUERY_ERROR_GET_QUERY_RECORDSET_DATA;
                    queryResult.QueryData = null;
                    return queryResult;

                }

                queryResult.ResultCode = QueryResultCodeEnum.QUERY_SUCCESS;
                queryResult.QueryData = queryData;
            }
            else
            {
                QueryResultExtend queryData = new QueryResultExtend();
                queryData.SumRecordCount = RecordsetCount;
                queryData.QueryRecordset = GetQueryRecordsetExtend();

                if (queryData.QueryRecordset == null)
                {
                    queryResult.ResultCode = QueryResultCodeEnum.QUERY_ERROR_GET_QUERY_RECORDSET_DATA;
                    queryResult.QueryData = null;
                    return queryResult;

                }

                queryResult.ResultCode = QueryResultCodeEnum.QUERY_SUCCESS;
                queryResult.QueryData = queryData;
            }

            
            
            

            return queryResult;
        }

        private long GetQueryRecordsetCount()
        {

            long RecordsetCount = 0;

            if (mysqlCnn == null)
            {
                LoggerManager.Log.Info("没有从MySql连接池获取连接对象！\n");
                return -1;
            }



            for (int i=0; i < queryConditionList.Count; i++)
            {
                string cmdStr = $"select COUNT(*) from `{queryConditionList[i].TableName}` where `DataTime` {queryConditionList[i].SelectCondition}";
                long tempCount = 0;

                MySqlDataReader dataReader = mysqlCnn.ExecuteReader(CommandType.Text, cmdStr, null);

                if (dataReader == null)
                {
                    continue;
                }

                while (dataReader.Read())
                {
                    tempCount = dataReader.GetUInt32(0);
                }

                dataReader.Close();

                queryConditionList[i].offset = RecordsetCount;
                queryConditionList[i].count = tempCount;

                RecordsetCount += tempCount;

            }

            return RecordsetCount;
        }

        private List<RecordsetData> GetQueryRecordset()
        {
            List<RecordsetData> tableData = new List<RecordsetData>();

            long RecordsetCount = 0;            
            long pageSize = dataQueryParam.PageSize;
            long offset = dataQueryParam.RecordOffset;

            if (mysqlCnn == null)
            {
                LoggerManager.Log.Info("没有从MySql连接池获取连接对象！\n");
                return null;
            }

            for (int i = 0; i < queryConditionList.Count; i++)
            {
                QueryCondition tempQueryCondition = queryConditionList[i];

                if (offset > (tempQueryCondition.count + RecordsetCount))
                {
                    //RecordsetCount = RecordsetCount + tempQueryCondition.count;

                    offset = offset - tempQueryCondition.count;

                    continue;
                }


                string cmdStr = $"select * from `{tempQueryCondition.TableName}` where `DataTime` {tempQueryCondition.SelectCondition} LIMIT {offset},{pageSize}";                

                MySqlDataReader dataReader = mysqlCnn.ExecuteReader(CommandType.Text, cmdStr, null);

                while (dataReader.Read())
                {
                    RecordsetData newRecordsetData = new RecordsetData();
                    newRecordsetData.DataTime = dataReader["DataTime"] == null?"":((DateTime)dataReader["DataTime"]).ToString("yyyy-MM-dd hh:mm:ss");
                    newRecordsetData.FieldValue = dataReader["FieldValue"] == null ? "" : dataReader["FieldValue"].ToString();

                    tableData.Add(newRecordsetData);

                    RecordsetCount++;
                    
                }

                dataReader.Close();

                if (RecordsetCount >= dataQueryParam.PageSize)
                {
                    break;
                }                    
                else
                {
                    pageSize = pageSize - RecordsetCount;
                    offset = 0;
                }

            }

            return tableData;


        }


        private List<RecordsetDataExtend> GetQueryRecordsetExtend()
        {
            List<RecordsetDataExtend> tableData = new List<RecordsetDataExtend>();

            long RecordsetCount = 0;
            long pageSize = dataQueryParam.PageSize;
            long offset = dataQueryParam.RecordOffset;

            if (mysqlCnn == null)
            {
                LoggerManager.Log.Info("没有从MySql连接池获取连接对象！\n");
                return null;
            }

            for (int i = 0; i < queryConditionList.Count; i++)
            {
                QueryCondition tempQueryCondition = queryConditionList[i];

                if (offset > (tempQueryCondition.count + RecordsetCount))
                {
                    //RecordsetCount = RecordsetCount + tempQueryCondition.count;

                    offset = offset - tempQueryCondition.count;

                    continue;
                }


                string cmdStr = $"select * from `{tempQueryCondition.TableName}` where `DataTime` {tempQueryCondition.SelectCondition} LIMIT {offset},{pageSize}";

                MySqlDataReader dataReader = mysqlCnn.ExecuteReader(CommandType.Text, cmdStr, null);

                while (dataReader.Read())
                {
                    RecordsetDataExtend newRecordsetData = new RecordsetDataExtend();
                    newRecordsetData.DataTime = dataReader["DataTime"] == null ? "" : ((DateTime)dataReader["DataTime"]).ToString("yyyy-MM-dd hh:mm:ss");
                    string tempFileValue = dataReader["FieldValue"] == null ? "" : dataReader["FieldValue"].ToString();

                    newRecordsetData.FieldValue = tempFileValue.Split(',');

                    tableData.Add(newRecordsetData);

                    RecordsetCount++;

                }

                dataReader.Close();

                if (RecordsetCount >= dataQueryParam.PageSize)
                {
                    break;
                }
                else
                {
                    pageSize = pageSize - RecordsetCount;
                    offset = 0;
                }

            }

            return tableData;


        }


    }
}
