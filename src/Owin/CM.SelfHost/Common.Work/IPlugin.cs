using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
namespace Common.Work
{
    /// <summary>
    /// 插件接口，统一的插件标准接口
    /// </summary>
    [InheritedExport]
    public interface IPlugin
    {
        void Init();
    }
}
