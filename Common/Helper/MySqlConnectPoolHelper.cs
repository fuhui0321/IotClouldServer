using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotCloudService.Common.Helper
{
    public class MySqlConnectPoolHelper
    {

        private static MySqlConnectPoolHelper cpool = null;//池管理对象
        private static Object objlock = typeof(MySqlConnectPoolHelper);//池管理对象实例
        private int PoolSize = 10;//池中连接数
        private int useCount = 0;//已经使用的连接数
        private ArrayList pool = null;//连接保存的集合
        private String ConnectionStr = "";//连接字符串

        public MySqlConnectPoolHelper()
        {
            ConnectionStr = ConfigurationManager.AppSettings["MySqlConnectString"];
            PoolSize = 10;
            pool = new ArrayList();
        }

        public int InitMySqlConnectPool()
        {
            int ConnectSize = 0;

            for (; ConnectSize < PoolSize; ConnectSize++)
            {

                MySqlConnectHelper newConnect = new MySqlConnectHelper(ConnectionStr);

                if (newConnect.OpenConnect() == true)
                {
                    pool.Add(newConnect);
                }
                else
                {
                    break;   
                }
            }

            if (ConnectSize < 1)
            {
                return -1;
            }
            return ConnectSize;
            

        }

        //创建获取连接池对象
        public static MySqlConnectPoolHelper getPool()
        {
            lock (objlock)
            {
                if (cpool == null)
                {
                    cpool = new MySqlConnectPoolHelper();
                }
                return cpool;
            }
        }

        //获取池中的连接
        public MySqlConnectHelper getConnection()
        {
            lock (pool)
            {
                MySqlConnectHelper tmp = null;
                if (pool.Count > 0)
                {
                    tmp = (MySqlConnectHelper)pool[0];

                    

                    pool.RemoveAt(0);
                    //不成功
                    if (!isUserful(tmp))
                    {
                        //可用的连接数据已去掉一个
                        useCount--;
                        tmp = getConnection();
                    }
                }
                else
                {
                    //可使用的连接小于连接数量
                    if (useCount < PoolSize)
                    {
                        try

                        {
                            //创建连接
                            MySqlConnectHelper conn = new MySqlConnectHelper(ConnectionStr);
                            conn.OpenConnect();
                            useCount++;
                            tmp = conn;
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }
                return tmp;
            }
        }

        //关闭连接,加连接回到池中
        public void closeConnection(MySqlConnectHelper con)
        {
            lock (pool)
            {
                if (con != null)
                {
                    pool.Add(con);
                }
            }
        }
        //目的保证所创连接成功,测试池中连接
        private bool isUserful(MySqlConnectHelper con)
        {
           
            return con.isUserful();
        }
    }
}
