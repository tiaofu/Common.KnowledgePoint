using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;
using Quartz;
using Quartz.Impl;
using Newtonsoft.Json;

namespace Citms.DailyStatistics
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.Title = "违法数据处理工具";
                //if (ConfigManage.dbConfig == null || string.IsNullOrEmpty(ConfigManage.dbConfig.DbConn))
                //{
                //    ConfigForm form = new ConfigForm();
                //    form.ShowDialog();
                //}
                if (ConfigManage.dbConfig == null || string.IsNullOrEmpty(ConfigManage.dbConfig.DbConn))
                {
                    string path = AppDomain.CurrentDomain.BaseDirectory + "\\config.js";
                    if (!File.Exists(path))
                    {
                        DbConfig config = new DbConfig() { DbConn = "", Exchange = "", MqAddress = "", Pwd = "", User = "", Sql = "", Port = "" };
                        string result = JsonConvert.SerializeObject(config, Formatting.Indented);
                        using (FileStream fs = File.Open(path, FileMode.Create))
                        {
                            char[] charData = result.ToCharArray();
                            byte[] byData = new byte[charData.Length];
                            Encoder enc = Encoding.UTF8.GetEncoder();
                            enc.GetBytes(charData, 0, charData.Length, byData, 0, true);
                            fs.Write(byData, 0, byData.Length);
                        }
                    }
                    System.Diagnostics.Process.Start(@"notepad.exe", AppDomain.CurrentDomain.BaseDirectory + "\\config.js");
                    //ConfigForm form = new ConfigForm();
                    //form.ShowDialog();
                }
                log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config"));
                LogHelper.WriteInfo("启动成功");               
                LogHelper.WriteInfo(string.Format("可执行命令 config(修改配置文件) reload(应用新的配置文件) start(开始重发)"));
                Thread thread = new Thread(new ThreadStart(delegate ()
                {
                    IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
                    scheduler.Start();
                }));
                thread.Start();
                while (true)
                {
                    string strCommand = Console.ReadLine();
                    if (string.IsNullOrEmpty(strCommand))
                    {
                        continue;
                    }
                    else
                    {
                        strCommand = strCommand.Trim();
                        if (strCommand.ToUpper() == "CONFIG")
                        {
                            System.Diagnostics.Process.Start(@"notepad.exe", AppDomain.CurrentDomain.BaseDirectory + "\\config.js");
                        }
                        else if (strCommand.ToUpper() == "RELOAD")
                        {
                            ConfigManage.Reload();
                            RabbitMQClient.InitMQConnection();
                        }
                        else if (strCommand.ToUpper() == "START")
                        {
                            Citms.DailyStatistics.DailyStatisticsWork.HandlerData();
                        }
                        else
                        {
                            LogHelper.Error(string.Format("命令不正确，可执行命令 config(修改配置文件) reload(应用新的配置文件)"));
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}
