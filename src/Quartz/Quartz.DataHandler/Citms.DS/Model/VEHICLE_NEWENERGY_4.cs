
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Citms.PIS.Model.Vehicle
{
    [Table("VEHICLE_NEWENERGY")]
    public class VEHICLE_NEWENERGY_4
    {
        /// <summary>
        /// 汽车制造商名称
        /// </summary>
        // TODO：未定义主键无法使用，暂时定义一个，避免出现问题，后续请修改
        // by chenf 2016-06-20
        [Column("A")]
        public string A { get; set; }

        /// <summary>
        /// 六合一车辆登记序号
        /// </summary>
        [Column("B", TypeName = "VARCHAR2")]
        public string B { get; set; }

        /// <summary>
        /// 车型描述
        /// </summary>
        [Column("C")]
        public string C { get; set; }

        /// <summary>
        /// 车型描述
        /// </summary>
        [Key, Column("ID", TypeName = "VARCHAR2")]
        public string ID { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [Column("CREATEDDATE")]
        public DateTime? CREATEDDATE{ get; set; }
    }
}
