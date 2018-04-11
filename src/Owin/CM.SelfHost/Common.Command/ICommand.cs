using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;

namespace Common.Command
{
    /// <summary>
    /// 命令接口
    /// </summary>
    [InheritedExport]
    public interface ICommand
    {
        string Key { get; }
        string Excute(string [] command);
    }
}
