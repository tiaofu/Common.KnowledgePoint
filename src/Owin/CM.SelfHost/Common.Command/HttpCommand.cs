using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Command
{
    /// <summary>
    /// http 打开网站命令
    /// </summary>
    public class HttpCommand : ICommand
    {
        public string Key
        {
            get
            {
                return "http";
            }
        }
        public string Excute(string[] command)
        {
            System.Diagnostics.Process.Start(url + "/index.html");
            return string.Empty;
        }
    }
}
