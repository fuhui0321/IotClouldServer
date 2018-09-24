using IotCloudService.Common.Helper;
using IotCloudService.Common.Modes;

using IotCloudService.ShardingDataQueryLibrary.Mode;
using IotCloudService.ShardingDataQueryLibrary.ShardingQueryAlgorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.ShardingDataQueryLibrary
{
    public class IotDataQueryInterface
    {

        public QueryResultBase HandleQueryInterface(DataQueryParamObject dataQueryParam)
        {
            QueryResultBase QueryResultObject = null;

            DataStoreTableInfo DataStoreConfigItem = CompanyManagerHelper.GetDeviceDataStoreConfigItem(
                dataQueryParam.CompanyCode, dataQueryParam.DeviceCode, dataQueryParam.TableName);

            if (DataStoreConfigItem == null)
            {
                QueryResultObject = new QueryResultBase();

                QueryResultObject.ResultCode = QueryResultCodeEnum.QUERY_ERROR_NO_DATA_STORE_CONFIG;
                QueryResultObject.QueryData = null;

                return QueryResultObject;
            }
            else
            {
                switch (dataQueryParam.DataQueryType)
                {
                    case DATA_QUERY_TYPE.HISTORY_QUERY:
                        QueryAlgorithm HistoryQueryObject = new QueryAlgorithm();
                        HistoryQueryObject.InitQueryParams(dataQueryParam, DataStoreConfigItem);
                        QueryResultObject = HistoryQueryObject.GetQueryRecordsetData();
                        HistoryQueryObject.ClearMySQLCnn();
                        break;
                    default:
                        break;

                }

            }


            

            return QueryResultObject;
        }

        public QueryResultBase HandleQueryInterfaceExtend(DataQueryParamObject dataQueryParam)
        {
            QueryResultBase QueryResultObject = null;

            DataStoreTableInfo DataStoreConfigItem = CompanyManagerHelper.GetDeviceDataStoreConfigItem(
                dataQueryParam.CompanyCode, dataQueryParam.DeviceCode, dataQueryParam.TableName);

            if (DataStoreConfigItem == null)
            {
                QueryResultObject = new QueryResultBase();

                QueryResultObject.ResultCode = QueryResultCodeEnum.QUERY_ERROR_NO_DATA_STORE_CONFIG;
                QueryResultObject.QueryData = null;

                return QueryResultObject;
            }
            else
            {
                switch (dataQueryParam.DataQueryType)
                {
                    case DATA_QUERY_TYPE.HISTORY_QUERY:
                        QueryAlgorithm HistoryQueryObject = new QueryAlgorithm();
                        HistoryQueryObject.InitQueryParams(dataQueryParam, DataStoreConfigItem);
                        QueryResultObject = HistoryQueryObject.GetQueryRecordsetData(true);
                        HistoryQueryObject.ClearMySQLCnn();
                        break;
                    default:
                        break;

                }

            }




            return QueryResultObject;
        }
    }
}
