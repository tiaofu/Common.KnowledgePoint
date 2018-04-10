using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressionLinq;

namespace ExpressionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "DynamicLinq";
            Student student1 = new Student();
            student1.Name = "1234";
            student1.Age = 12;
            Student student2 = new Student();
            student2.Name = "2345";
            student2.Age = 13;
            Student student3 = new Student();
            student3.Name = "3456";
            student3.Age = 14;
            Student student4 = new Student();
            student4.Name = "5678";
            student4.Age = 15;
            Student student5 = new Student();
            student5.Name = "9120";
            student5.Age = 16;
            StudentSearch sc = new StudentSearch();
            sc.Name = "12";
            sc.StartAge = 13;
            sc.EndAge = 49;

            List<Student> stuList = new List<Student>() { student1, student2, student3, student4, student5 };            
            var result = stuList.AsQueryable().Search(sc).ToList();
            foreach (var item in result)
            {
                Console.WriteLine($"Name:{item.Name},Age:{item.Age}");
            }
            Console.WriteLine("press to exit!");
            Console.ReadLine();
        }
    }
}
