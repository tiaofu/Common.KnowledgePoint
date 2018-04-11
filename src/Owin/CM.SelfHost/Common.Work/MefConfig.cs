using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
namespace Common.Work
{
    public class MefConfig
    {
        private static CompositionContainer _container;
        public static void Init()
        {
            //1.Mef接管
            var catalog = new AggregateCatalog();
            string[] Dir = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "\\Plugins");
            foreach (var item in Dir)
            {
                catalog.Catalogs.Add(new DirectoryCatalog(item));
            }           
            _container = new CompositionContainer(catalog);
            _container.ComposeParts();
        }

        /// <summary>
        /// 获取解析类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T TryResolve<T>()
        {
            return _container.GetExportedValueOrDefault<T>();
        }

        /// <summary>
        /// 获取解析类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> ResolveMany<T>()
        {
            return _container.GetExportedValues<T>();
        }

        public static T Resolve<T>()
        {
            return _container.GetExportedValue<T>();
        }
    }
}
