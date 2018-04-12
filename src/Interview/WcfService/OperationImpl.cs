/*********************************************************
 * CopyRight: tiaoshuidenong. 
 * Author: tiaoshuidenong.
 * Address: wuhan
 * Create: 2018/4/11 16:39:48
 * Modify: 2018/4/11 16:39:48
 * Blog: http://www.cnblogs.com/tiaoshuidenong/
 * Description: 实现接口 
 *********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfService
{
    public class OperationImpl : IOperationInterface
    {
        public string Call(int num1, int num2)
        {
            return $"result:{num1 + num2}";
        }
    }
}
