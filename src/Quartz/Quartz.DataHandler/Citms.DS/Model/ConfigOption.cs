using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace Citms.DailyStatistics
{
    public class ConfigManage
    {
        static string Path = AppDomain.CurrentDomain.BaseDirectory + "\\config.js";
        public static DbConfig _dbConfig;
        public static DbConfig dbConfig
        {
            get
            {
                if (_dbConfig != null)
                {
                    return _dbConfig;
                }
                else
                {
                    if (!File.Exists(Path))
                        return null;
                    string result = File.ReadAllText(Path);
                    return JsonConvert.DeserializeObject<DbConfig>(result);
                }
            }
            set
            {
                _dbConfig = value;
                string result = JsonConvert.SerializeObject(value, Formatting.Indented);
                File.WriteAllText(Path, result);
            }
        }
        public static void Reload()
        {
            try
            {
                string result = File.ReadAllText(Path);
                _dbConfig = JsonConvert.DeserializeObject<DbConfig>(result);
                LogHelper.WriteInfo(string.Format("应用新的配置文件成功"));
            }
            catch (Exception e)
            {
                LogHelper.Error(string.Format("应用新的配置文件异常，{0}", e.ToString()));
            }
        }
    }

    public class DbConfig
    {
        public string DbConn { get; set; }
        public string Sql { get; set; }
        public string MqAddress { get; set; }
        public string Port { get; set; }
        public string User { get; set; }
        public string Pwd { get; set; }
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
        public string DataType { get; set; }
    }
}
