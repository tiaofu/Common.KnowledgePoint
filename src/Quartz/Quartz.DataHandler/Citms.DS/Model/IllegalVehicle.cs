using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Citms.Traffics.Punishment.Models
{
    /// <summary>
    /// 电子警察车辆违法处理实体
    /// </summary>
    [Table("PUNISH_ILLEGALVEHICLE")]
    public class IllegalVehicle
    {
        #region 属性
        /// <summary>
        /// 违法处理ID（主键）
        /// </summary>
        [Required]
        [Key, DisplayName("违法处理ID（主键）")]
        public Guid ProcessId { set; get; }
        /// <summary>
        /// 卡口编号
        /// </summary>
        [Required]
        [DisplayName("卡口编号")]
        public string TollgateId
        {
            get;
            set;
        }

        /// <summary>
        /// 卡口名称
        /// </summary>
        [DisplayName("卡口名称")]
        public string TollgateName
        {
            get;
            set;
        }

        /// <summary>
        /// 路口编号
        /// </summary>
        [Required]
        [DisplayName("路口编号")]
        public string CrossingId
        {
            get;
            set;
        }

        /// <summary>
        /// 路口名称
        /// </summary>
        [DisplayName("路口名称")]
        public string CrossingName
        {
            get;
            set;
        }

        /// <summary>
        /// 方向编号
        /// </summary>
        [Required]
        [DisplayName("方向编号")]
        public string DirectionId
        {
            get;
            set;
        }

        /// <summary>
        /// 方向名称
        /// </summary>
        [DisplayName("方向名称")]
        public string DirectionName
        {
            get;
            set;
        }

        /// <summary>
        /// 设备编号
        /// </summary>
        [DisplayName("设备编号")]
        public string AssetsId
        { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        [DisplayName("设备名称")]
        public string AssetsName
        { get; set; }

        /// <summary>
        /// 车道编号
        /// </summary>
        [DisplayName("车道编号")]
        public string LaneId
        {
            get;
            set;
        }

        /// <summary>
        /// 经过时间
        /// </summary>
        [Required]
        [DisplayName("经过时间")]
        public DateTime Timestamp
        {
            get;
            set;
        }

        /// <summary>
        /// 行驶速度
        /// </summary>
        [DisplayName("行驶速度")]
        public int? RunSpeed
        {
            get;
            set;
        }

        /// <summary>
        /// 车牌号码
        /// </summary>
        [Required]
        [DisplayName("车牌号码")]
        public string PlateNo
        {
            get;
            set;
        }

        /// <summary>
        /// 号牌颜色
        /// </summary>
        [Required]
        [DisplayName("号牌颜色")]
        public string PlateColorId
        {
            get;
            set;
        }

        /// <summary>
        /// 号牌类型
        /// </summary>
        [DisplayName("号牌类型")]
        public string PlateTypeId
        {
            get;
            set;
        }

        /// <summary>
        /// 车辆类型
        /// </summary>
        [DisplayName("车辆类型")]
        public string VehicleTypeId
        {
            get;
            set;
        }

        /// <summary>
        /// 车辆外形
        /// </summary>
        [DisplayName("车辆外形")]
        public string VehicleShape
        {
            get;
            set;
        }

        /// <summary>
        /// 车外廓长
        /// </summary>
        [DisplayName("车外廓长")]
        public string VehicleLength
        {
            get;
            set;
        }

        /// <summary>
        /// 车辆品牌
        /// </summary>
        [DisplayName("车辆品牌")]
        public string VehicleBrand
        {
            get;
            set;
        }

        /// <summary>
        /// 车身颜色
        /// </summary>
        [DisplayName("车身颜色")]
        public string VehicleColor
        {
            get;
            set;
        }

        /// <summary>
        /// 全景图路径
        /// </summary>
        [Required]
        [DisplayName("全景图路径")]
        public string PanoramaImageUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 特征图片路径
        /// </summary>
        [DisplayName("特征图片路径")]
        public string FeatureImageUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 违章标记
        /// </summary>
        [Required]
        [DisplayName("违章标记")]
        public string IllegalTypeId
        {
            get;
            set;
        }
        /// <summary>
        /// 标准违法代码
        /// </summary>
        [DisplayName("标准违法代码")]
        public string LegalizeIllegalTypeNo
        {
            get;
            set;
        }
        /// <summary>
        /// 区域代码
        /// </summary>
        [Required]
        [DisplayName("区域代码")]
        public string AreaCode
        {
            get;
            set;
        }

        /// <summary>
        /// 管辖单位
        /// </summary>
        [DisplayName("管辖单位")]
        public long DepartmentId
        {
            get;
            set;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        [DisplayName("创建时间")]
        public DateTime CreatedTime
        {
            get;
            set;
        }

        /// <summary>
        /// 审核时间
        /// </summary>
        [Required]
        [DisplayName("审核时间")]
        public DateTime? CheckTime
        {
            set;
            get;
        }

        /// <summary>
        /// 审核人
        /// </summary>
        [Required]
        [DisplayName("审核人")]
        public string Checker
        {
            set;
            get;
        }


        /// <summary>
        /// 废弃时间
        /// </summary>
        [Required]
        [DisplayName("废弃时间")]
        public DateTime? ScrapTime
        {
            set;
            get;
        }

        /// <summary>
        /// 废弃人
        /// </summary>
        [Required]
        [DisplayName("废弃人")]
        public string Scraper
        {
            set;
            get;
        }
        /// <summary>
        /// 废弃原因
        /// </summary>
        [Required]
        [DisplayName("废弃原因")]
        public int ScrapReason
        {
            set;
            get;
        }
        /// <summary>
        /// 数据状态
        /// </summary>
        [DisplayName("数据状态")]
        public int Status
        {
            set;
            get;
        }

        /// <summary>
        /// 图片是否识别
        /// </summary>
        [DisplayName("图片是否识别")]
        public bool Distinguishable
        {
            set;
            get;
        }
        #region 六合一信息
        /// <summary>
        /// 全国车辆库中的序号[六合一字段]
        /// </summary>
        [DisplayName("序号")]
        public string SerialId { get; set; }
        /// <summary>
        /// 检验有效期止[六合一字段]
        /// </summary>
        [DisplayName("检验有效期止")]
        public DateTime EffectiveDate { get; set; }
        /// <summary>
        /// 发动机号[六合一字段]
        /// </summary>
        [DisplayName("发动机号")]
        public string EngineCode { get; set; }
        /// <summary>
        /// 身份证明名称[六合一字段]
        /// </summary>
        [DisplayName("身份证明名称")]
        public string IdentificationName { get; set; }
        /// <summary>
        /// 身份证明号码[六合一字段]
        /// </summary>
        [DisplayName("身份证明号码")]
        public string IdentificationNo { get; set; }
        /// <summary>
        /// 手机号码[六合一字段]
        /// </summary>
        [DisplayName("手机号码")]
        public string OwnerMobileNumber { get; set; }
        /// <summary>
        /// 所有人姓名[六合一字段]
        /// </summary>
        [DisplayName("所有人姓名")]
        public string OwnerName { get; set; }
        /// <summary>
        /// 核定载客[六合一字段]
        /// </summary>
        [DisplayName("核定载客")]
        public short Busload { get; set; }
        /// <summary>
        /// 车辆品牌(外文)[六合一字段]
        /// </summary>
        [DisplayName("车辆品牌")]
        public string VehicleBrandAlias { get; set; }
        /// <summary>
        /// 车辆识别代号[六合一字段]
        /// </summary>
        [DisplayName("车辆识别代号")]
        public string VehicleIdentificationNo { get; set; }
        /// <summary>
        /// 车辆状态[六合一字段]
        /// </summary>
        [DisplayName("车辆状态")]
        public string VehicleStatus { get; set; }
        /// <summary>
        /// 发证机关[六合一字段]
        /// </summary>
        [DisplayName("发证机关")]
        public string LicenceIssuingAuthority { get; set; }
        /// <summary>
        /// 车辆品牌(中文)[六合一字段]
        /// </summary>
        [DisplayName("车辆品牌")]
        public string VehicleBrandInTraffics { get; set; }
        /// <summary>
        /// 车身颜色[六合一字段]
        /// </summary>
        [DisplayName("车身颜色")]
        public string VehicleColorInTraffics { get; set; }
        /// <summary>
        /// 车辆类型[六合一字段]
        /// </summary>
        [DisplayName("车辆类型")]
        public string VehicleModel { set; get; }
        /// <summary>
        /// 住所详细地址[六合一字段]
        /// </summary>
        [DisplayName("住所详细地址")]
        public string HomeAddress { get; set; }
        /// <summary>
        /// 住所行政区划[六合一字段]
        /// </summary>
        [DisplayName("住所行政区划")]
        public string HomeArea { get; set; }
        /// <summary>
        /// 联系电话[六合一字段]
        /// </summary>
        [DisplayName("联系电话")]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 车辆类型[六合一字段]
        /// </summary>
        [DisplayName("车辆类型")]
        public string VehicleType { get; set; }
        /// <summary>
        /// 号牌类型[六合一字段]
        /// </summary>
        [DisplayName("号牌类型")]
        public string PlateType { get; set; }
        /// <summary>
        /// 使用性质[六合一字段]
        /// </summary>
        [DisplayName("使用性质")]
        public string UseProperty { get; set; }
        #endregion

        /// <summary>
        /// 是否未识别车牌号码
        /// </summary>
        [DisplayName("是否未识别车牌号码")]
        public int IsUnrecognizedPlateNo { get; set; }


        /// <summary>
        /// 是否已填充六合一信息
        /// </summary>
        [DisplayName("是否已填充六合一信息")]
        public int InquiryFlag { get; set; }

        /// <summary>
        /// 上传人
        /// </summary>
        [DisplayName("上传人")]
        public string UploadPerson
        {
            get;
            set;
        }
        /// <summary>
        /// 上传时间
        /// </summary>
        [DisplayName("上传时间")]
        public DateTime? UploadTime
        {
            get;
            set;
        }

        /// <summary>
        /// 导入状态
        /// </summary>
        [DisplayName("导入状态")]
        public string UploadStatus
        {
            get;
            set;
        }
        /// <summary>
        /// 罚款金额
        /// </summary>
        [DisplayName("罚款金额")]
        public int PenaltyAmount
        {
            get;
            set;
        }
        /// <summary>
        /// 扣分
        /// </summary>
        [DisplayName("扣分")]
        public int DeductionScore
        {
            get;
            set;
        }
        /// <summary>
        /// 罚款标记:0表示未，1表示已，9表示不需要
        /// </summary>
        [DisplayName("罚款标记")]
        public int PenaltyAmountFlag
        {
            get;
            set;
        }
        /// <summary>
        /// 处理单位
        /// </summary>
        [DisplayName("处理单位")]
        public long? ProcessingDepartment
        {
            get;
            set;
        }
        /// <summary>
        /// 上传编号
        /// </summary>
        [DisplayName("上传编号")]
        public string UploadNo
        {
            get;
            set;
        }

        /// <summary>
        /// 上传失败次数
        /// </summary>
        [DisplayName("上传失败次数")]
        public int UploadFailCount
        {
            get;
            set;
        }

        /// <summary>
        /// 校验码
        /// </summary>
        [DisplayName("校验码")]
        public string CheckCode
        {
            get;
            set;
        }
        /// <summary>
        /// 是否需要对违法行为重复判断
        /// </summary>
        [DisplayName("是否需要对违法行为重复判断")]
        public int? RepeatCheck
        {
            get;
            set;
        }
        /// <summary>
        /// 修改时间
        /// </summary>
        [Required]
        [DisplayName("修改时间")]
        public DateTime? UpdateTime
        {
            set;
            get;
        }

        /// <summary>
        /// 车牌号码位置
        /// </summary>
        [DisplayName("车牌号码位置")]
        public string PlateNoLocation
        {
            get;
            set;
        }
        /// <summary>
        /// 备注
        /// </summary>
        [Required]
        [DisplayName("备注")]
        public string Remark
        {
            set;
            get;
        }

        /// <summary>
        /// 黄标审核人
        /// </summary>
        [DisplayName("黄标审核人")]
        public string YellowMarkChecker
        {
            get;
            set;
        }

        /// <summary>
        /// 黄标审核时间
        /// </summary>
        [DisplayName("黄标审核人")]
        public DateTime? YellowMarkCheckTime
        {
            get;
            set;
        }

        /// <summary>
        /// 预审核操作人
        /// </summary>
        [DisplayName("预审核操作人")]
        public string PreOperator
        {
            get;
            set;
        }

        /// <summary>
        /// 预审核操作时间
        /// </summary>
        [DisplayName("预审核操作时间")]
        public DateTime? PreOperateTime
        {
            get;
            set;
        }

        /// <summary>
        /// 预审核操作类型：0 审核，1废弃
        /// </summary>
        [DisplayName("预审核操作类型")]
        public int? PreOperateType
        {
            get;
            set;
        }

        ///// <summary>
        ///// 二次识别结果
        ///// </summary>
        //public int? SecondlyRecognitionResult { get; set; }

        ///// <summary>
        ///// 二次识别备注
        ///// </summary>
        //public string SecondlyRecognitionRemark { get; set; }


        #endregion
    }
}
