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
        //[DllImport("kernel32.dll")]
        //public static extern int WinExec(string exeName, int operType);//添加申明调用的是系统提供的函数  
        static void Main(string[] args)
        {
            try
            {
                Console.Title = "违法每日统计工具";
              
                if (ConfigManage.SysConfig == null || string.IsNullOrEmpty(ConfigManage.SysConfig.DbConn))
                {
                    string path = AppDomain.CurrentDomain.BaseDirectory + "\\config.js";
                    if (!File.Exists(path))
                    {
                        SysConfig config = new SysConfig() { DbConn = "", ApiAddress = "", NoticePercent = 0 };
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
                LogHelper.WriteInfo("启动成功");
                LogHelper.WriteInfo(string.Format("可执行命令 config(修改配置文件) reload(应用新的配置文件)"));
                LogHelper.WriteInfo("每日凌晨四点执行昨日统计");
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
                            //ConfigForm form = new ConfigForm();
                            //form.ShowDialog();
                        }
                        else if (strCommand.ToUpper() == "RELOAD")
                        {
                            ConfigManage.Reload();
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
