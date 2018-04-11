/*********************************************************
 * CopyRight: tiaoshuidenong. 
 * Author: tiaoshuidenong
 * Address: wuhan
 * Create: 2018-04-10 17:44:16
 * Modify: 2018-04-10 17:44:16
 * Description: 
 *********************************************************/
using System;

namespace tiaoshuidenong.AutoMapper
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class DoNotMapperAttribute :Attribute
    {
    }
}
