
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Citms.PIS.Model.Common
{
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
        [Column("ILLEGALTYPENO", TypeName = "VARCHAR2"),Required]
        public string IllegalTypeNo { get; set; }

        /// <summary>
        /// 违法行为名称
        /// </summary>
        [Column("NAME"),Required]
        public string Name { get; set; }

        /// <summary>
        /// 罚款金额
        /// </summary>
        [Column("PENALTYAMOUNT"),Required]
        public int PenaltyaMount { get; set; }

        /// <summary>
        /// 扣分
        /// </summary>
        [Column("DEDUCTIONSCORE"),Required]
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
