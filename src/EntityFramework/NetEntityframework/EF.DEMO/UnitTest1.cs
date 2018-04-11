using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DB.First;

namespace EF.DEMO
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //__MigrationHistory这个表为ef中Code first的编程方式
            //如果通过code first自动生成的数据库，就会存在这个表，这个表存在代码自动生成数据库
            //后续程序启动程序就会判断该表是否存在，如果存在就代码使用的是code first代码创建的数据库
            //如果不存在代表使用的是code first数据库优先的方式
            //两种模式不可混用
            //以下代码在数据库不存在的情况下，可自动创建数据库表
            //一旦数据实体更改增加字段，那么就需要采用ef数据迁移的方式更新数据库
            //就算手动更新了数据库，增加了字段，保证了与实体的一致也达不到效果，也将报错
            //可以删除__MigrationHistory表，代表采用的是code first中的数据库优先，这时保证数据库接口与实体的结构一直就可以保存成功
            try
            {
                UserInfo u = new UserInfo()
                {
                    Id = "2",
                    UserCode = "1",
                    UserName = "1",
                    PassWord = "1",
                    PassWord3 = "2",
                    PassWord4 = "2"
                };
                using (HotelDBContext contenxt = new HotelDBContext())
                {
                    contenxt.customer.Add(u);
                    contenxt.SaveChanges();
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
