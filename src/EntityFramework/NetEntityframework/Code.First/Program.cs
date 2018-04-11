using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code.First
{
    class Program
    {
        static void Main(string[] args)
        {
            //实例化一个数据上下文对象  
            HotelDBContext dbcontext = new HotelDBContext();
            //创建数据库如果不存在的话  
            //if (dbcontext.Database.CreateIfNotExists())
            //{
            //    Console.WriteLine("数据库已创建成功！");
            //    Console.Read();
            //}
            //else
            //{
            //    Console.WriteLine("数据库已经存在，无需创建！");
            //}
            Customer su = new Customer();
            su.CusName = "2";
            su.Id = 3;
            su.CusId = "1";
            dbcontext.customer.Add(su);
            dbcontext.SaveChanges();

            Console.Read();
        }
    }
}
