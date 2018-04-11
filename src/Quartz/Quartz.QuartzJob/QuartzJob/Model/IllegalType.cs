
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.IO;
using System;
using log4net;
using QuartzJob.QuartzJobs;
using System.Collections.Generic;
namespace Citms.PIS.Model.Common
{

    public class ConfigManage
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(TestJob));
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
                _logger.Info(string.Format("应用新的配置文件成功"));
            }
            catch (Exception e)
            {
                _logger.Error(string.Format("应用新的配置文件异常，{0}", e.ToString()));
            }
        }
    }

    public class DbConfig
    {
        public string V2 { get; set; }
        public string V4 { get; set; }
        public string ST { get; set; }
        public string ET { get; set; }
        public string DisplayName { get; set; }
        public string ServiceName { get; set; }
        public string Range { get; set; }
        public string Sql { get; set; }
        public List<FormatOption> Format { get; set; }
    }
    public class FormatOption
    {
        public string Field { get; set; }
        public List<string> Values { get; set; }
    }
    /// <summary>
    /// 违法行为
    /// </summary>
    [Table("COMMON_ILLEGALTYPE")]
    public class IllegalType
    {
        /// <summary>
        /// 违法行为主键
        /// </summary>
        [Key, Column("ILLEGALTYPEID", TypeName = "VARCHAR2")]
        public string IllegalTypeID { get; set; }

        /// <summary>
        /// 违法编号（厂家违法编号）
        /// </summary>
        [Column("ILLEGALTYPENO", TypeName = "VARCHAR2"), Required]
        public string IllegalTypeNo { get; set; }

        /// <summary>
        /// 违法行为名称
        /// </summary>
        [Column("NAME"), Required]
        public string Name { get; set; }

        /// <summary>
        /// 罚款金额
        /// </summary>
        [Column("PENALTYAMOUNT"), Required]
        public int PenaltyaMount { get; set; }

        /// <summary>
        /// 扣分
        /// </summary>
        [Column("DEDUCTIONSCORE"), Required]
        public int Deductionscore { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Column("REMARK", TypeName = "VARCHAR2")]
        public string Remark { get; set; }

        /// <summary>
        /// 限行类违法打印在图片上面的水印内容
        /// </summary>
        [Column("WATERMARKCONTEN", TypeName = "VARCHAR2")]
        public string WaterMarkContent { get; set; }

        /// <summary>
        /// 标准违法代码
        /// </summary>
        [Column("ILLEGALCODE", TypeName = "VARCHAR2")]
        public string IllegalCode { get; set; }

        /// <summary>
        /// 是否删除(0表示未删除,1表示已删除)
        /// </summary>
        [Column("ISDELETED")]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 标准违法行为描述
        /// </summary>
        [NotMapped]
        public string IllegalContent { get; set; }
    }
}
