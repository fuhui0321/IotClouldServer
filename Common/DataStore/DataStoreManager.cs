using IotCloudService.Common.Helper;
using IotCloudService.Common.Modes;
using IotCloudService.Common.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IotCloudService.Common.DataStore
{
    public class DataStoreManager
    {
        private string[] _tagNameList;
        private Dictionary<string, string> _tagValueMap = new Dictionary<string, string>();
        private List<DataSotreItem> _dataStoreList = new List<DataSotreItem>();

        private bool _ThreadExitFalg = false;
        private Thread _threadHandle;
        private string _cnnDB = "";
        private RedisInfo _redisInfo;
        private string _redisTableName;


        
        public int InitializeDataStoreManager(string DBCnnString,
            CompanyInfoEx companyInfo,DeviceInfoEx deviceInfo)
        {
            _cnnDB = DBCnnString;
            _redisInfo = companyInfo.Redis;
            _redisTableName = $"[{companyInfo.CompanyCode}]-[{deviceInfo.DeviceCode}]";

            

            return 0;
        }


        public int AddDataStoreItem(DataStoreConfigInfo dataStoreConfigItem)
        {
            DataSotreItem newDataStoreItem = new DataSotreItem(this,dataStoreConfigItem);
            if (newDataStoreItem.InitializeDataStoreItem() == true)
            {
                _dataStoreList.Add(newDataStoreItem);
            }

            return 0;
        }

        public string GetDBConnectString()
        {
            return _cnnDB;
        }

        public void InsertTagName(string newTagName)
        {
            if (_tagValueMap.ContainsKey(newTagName))
                return;

            _tagValueMap.Add(newTagName, "0");
            return;
            
        }

        public string ReadTagValue(string TagName)
        {
            string tagValue="";

            if (_tagValueMap.ContainsKey(TagName) == true)
            {
                tagValue = _tagValueMap[TagName];
            }
            else
            {
                LoggerManager.Log.Error($"没有读取到标签[{TagName}]的数据！");
            }

            

            return tagValue;
        }

        public void StartDataStoreTask()
        {
            if (_dataStoreList.Count < 1)
                return;


            _tagNameList = new string[_tagValueMap.Count];
            int index = 0;
            foreach (var item in _tagValueMap)
            {
                _tagNameList[index] = item.Key;
                index++;



            }

            _ThreadExitFalg = false;
            _threadHandle = new Thread(DataStoreHandler);
            _threadHandle.Start();

        }

        public void StopDataStoreTask()
        {
            if (_dataStoreList.Count < 1)
                return;

            _ThreadExitFalg = true;
            _threadHandle.Join();
        }

        private void DataStoreHandler()
        {
            while(_ThreadExitFalg == false)
            {
                UpdateTagValues();

                for (int i = 0; i < _dataStoreList.Count; i++)
                {
                    _dataStoreList[i].DataStoreItemHandler();
                }

                Thread.Sleep(1000);
            }

        }

        private void UpdateTagValues()
        {
            var redisClient = RedisManager.GetClient();
            string[] tagValueList;

            redisClient.Select(0);
            if (redisClient == null)
            {
                LoggerManager.Log.Error("获取Redis对象失败！");
                return;
            }

            tagValueList = redisClient.HMGet(_redisTableName, _tagNameList);

            for (int i=0;i<_tagNameList.Length;i++)
            {
                _tagValueMap[_tagNameList[i]] = tagValueList[i];
            }


        }
      
    }
}
