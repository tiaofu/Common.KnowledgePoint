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

namespace ExpressionLinq
{
    /// <summary>
    /// 查询的自定义属性类
    /// 这里只是将查询的对象封装到属性 
    /// 真正的做法是将查询通过UI封装，Service进行linq查询，而非通过对象属性的方式
    /// </summary>
    public class SQAttribute : Attribute
    {
        /// <summary>
        /// 属性名称，用于对比查询
        /// </summary>
        private string _pName;
        /// <summary>
        /// 操作类型
        /// </summary>
        private OperationType _opType;

        public string PName
        {
            get { return _pName; }
        }
        public OperationType OpType
        {
            get { return _opType; }
        }

        public SQAttribute(string PName, OperationType OpType)
        {
            _pName = PName;
            _opType = OpType;
        }

        /// <summary>
        /// 不是查询的条件时调用此构造函数  参数值=OperationType.None
        /// </summary>
        /// <param name="operation"></param>
        public SQAttribute(OperationType OpType)
        {
            _opType = OpType;
        }
    }
}
