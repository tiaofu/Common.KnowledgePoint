/*********************************************************
 * CopyRight: tiaoshuidenong. 
 * Author: tiaoshuidenong.
 * Address: wuhan
 * Create: 2018/4/11 16:39:48
 * Modify: 2018/4/11 16:39:48
 * Blog: http://www.cnblogs.com/tiaoshuidenong/
 * Description: wcf 
 *********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace WcfService
{
    class Program
    {
        static void Main(string[] args)
        {
            __todo:
            try
            {
                string url = "http://127.0.0.1:8889";
                Uri baseAddress = new Uri(url);
                ServiceHost host = new ServiceHost(typeof(OperationImpl), baseAddress);
                /**
                  WCF的终结点类型有很多种类 :
                  BasicHttpBinding: 用于把 WCF 服务当作 ASMX Web 服务。用于兼容旧的Web ASMX 服务（web service）。
                  WSHttpBinding: 比 BasicHttpBinding 更加安全，通常用于 non-duplex (双工)  服务通讯
                  NetTcpBinding: 使用 TCP 协议，用于在局域网(Intranet)内跨机器通信。有几个特点：可靠性、事务支持和安全，优化了 WCF 到 WCF 的通信。限制是服务端和客户端都必须使用 WCF 来实现
                  方法 AddServiceEndpoint 
                  在使用不同的Binding时，address配置也不尽相同，可配置自己单独的http地址，或者tcp地址，也可以写相对地址，与baseaddress拼接                  
               **/
                host.AddServiceEndpoint(typeof(IOperationInterface), new NetTcpBinding() { Security = new NetTcpSecurity() { Mode = SecurityMode.None } }, "net.tcp://127.0.0.1:8888/Call");
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                host.Description.Behaviors.Add(smb);
                host.Open();
                Console.WriteLine($"成功创建代理:{url}");
                Console.Read();
            }
            catch (Exception e)
            {
                Console.WriteLine("创建连接异常,ex：{0}", e.ToString());
                goto __todo;
            }
        }
    }
}
