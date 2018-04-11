using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Work;
using System.ComponentModel.Composition;
using System.Threading;
namespace Common.Command
{
    /// <summary>
    /// 命令插件
    /// </summary>
    //[Export(typeof(IPlugin))]
    public class CommandPlugin : IPlugin
    {
        /// <summary>
        /// 命令
        /// </summary>        
        private IEnumerable<ICommand> Commands { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            Commands = MefConfig.ResolveMany<ICommand>();
            Thread thread = new Thread(Run);
            thread.Start();
        }

        public void Run()
        {
            while (true)
            {
                string strCommand = Console.ReadLine();
                if (string.IsNullOrEmpty(strCommand))
                {
                    continue;
                }
                else
                {
                    strCommand = strCommand.Trim();
                }
                ExcuteCommad(strCommand);
            }
        }

        public string ExcuteCommad(string strCommand)
        {
            ICommand command = null;
            var listCommand = strCommand.Split(' ').Where(e => !string.IsNullOrEmpty(e)).ToList();
            if (listCommand.Count == 0)
            {
                return string.Empty;
            }
            command = Commands.Where(p => p.Key.ToUpper() == listCommand[0].ToUpper()).FirstOrDefault();
            if (command != null)
            {
                listCommand.RemoveAt(0);
                try
                {
                    return command.Excute(listCommand.ToArray());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return ex.Message;
                }
            }
            else
            {
                string msg = string.Format("'{0}' 不是可识别的命令", listCommand[0]);
                Console.WriteLine(msg);
                return msg;
            }
        }
    }
}
