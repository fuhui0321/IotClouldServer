using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.DataStore
{
    public enum CONDITION_TYPE
    {
        CONDITION_EQ,//等于
        CONDITION_GT,//大于
        CONDITION_LT,//小于
        CONDITION_NE,//不等于
        CONDITION_AND,//与
        CONDITION_OR,//或


    }
    public class ConditionItem
    {
        public string ConditionTagName { set; get; }
        public float TargetValue1 { set; get; }
        public float TargetValue2 { set; get; }
        public CONDITION_TYPE ConditionType { set; get; }
        public float TagValue { set; get; }


    }
}
