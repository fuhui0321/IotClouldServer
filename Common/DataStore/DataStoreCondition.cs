using IotCloudService.Common.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IotCloudService.Common.DataStore
{
    public class DataStoreCondition
    {
        private List<ConditionItem> _conditionList = null;
        private DataStoreManager _parentDataStoreManager = null;



        public bool InitializeConditionList(DataStoreManager parentDataStoreManager, string conditionJson)
        {
            _parentDataStoreManager = parentDataStoreManager;

            if (string.IsNullOrEmpty(conditionJson) == true)
            {
                _conditionList = new List<ConditionItem>();
                return true;

            } 
            else
            {
                try
                {
                    _conditionList = JsonConvert.DeserializeObject<List<ConditionItem>>(conditionJson);
                }
                catch
                {
                    return false;
                }

                //增加需要读取的TagName
                for (int i = 0; i < _conditionList.Count; i++)
                {
                    _parentDataStoreManager.InsertTagName(_conditionList[i].ConditionTagName);
                }

            }

            


            return true;

        }

        private void  GetConditionTagValue()
        {            

            for (int i = 0; i < _conditionList.Count; i++)
            {
                if (_parentDataStoreManager.ReadTagValue(_conditionList[i].ConditionTagName) == null)
                    return;
                _conditionList[i].TagValue =float.Parse(_parentDataStoreManager.ReadTagValue(_conditionList[i].ConditionTagName));
            }

        } 

        public bool GetConditionResult()
        {
            bool last_flag = true;
            bool logical_operator = false;//false;||,ture:&&
            bool temp_flag = false;
            bool logical_operator_enable = false;


            GetConditionTagValue();
            //if (GetConditionTagValue()== false)
            //{
            //    return false;
            //}

            for (int i = 0; i < _conditionList.Count(); i++)
            {
                switch (_conditionList[i].ConditionType)
                {
                    case CONDITION_TYPE.CONDITION_EQ://=
                        {
                            if ((_conditionList[i].TagValue < (_conditionList[i].TargetValue1 + 0.000001)) &&
                                 (_conditionList[i].TagValue > (_conditionList[i].TargetValue1 - 0.000001)))
                            {
                                temp_flag = true;
                            }
                            else
                            {
                                temp_flag = false;
                            }
                        }
                        break;
                    case CONDITION_TYPE.CONDITION_GT://>
                        {
                            if (_conditionList[i].TagValue > _conditionList[i].TargetValue1)
                            {
                                temp_flag = true;
                            }
                            else
                            {
                                temp_flag = false;
                            }
                        }
                        break;
                    case CONDITION_TYPE.CONDITION_LT://<
                        {
                            if (_conditionList[i].TagValue < _conditionList[i].TargetValue1)
                            {
                                temp_flag = true;
                            }
                            else
                            {
                                temp_flag = false;
                            }
                        }
                        break;
                    case CONDITION_TYPE.CONDITION_NE://<>
                        if ((_conditionList[i].TagValue > _conditionList[i].TargetValue1) &&
                            (_conditionList[i].TagValue < _conditionList[i].TargetValue2))
                        {
                            temp_flag = true;
                        }
                        else
                        {
                            temp_flag = false;
                        }
                        break;
                    case CONDITION_TYPE.CONDITION_AND://&&
                        {
                            logical_operator = true;
                            logical_operator_enable = true;


                        }

                        break;
                    case CONDITION_TYPE.CONDITION_OR://||
                        {
                            logical_operator = false;
                            logical_operator_enable = true;
                        }

                        break;

                }

                if (logical_operator_enable == true)
                {
                    if (logical_operator == false)
                    {
                        last_flag = last_flag || temp_flag;
                    }
                    else
                    {
                        last_flag = last_flag && temp_flag;

                        logical_operator = false;
                    }
                }
                else
                    last_flag = temp_flag;




            }

            return last_flag;
        }

    }
}
