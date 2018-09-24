/** 
*  @brief  RedisOperator
*  @author Fu Hui
*  @version 1.0
*  @date   2017-07-01
*/

#define debug



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Redis
{
    public class RedisOperator : RedisBase
    {
        public const char  SplisChar = '#';
        private string _companyCode;
        private string _stationCode;
        private string _hashKey;
        private string _channelKey;
        public RedisOperator(string companyCode, string stationCode) : base()
        {
            this._companyCode = companyCode;
            this._stationCode = stationCode;
            this._hashKey = $"{companyCode}_{stationCode}";
            this._channelKey = $"{companyCode}_{stationCode}";
        }

        public Dictionary<string, string> GetTagValues(List<string> tagNames)
        {
            var result = new Dictionary<string, string>();

            if (tagNames?.Count > 0)
            {

                var vs = this.Client.HMGet(this._hashKey, tagNames.ToArray()) ?? new string[] { };
                for (int i = 0; i < tagNames.Count; i++)
                {
                    if (i < vs.Length)
                    {
                        result.Add(tagNames[i], vs[i]);
                    }
                    else
                    {
                        result.Add(tagNames[i], null);
                    }
                }

            }
            else
            {
                result = this.Client.HGetAll(this._hashKey);
            }
            return result;
        }
        public void SetTagValues(Dictionary<string, string> list)
        {
            this.Client.HMSet(this._hashKey, list);

        }
        public void Publish(string channel,string msg)
        {
            var pcs = this.Client.Publish($"{_channelKey}.{channel}", msg);
        }

    }
}
