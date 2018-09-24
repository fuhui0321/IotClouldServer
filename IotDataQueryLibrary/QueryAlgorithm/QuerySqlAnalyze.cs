using IotCloudService.IotDataQueryLibrary.Mode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.IotDataQueryLibrary.QueryAlgorithm
{
    public class QuerySqlAnalyze
    {
        public string[] GetQuerySqlList(DataQueryParamObject DataQueryParam, DataStoreConfig DataStoreItem)
        {
            string[] querySqlList = null;

            switch(DataStoreItem.SpliteTableType)
            {
                case SPLITE_TABLE_TYPE.SPLITE_DEFAULT:
                    break;
                case SPLITE_TABLE_TYPE.SPLITE_MONTH:
                    break;
                case SPLITE_TABLE_TYPE.SPLITE_YEAR:
                    break;
                default:
                    break;

            }

            return querySqlList;

        }

        private string[] AnalyzeMonthSpliteSql(DataQueryParamObject DataQueryParam, DataStoreConfig DataStoreItem)
        {
            string[] querySqlList = null;

            List<QueryCondition> queryConditionList = new List<QueryCondition>();

            DateTime dtStart = Convert.ToDateTime(DataQueryParam.StartDate);
            DateTime dtEnd = Convert.ToDateTime(DataQueryParam.EndDate);

            int Months = (dtStart.Year - dtEnd.Year) * 12 + (dtStart.Month - dtEnd.Month) +1;

            int StartMonth = dtStart.Month;
            int currYear = dtStart.Year;

            string QueryTableNamePrex = $"[datastore]-[{ DataStoreItem.DeviceCode}]-[{ DataStoreItem.DefaultTableName}]"; 
            for (int i=0; i < Months; i++)
            {
                QueryCondition tempQueryCondition = new QueryCondition();
                string startDate;
                string endDate;

                int currMonth = StartMonth + i;

                if (currMonth >12)
                {
                    currYear++;
                    currMonth = 1;
                }

                string QueryTableNameSuffix = $"-{currYear}-{currMonth}";
                string tempQueryTableName = QueryTableNamePrex + QueryTableNameSuffix;

                tempQueryCondition.TableName = tempQueryTableName;

                

                if (i == 0)
                {
                    startDate = $"{currYear}-{currMonth}-{dtStart.Day} {dtStart.Hour}:{dtStart.Minute}:{dtStart.Second}";                   
                    
                }                
                else
                {
                    startDate = $"{currYear}-{currMonth}-{dtStart.Day} 00:00:00";

                }

                if (i == (Months - 1))
                {
                    endDate = $"{currYear}-{currMonth}-{dtEnd.Day} {dtEnd.Hour}:{dtEnd.Minute}:{dtEnd.Second}";
                }
                else
                {
                    DateTime d1 = new DateTime(currYear, currMonth, 1);
                    DateTime d2 = d1.AddMonths(1).AddDays(-1);

                    endDate = $"{currYear}-{currMonth}-{d2.Day} 23:59:59";
                }

                tempQueryCondition.SelectCondition = $"between '{startDate} 'and '{endDate}'";
                queryConditionList.Add(tempQueryCondition);
            }

            

            return querySqlList;

        }

        private string[] AnalyzeYearSpliteSql(DataQueryParamObject DataQueryParam, DataStoreConfig DataStoreItem)
        {
            string[] querySqlList = null;

            return querySqlList;

        }

    }
}
