using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Reflection;

namespace CM.API
{
    public class UserResolver: DefaultAssembliesResolver
    {
        public override ICollection<Assembly> GetAssemblies()
        {
            ICollection<Assembly> baseAssemblies = base.GetAssemblies();
            List<Assembly> assemblies = new List<Assembly>(baseAssemblies);
            var assembly = AppDomain.CurrentDomain.BaseDirectory;
            string path = assembly + "\\" + "CM.API.dll";
            var controllersAssembly = Assembly.LoadFrom(path);
            baseAssemblies.Add(controllersAssembly);
            return assemblies;
        }
    }
}
