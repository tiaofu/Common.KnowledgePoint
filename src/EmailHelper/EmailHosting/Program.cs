/*********************************************************
 * CopyRight: tiaoshuidenong. 
 * Author: tiaoshuidenong.
 * Address: wuhan
 * Create: 2018/4/13 16:39:48
 * Modify: 2018/4/13 16:39:48
 * Blog: http://www.cnblogs.com/tiaoshuidenong/
 * Description: Email send 
 *********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmailLib;
namespace EmailHosting
{
    class Program
    {
        static void Main(string[] args)
        {
            //SmtpToEmail.SendMessage("xxxx@163.com", "工作汇报123", "关于最近的工作问题，最近的工作开展问题，需要该井");
            var result = ApiToEmail.Send("xxxx@163.com");
            Console.WriteLine($"result:{ result }");
            Console.Read();
        }
    }
}
