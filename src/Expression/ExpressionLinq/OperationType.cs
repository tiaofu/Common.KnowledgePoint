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
using System.Text;

namespace ExpressionLinq
{
    /// <summary>
    /// 查询操作类
    /// </summary>
    public enum OperationType
    {
        /// <summary>
        /// 不进行查询
        /// </summary>
        None,
        /// <summary>
        /// 比较该查询属性的值是否与元数据数据的值相等 即sql中=
        /// </summary>
        EQ,//Equal,
        /// <summary>
        /// 比较元数据数据的值是否包含该查询属性的值  即sql中like
        /// </summary>
        LK,//Like,
        /// <summary>
        /// 大于
        /// </summary>
        GT,//GreaterThan,
        /// <summary>
        /// 小于
        /// </summary>
        LT,//LessThan,
        /// <summary>
        /// >=
        /// </summary>
        GTE,//GreaterThanOrEqual,
        /// <summary>
        /// <=
        /// </summary>
        LTE,//LessThanOrEqual
    }
}
