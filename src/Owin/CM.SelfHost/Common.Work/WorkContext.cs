using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
namespace Common.Work
{
    /// <summary>
    /// 当前程序上下文
    /// </summary>
    public class WorkContext
    {
        /// <summary>
        /// 当前IP
        /// </summary>
        public static string CurrentIp { get; set; }
        /// <summary>
        /// 程序端口
        /// </summary>
        public static int Port { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public static string[] Urls { get; set; }

        static WorkContext()
        {
            string name = Dns.GetHostName();
            IPAddress[] ipadrlist = Dns.GetHostAddresses(name);
            foreach (IPAddress ipa in ipadrlist)
                if (ipa.AddressFamily == AddressFamily.InterNetwork)
                    CurrentIp = ipa.ToString();
            Urls = new string[] { "http://" + CurrentIp + ":" + Port, "http://localhost:" + Port };
        }
    }
}
