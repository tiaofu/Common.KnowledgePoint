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
    class Programauto
    {

        [ImportMany]
        private IEnumerable<IPlugin> regist { get; set; }

        private CompositionContainer _container;
        static void Main1(string[] args)
        {
            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            Programauto pro = new Programauto();
            pro.Init();
            foreach (var item in pro.regist)
            {
                item.Init();
            }
            Console.Read();
        }
        static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Plugins\B\");
            path = System.IO.Path.Combine(path, args.Name.Split(',')[0]);
            path = String.Format(@"{0}.dll", path);
            return System.Reflection.Assembly.LoadFrom(path);
        }
        private void Init()
        {
            var catalog = new AggregateCatalog();
            string[] Dir= Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory+"\\Plugins");
            foreach (var item in Dir)
            {
                catalog.Catalogs.Add(new DirectoryCatalog(item));
            }
            _container = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection);

            try
            {
                _container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }
    }
}
