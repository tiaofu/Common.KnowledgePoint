/*********************************************************
 * CopyRight: tiaoshuidenong. 
 * Author: tiaoshuidenong
 * Address: wuhan
 * Create: 2018-04-10 17:44:16
 * Modify: 2018-04-10 17:44:16
 * Description: 
 *********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace tiaoshuidenong.AutoMapper
{
    internal sealed class MapperExpression
    {
        /// <summary>
        /// structure func
        /// </summary>
        /// <param name="outType"></param>
        /// <param name="inTypes"></param>
        /// <param name="memberInitExpression"></param>
        /// <param name="parameterExpressionList"></param>
        public static void GetFunc(Type outType, Type[] inTypes, out MemberInitExpression memberInitExpression, out List<ParameterExpression> parameterExpressionList)
        {
            parameterExpressionList = new List<ParameterExpression>();
            List<MemberBinding> memberBindingList = new List<MemberBinding>();
            PropertyInfo[] propertyInfos = outType.GetProperties();
            Dictionary<string, PropertyInfo> outPropertyDic = propertyInfos.ToDictionary(t => t.Name, t => t);
            foreach (var inType in inTypes)
            {
                //创建一个 ParameterExpression 节点，该节点可用于标识表达式树中的参数或变量 用来访问源对象的访问
                ParameterExpression parameterExpression = Expression.Parameter(inType, inType.FullName);
                PropertyInfo[] inTypePpropertyInfos = inType.GetProperties();
                foreach (var inTypeInfo in inTypePpropertyInfos)
                {
                    if (inTypeInfo.GetCustomAttribute(typeof(DoNotMapperAttribute)) == null)
                    {
                        //获取是否指定了特定的转换属性
                        //first
                        string outPropertyDicKey = MapperAttribute.GetTargetName(inTypeInfo);
                        //second
                        if (string.IsNullOrEmpty(outPropertyDicKey) && outPropertyDic.Keys.Contains(inTypeInfo.Name))
                        {
                            //如果没有特定转换，找到目标对象的同名属性
                            outPropertyDicKey = inTypeInfo.Name;
                        }
                        //third
                        if (!string.IsNullOrEmpty(outPropertyDicKey) && outPropertyDic.Keys.Contains(outPropertyDicKey))
                        {
                            //创建一个表示访问属性的 MemberExpression  得到访问源属性的节点
                            MemberExpression property = Expression.Property(parameterExpression, inTypeInfo);
                            //使用属性访问器方法，创建一个表示成员初始化的 MemberAssignment 
                            MemberBinding memberBinding = Expression.Bind(outPropertyDic[outPropertyDicKey], property);
                            //目标成员的访问器的集合
                            memberBindingList.Add(memberBinding);
                            //移除该目标属性，该属性已找到初始化访问器
                            outPropertyDic.Remove(outPropertyDicKey);//remove property if has be valued
                        }
                    }
                }
                if (!parameterExpressionList.Exists(t => t.Name.Equals(parameterExpression.Name)))
                {
                    parameterExpressionList.Add(parameterExpression);
                }
            }
            //创建一个 MemberInitExpression    MemberInitExpression : 表示调用构造函数并初始化新对象的一个或多个成员。
            memberInitExpression = Expression.MemberInit(Expression.New(outType), memberBindingList.ToArray());
        }
    }

}
