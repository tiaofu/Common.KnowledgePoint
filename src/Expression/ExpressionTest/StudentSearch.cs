﻿/*********************************************************
 * CopyRight: tiaoshuidenong. 
 * Author: tiaoshuidenong
 * Address: wuhan
 * Create: 2018-04-10 17:44:16
 * Modify: 2018-04-10 17:44:16
 * Description: 
 *********************************************************/
using ExpressionLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionTest
{
    /// <summary>
    /// 学生查询类
    /// </summary>
    public class StudentSearch
    {
        [SQ("Name", OperationType.LK)]
        public string Name { get; set; }

        [SQ("Age", OperationType.GTE)]
        public int StartAge { get; set; }

        [SQ("Age", OperationType.LTE)]
        public int EndAge { get; set; }
    }
}
