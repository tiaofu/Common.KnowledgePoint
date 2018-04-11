using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel.Composition;
using Common.Task;
namespace ConsoleHosting
{
    class Program
    {
        [ImportMany(typeof(IWork))]
        public static IEnumerable<IWork> WorkList;

        static void Main(string[] args)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string path = "Common.Http";
            string dllPath = basePath + "/" + path;
            string[] paths = Directory.GetFiles(dllPath, "*.dll");
            foreach (var item in paths)
            {
                AppDomain.CurrentDomain.Load(item);
            }            
        }
    }
}
