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

namespace tiaoshuidenong.AutoMapper
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, Inherited = true)]
    public class MapperClassAttribute : Attribute
    {
        public string Name { get; set; }
        public static string GetName(Type type)
        {
            var attr = type.GetCustomAttributes(typeof(MapperClassAttribute), true).FirstOrDefault();
            return attr != null ? (attr as MapperClassAttribute).Name ?? default(string) : default(string);
        }
    }
}
