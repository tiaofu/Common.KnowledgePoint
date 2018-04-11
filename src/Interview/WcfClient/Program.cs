/*********************************************************
 * CopyRight: tiaoshuidenong. 
 * Author: tiaoshuidenong.
 * Address: wuhan
 * Create: 2018/4/11 16:39:48
 * Modify: 2018/4/11 16:39:48
 * Blog: http://www.cnblogs.com/tiaoshuidenong/
 * Description: wcf client
 *********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //创建连接
            IOperationInterface service = InvokeContext.CreateWCFServiceByURL<IOperationInterface>("net.tcp://127.0.0.1:8888/Call");
            var result = service.Call(1, 3);
            Console.WriteLine(result);
            Console.WriteLine("调用结束!");
            Console.Read();
        }
    }
}
