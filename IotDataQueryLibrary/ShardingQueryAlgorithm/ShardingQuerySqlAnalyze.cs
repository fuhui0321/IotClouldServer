using IotCloudService.Common.Modes;
using IotCloudService.ShardingDataQueryLibrary.Mode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IotCloudService.Common.Modes.DataStoreTableInfo;

namespace IotCloudService.ShardingDataQueryLibrary.ShardingQueryAlgorithm
{
    public class ShardingQuerySqlAnalyze
    {

        List<QueryCondition> queryConditionList = new List<QueryCondition>();

        public List<QueryCondition> GetQuerySqlList(DataQueryParamObject DataQueryParam, DataStoreTableInfo DataStoreItem)
        {
            

            switch(DataStoreItem.SpliteTableType)
            {

                case SPLITE_TABLE_TYPE.SPLITE_DEFAULT:
                    AnalyzeDefaultSpliteSql(DataQueryParam, DataStoreItem);
                    break;
                case SPLITE_TABLE_TYPE.SPLITE_MONTH:
                    AnalyzeMonthSpliteSql(DataQueryParam, DataStoreItem);
                    break;
                case SPLITE_TABLE_TYPE.SPLITE_YEAR:
                    break;
                default:
                    break;

            }

            return queryConditionList;

        }

        private void AnalyzeDaySpliteSql(DataQueryParamObject DataQueryParam, DataStoreTableInfo DataStoreItem)
        {
            
            

            DateTime dtStart = Convert.ToDateTime(DataQueryParam.StartDate);
            DateTime dtEnd = Convert.ToDateTime(DataQueryParam.EndDate);

            DateTime d3 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", dtStart.Year, dtStart.Month, dtStart.Day));
            DateTime d4 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", dtEnd.Year, dtEnd.Month, dtEnd.Day));
            int days = (d4 - d3).Days;           

            int StartMonth = dtStart.Month;
            int currYear = dtStart.Year;

            string QueryTableNamePrex = $"[datastore]-[{ DataQueryParam.DeviceCode}]-[{ DataQueryParam.TableName}]";

            for (int i = 0; i < days; i++)
            {

                QueryCondition tempQueryCondition = new QueryCondition();
                string startDate;
                string endDate;

                if (i == 0)
                {
                    startDate = $"{dtStart.Year}-{dtStart.Month}-{dtStart.Day} {dtStart.Hour}:{dtStart.Minute}:{dtStart.Second}";
                }
                else
                {
                    startDate = $"{dtStart.Year}-{dtStart.Month}-{dtStart.Day} 00:00:00";
                }

                if (i == (days -1))
                {
                    endDate = $"{dtEnd.Year}-{dtEnd.Month}-{dtEnd.Day} {dtEnd.Hour}:{dtEnd.Minute}:{dtEnd.Second}";
                }
                else
                {
                    endDate = $"{dtStart.Year}-{dtStart.Month}-{dtStart.Day} 23:59:59";
                }
                

                string QueryTableNameSuffix = $"-{dtStart.Year}-{dtStart.Month}-{dtStart.Day}";
                string tempQueryTableName = QueryTableNamePrex + QueryTableNameSuffix;

                tempQueryCondition.SelectCondition = $"between '{startDate}'and '{endDate}'";
                tempQueryCondition.TableName = tempQueryTableName;

                dtStart.AddDays(1);
            }




                return ;

        }

        private void AnalyzeDefaultSpliteSql(DataQueryParamObject DataQueryParam, DataStoreTableInfo DataStoreItem)
        {
            DateTime dtStart = Convert.ToDateTime(DataQueryParam.StartDate);
            DateTime dtEnd = Convert.ToDateTime(DataQueryParam.EndDate);

            QueryCondition tempQueryCondition = new QueryCondition();

            tempQueryCondition.TableName = $"[datastore]-[{ DataQueryParam.DeviceCode}]-[{ DataQueryParam.TableName}]";
            tempQueryCondition.SelectCondition = $"between '{DataQueryParam.StartDate}' and '{DataQueryParam.EndDate}'";
            queryConditionList.Add(tempQueryCondition);
           

            return; 

        }


        private void AnalyzeMonthSpliteSql(DataQueryParamObject DataQueryParam, DataStoreTableInfo DataStoreItem)
        {
            

            DateTime dtStart = Convert.ToDateTime(DataQueryParam.StartDate);
            DateTime dtEnd = Convert.ToDateTime(DataQueryParam.EndDate);

            int Months = (dtEnd.Year - dtStart.Year) * 12 + (dtEnd.Month - dtStart.Month) +1;

            int StartMonth = dtStart.Month;
            int currYear = dtStart.Year;

            string QueryTableNamePrex = $"[datastore]-[{ DataQueryParam.DeviceCode}]-[{ DataQueryParam.TableName}]"; 
            for (int i=0; i < Months; i++)
            {
                QueryCondition tempQueryCondition = new QueryCondition();
                string startDate;
                string endDate;
               

                string QueryTableNameSuffix = $"-{dtStart.Year}-{dtStart.ToString("MM")}";
                string tempQueryTableName = QueryTableNamePrex + QueryTableNameSuffix;

                tempQueryCondition.TableName = tempQueryTableName;

                

                if (i == 0)
                {
                    startDate = $"{dtStart.Year}-{dtStart.Month}-{dtStart.Day} {dtStart.Hour}:{dtStart.Minute}:{dtStart.Second}";                   
                    
                }                
                else
                {
                    startDate = $"{dtStart.Year}-{dtStart.Month}-01 00:00:00";

                }

                if (i == (Months - 1))
                {
                    endDate = $"{dtStart.Year}-{dtStart.Month}-{dtEnd.Day} {dtEnd.Hour}:{dtEnd.Minute}:{dtEnd.Second}";
                }
                else
                {
                    DateTime d1 = new DateTime(dtStart.Year, dtStart.Month, 1);
                    DateTime d2 = d1.AddMonths(1).AddDays(-1);

                    endDate = $"{dtStart.Year}-{dtStart.Month}-{d2.Day} 23:59:59";
                }


                dtStart = dtStart.AddMonths(1);

                tempQueryCondition.SelectCondition = $"between '{startDate}' and '{endDate}'";
                queryConditionList.Add(tempQueryCondition);
            }

            return ;

        }

        private string[] AnalyzeYearSpliteSql(DataQueryParamObject DataQueryParam, DataStoreTableInfo DataStoreItem)
        {
            string[] querySqlList = null;

            return querySqlList;

        }

    }
}
