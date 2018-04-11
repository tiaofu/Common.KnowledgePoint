/*********************************************************
 * CopyRight: tiaoshuidenong. 
 * Author: tiaoshuidenong
 * Address: wuhan
 * Create: 2017-05-10 17:44:16
 * Modify: 2017-05-10 17:44:16
 * Blog: http://www.cnblogs.com/tiaoshuidenong/
 * Description: Rewrite
 *********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basicknowledge
{
    public class Student
    {
        public Student()
        {
            this.Age = 12;
            Console.WriteLine("调用构造函数");
        }
        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }
    }
}
