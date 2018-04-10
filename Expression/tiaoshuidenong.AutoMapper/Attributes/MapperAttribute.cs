/*********************************************************
 * CopyRight: tiaoshuidenong. 
 * Author: tiaoshuidenong
 * Address: wuhan
 * Create: 2018-04-10 17:44:16
 * Modify: 2018-04-10 17:44:16
 * Description: 
 *********************************************************/
using System;
using System.Linq;
using System.Reflection;

namespace tiaoshuidenong.AutoMapper
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class MapperAttribute : Attribute
    {
        public string TargetName { get; set; }

        public MapperAttribute() { }
        public MapperAttribute(string targetName)
        {
            this.TargetName = targetName;
        }

        public static string GetTargetName(PropertyInfo property)
        {
            var attr = property.GetCustomAttributes<MapperAttribute>(true).FirstOrDefault();
            return attr != null ? (attr as MapperAttribute).TargetName ?? default(string) : default(string);
        }
    }
}
