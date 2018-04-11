using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using log4net;

namespace Citms
{
    public class LogHelper
    {
        private static readonly ILog info = LogManager.GetLogger("loginfo");
        private static readonly ILog error = LogManager.GetLogger("ErrorLog");

        public static void WriteInfo(string message)
        {
            info.Info(message);
        }

        public static void Error(string message)
        {
            error.Error(message);
        }

    }
}
