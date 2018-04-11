/*********************************************************
 * CopyRight: tiaoshuidenong. 
 * Author: tiaoshuidenong
 * Address: wuhan
 * Create: 2017-05-10 17:44:16
 * Modify: 2017-05-10 17:44:16
 * Blog: http://www.cnblogs.com/tiaoshuidenong/
 * Description: 
 *********************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using System.Configuration;

namespace QuartzJob
{
    class Program
    {
        static string DisplayName = ConfigurationManager.AppSettings["DisplayName"];
        static string ServiceName = ConfigurationManager.AppSettings["ServiceName"];
        static void Main(string[] args)
        {           
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config"));
            HostFactory.Run(x =>
            {
                x.UseLog4Net();

                x.Service<ServiceRunner>();

                x.SetDescription("进行数据库的数据复制");
                x.SetDisplayName(DisplayName);
                x.SetServiceName(ServiceName);

                x.EnablePauseAndContinue();
            });
        }
    }
}
