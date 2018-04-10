/*********************************************************
 * CopyRight: tiaoshuidenong. 
 * Version: 5.0.0
 * Author: tiaoshuidenong
 * Address: Earth
 * Create: 2018-04-10 17:02
 * Modify: 2018-04-10 17:02
 * GitHub: 
 * Description: 
 *********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionLinq
{
    /// <summary>
    /// linq 查询方法拓展
    /// </summary>
    public static class QueryExtend
    {
        public static IQueryable<TSource> Search<TSource, T>(this IQueryable<TSource> queryList, T searchOptions)
        {
            return queryList.Where(Search<TSource, T>(searchOptions));
        }

        private static Expression<Func<TSource, bool>> Search<TSource, T>(T searchOptionEntity)
        {
            var dataSouceType = typeof(TSource);  //数据源列表元素对象的类型
            var dataSource = new
            {
                Type = dataSouceType,  //数据源列表元素对象的类型
                Properties = dataSouceType.GetProperties(),  //数据源列表元素对象的属性集合
            };

            //List<string> sourcePropertyName = sourceProperties.Select(p => p.Name).ToList();

            PropertyInfo[] searchProperties = searchOptionEntity.GetType().GetProperties();  //查询选择器对象的属性集合

            var pe = Expression.Parameter(dataSource.Type, "p"); //创建一个 ParameterExpression 节点，该节点可用于标识表达式树中的参数或变量
            var expression = Expression.Equal(Expression.Constant(true), Expression.Constant(true));

            //遍历查询选择器对象的属性集合
            foreach (var property in searchProperties)
            {
                var propertySearchAttribute = property.GetCustomAttributes(true)[0] as SQAttribute;  //获取查询选择器属性的自定义特性对象
                var propertySearchVlaue = property.GetValue(searchOptionEntity, null);  //获取查询选择器属性的值
                var propertySearchAttributeName = propertySearchAttribute.PName;  //获取查询选择器属性的自定义特性对象的对比查询的字段名称

                //查询选择器中的该属性的自定义的对比查询的字段名称 in 数据源列表对象的属性集合 && 查询选择器中的该属性是查询的条件  &&  查询选择器该属性的值!=null或者""
                if (Array.Exists(dataSource.Properties, p => p.Name == propertySearchAttributeName) && propertySearchAttribute.OpType != OperationType.None && propertySearchVlaue != null && propertySearchVlaue != (object)string.Empty)
                {
                    var propertyReference = Expression.Property(pe, propertySearchAttributeName);
                    var sourcePropertyType = dataSource.Properties.FirstOrDefault(p => p.Name == propertySearchAttributeName).PropertyType;  //获取数据源列表元素对象的单个属性的属性类型
                    ConstantExpression constantReference = null;
                    Expression Expr = null;

                    bool isGenericType = sourcePropertyType.IsGenericType && sourcePropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);  //搜索sourcePropertyType是否可为空
                    if (isGenericType)
                        constantReference = Expression.Constant(Convert.ChangeType(propertySearchVlaue, Nullable.GetUnderlyingType(sourcePropertyType)), sourcePropertyType);  //如果可为空类型，则将propertySearchVlaue的类型设置为可为空类型
                    else
                        constantReference = Expression.Constant(Convert.ChangeType(propertySearchVlaue, sourcePropertyType));

                    //根据查询选择器中该属性的查询条件进行不同的操作
                    switch (propertySearchAttribute.OpType)
                    {
                        case OperationType.EQ:
                            Expr = Expression.Equal(propertyReference, constantReference);
                            break;
                        case OperationType.GT:
                            Expr = Expression.GreaterThan(propertyReference, constantReference);
                            break;
                        case OperationType.LT:
                            Expr = Expression.LessThan(propertyReference, constantReference);
                            break;
                        case OperationType.GTE:
                            Expr = Expression.GreaterThanOrEqual(propertyReference, constantReference);
                            break;
                        case OperationType.LTE:
                            Expr = Expression.LessThanOrEqual(propertyReference, constantReference);
                            break;
                        case OperationType.LK:
                            Expr = Expression.Call(propertyReference, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), constantReference);
                            break;
                        default: break;

                    }

                    expression = Expression.AndAlso(expression, Expr);  //最终的查询条件
                }
            }
            Expression<Func<TSource, bool>> lambda = Expression.Lambda<Func<TSource, bool>>(expression, pe);
            return lambda;
        }

    }
}
