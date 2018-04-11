using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Reflection;
using Common.Work;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
namespace Common.Web
{
    class Program
    {

        
        static void Main(string[] args)
        {
            WebPlugin wb = new WebPlugin();
            wb.Init();
            Console.Read();
        }
        
        
    }
}
