using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Autofac;
using System.Reflection;
using Common.Work;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
namespace Common.ConsoleCarrier
{
    class Program
    {
        /// <summary>
        /// 插件集合
        /// </summary>
        private static IEnumerable<IPlugin> Plugins { get; set; }

        static void Main(string[] args)
        {
            MefConfig.Init();
            Plugins = MefConfig.ResolveMany<IPlugin>();
            foreach (var item in Plugins)
            {
                item.Init();
            }
            Console.Read();
        }
    }
}
