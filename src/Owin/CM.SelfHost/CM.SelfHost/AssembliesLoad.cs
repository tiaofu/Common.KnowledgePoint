using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Reflection;

namespace CM.SelfHost
{
    public class AssembliesLoad : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public AssemblyElementCollection AssemblyNames
        {
            get { return (AssemblyElementCollection)this[""]; }
        }

        public static AssembliesLoad GetSection()
        {
            return ConfigurationManager.GetSection("AssembliesLoad") as AssembliesLoad;
        }
    }
    public class AssemblyElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new AssemblyElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            AssemblyElement serviceTypeElement = (AssemblyElement)element;
            return serviceTypeElement.AssemblyName;
        }
    }

    public class AssemblyElement : ConfigurationElement
    {
        [ConfigurationProperty("assemblyName", IsRequired = true)]
        public string AssemblyName
        {
            get { return (string)this["assemblyName"]; }
            set { this["assemblyName"] = value; }
        }
    }
}
