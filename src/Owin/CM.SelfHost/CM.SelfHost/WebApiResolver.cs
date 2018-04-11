using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dispatcher;
using System.Reflection;

namespace CM.SelfHost
{
    public class WebApiResolver : DefaultAssembliesResolver
    {
        public override ICollection<Assembly> GetAssemblies()
        {
            AssembliesLoad settings = AssembliesLoad.GetSection();
            if (null != settings)
            {
                try
                {


                    foreach (AssemblyElement element in settings.AssemblyNames)
                    {
                        AssemblyName assemblyName = AssemblyName.GetAssemblyName(element.AssemblyName);
                        if (!AppDomain.CurrentDomain.GetAssemblies().Any(assembly => AssemblyName.ReferenceMatchesDefinition(assembly.GetName(), assemblyName)))
                        {
                            AppDomain.CurrentDomain.Load(assemblyName);
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
            return base.GetAssemblies();
        }
    }
}
