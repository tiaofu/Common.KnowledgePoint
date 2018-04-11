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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basicknowledge
{
    class A
    {
        public static int X = 2;
        static A()
        {
            X = Program.Y + 1;
        }
    }
    class Program
    {
        public static int Y = A.X + 1;
        static Program() { }
        static void Main(string[] args)
        {
            //static property 
            Console.WriteLine("X={0},Y={1}", A.X, Program.Y);  
            //rewrite        
            Animal animal = new Dog();
            animal.Shout();
            //Lazy 
            Lazy<Student> student = new Lazy<Student>();
            Console.WriteLine("已创建实例对象");
            Console.WriteLine("是否显示学生年龄?（0否1是）");
            string id = Console.ReadLine();
            if (id == "1")
            {
                Console.WriteLine(student.Value.Age);
            }
            Console.Read();
        }
    }
}
