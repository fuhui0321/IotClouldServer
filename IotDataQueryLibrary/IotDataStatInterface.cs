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
    public class IotDataStatInterface
    {
        public QueryResultBase HandleStatInterface(DataStatParamInfo dataStatParam)
        {
            QueryResultBase QueryResultObject = new QueryResultBase();

            DataStoreTableInfo DataStoreConfigItem = CompanyManagerHelper.GetDeviceDataStoreConfigItem(
                dataStatParam.CompanyCode, dataStatParam.DeviceCode, dataStatParam.TableName);

            if (DataStoreConfigItem == null)
            {
                //QueryResultObject = new QueryResultBase();

                QueryResultObject.ResultCode = QueryResultCodeEnum.QUERY_ERROR_NO_DATA_STORE_CONFIG;
                QueryResultObject.QueryData = null;

                
            }
            else
            {
                switch (dataStatParam.DataType)
                {
                    case DATA_TYPE.DATA_SUM:
                       
                        break;
                    case DATA_TYPE.DATA_CUSTOM_ACC:
                        QueryResultObject = new QueryResultBase();
                        StatAlgorithm statAlogrithmObject = new StatAlgorithm();

                        if (statAlogrithmObject.InitStatParams(dataStatParam,DataStoreConfigItem) == true)
                        {
                            QueryResultObject = statAlogrithmObject.GetStatRecordsetData();

                        }
                        else
                        {
                            QueryResultObject.ResultCode = QueryResultCodeEnum.QUERY_ERROR_NO_DATA_STORE_CONFIG;
                            QueryResultObject.QueryData = null;
                        }

                        break;
                    default:
                        break;

                }

            }




            return QueryResultObject;
        }
    }
}
