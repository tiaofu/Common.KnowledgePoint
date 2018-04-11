using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Configuration;

namespace Citms.DailyStatistics
{
    /// <summary>
    /// 全局RabbitMQ消息处理
    /// </summary>
    public class RabbitMQClient : IDisposable
    {
        static string ip = ConfigManage.dbConfig.MqAddress;// ConfigurationManager.AppSettings["IP"];
        static string port = ConfigManage.dbConfig.Port;// ConfigurationManager.AppSettings["PORT"];
        static string user = ConfigManage.dbConfig.User;//ConfigurationManager.AppSettings["USER"];
        static string pwd = ConfigManage.dbConfig.Pwd;//ConfigurationManager.AppSettings["PWD"];
        static string exchange = ConfigManage.dbConfig.Exchange;//ConfigurationManager.AppSettings["EXCHANGE"];
        /// <summary>
        /// 发送消息路由键默认前缀
        /// </summary>
        private static string STUFF_ROUTINGKEY = "Database.Changed.";

        //队列名称以本机服务器IP为后缀,方便查错
        private static string QueneName = "IMS__" + Guid.NewGuid().ToString("N");
        private static string Type = "topic";
        /// <summary>
        /// 监听主题路由键
        /// </summary>
        private static readonly string[] ROUNTING_KEYS = new string[] { "Moniting.Status.#", "Database.Changed.#", "Notification.Alarm.Inspection.#", "Notification.Update.AssetsPrevention", "Notification.Warn.VideoLost" };

        /// <summary>
        /// RabbitMQ连接对象
        /// </summary>
        public static IConnection RbConnection = null;

        /// <summary>
        /// 初始化MQ连接 和消息接收
        /// </summary>
        /// <param name="config">MQ连接配置信息</param>
        /// <param name="callback">接收到消息后的回调事件
        /// 会返回两个参数  第一个 为路由键  第二个为实际接收到的数据
        /// </param>
        public static void InitClient(Action<string, string> callback)
        {
            try
            {
                InitMQConnection(5, true);

                //接收消息
                //ReciveMessage(callback);
            }
            catch (Exception ex)
            {
                LogHelper.WriteInfo("RabbitMQ连接异常,可能由于主机IP用户名密码相关信息配置错误");
                throw ex;
            }
        }

        /// <summary>
        /// 初始化MQ连接
        /// </summary>
        public static void InitMQConnection(int tryConnectionCount = 50, bool throwException = false)
        {
            if (tryConnectionCount < 0)
            {
                if (throwException == true)
                {
                    throw new Exception("多次尝试连接RabbitMQ服务器失败，终止连接");
                }
                else
                {
                    //LogHelper.WriteLog("多次尝试连接RabbitMQ服务器失败，终止连接");
                }
            }
            if (RbConnection == null || !RbConnection.IsOpen)
            {
                ConnectionFactory FactoryFactory = new ConnectionFactory();
                FactoryFactory.HostName = ConfigManage.dbConfig.MqAddress;
                FactoryFactory.UserName = ConfigManage.dbConfig.User;
                FactoryFactory.Password = ConfigManage.dbConfig.Pwd;
                FactoryFactory.Port = Convert.ToInt32(ConfigManage.dbConfig.Port);
                try
                {
                    RbConnection = FactoryFactory.CreateConnection();
                }
                catch (Exception ex)
                {
                    LogHelper.WriteInfo("连接RabbitMQ服务器失败");
                    Thread.Sleep(300);
                    tryConnectionCount--;
                    InitMQConnection(tryConnectionCount);
                }
            }
        }

        /// <summary>
        /// 测试MQ连接信息
        /// </summary>
        /// <param name="Host">主机地址</param>
        /// <param name="Port">端口</param>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">密码</param>
        public static void TestConnection(string Host, int Port, string UserName, string Password)
        {
            IConnection RbConnection = null;
            ConnectionFactory FactoryFactory = new ConnectionFactory();
            FactoryFactory.HostName = Host;
            FactoryFactory.UserName = UserName;
            FactoryFactory.Password = Password;
            FactoryFactory.Port = Port;
            try
            {

                RbConnection = FactoryFactory.CreateConnection();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (RbConnection != null)
                {
                    RbConnection.Close();
                    RbConnection.Dispose();
                    RbConnection = null;
                }
            }
        }

       

        #region "发送消息接口"
        /// <summary>
        /// 异步发送消息到MQ
        /// </summary>
        /// <param name="RoutingKey">路由键  比如设备更改 则传Assest 路口则传Spotting</param>
        /// <param name="message">消息</param>
        /// <param name="isUseStuff">路由键是否使用“Database.Changed.”前缀 为true则最终路由键为Database.Changed.RoutingKey
        /// 否则为RoutingKey
        /// </param>
        public static void SendMessage(string RoutingKey, string message, bool isUseStuff = true)
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                if (!string.IsNullOrEmpty(message))
                {
                    if (string.IsNullOrEmpty(RoutingKey))
                    {
                        RoutingKey = "#";
                    }
                    try
                    {
                        InitMQConnection();
                        using (var channel = RbConnection.CreateModel())
                        {
                            channel.ExchangeDeclare(ConfigManage.dbConfig.Exchange, Type, true);
                            //消息内容主体
                            byte[] bytes = Encoding.UTF8.GetBytes(message);
                            if (isUseStuff)
                            {
                                RoutingKey = STUFF_ROUTINGKEY + RoutingKey;
                            }
                            channel.BasicPublish(ConfigManage.dbConfig.Exchange, RoutingKey, null, bytes);
                            LogHelper.WriteInfo(string.Format("RabbitMQ发送消息,消息如下 路由键:{0} 消息体:{1}", RoutingKey, message));
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteInfo("RabbitMQ发送消息异常"+ ex.ToString());
                    }
                }
            });
        }       

        #endregion

        /// <summary>
        /// 释放连接资源
        /// </summary>
        public void Dispose()
        {
            if (RbConnection != null)
            {
                RbConnection.Dispose();
                RbConnection = null;
            }
        }

    }
}
