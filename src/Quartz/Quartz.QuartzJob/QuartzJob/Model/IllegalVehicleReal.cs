
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace QuartzJob.Punish
{
    ///<summary>
    ///违法记录表
    ///</summary>
    [Table("PUNISH_ILLEGALVEHICLE_REAL")]
    public class IllegalVehicleReal
    {
        /// <summary>
        /// 0000	未确认 从六合一车辆库查询不到对应的车辆信息
        /// </summary>
        public const string STATUS_NOTCONFIRMED = "0000";

        /// <summary>
        /// 0001	待审核 已从六合一车辆库查询到该车辆信息
        /// </summary>
        public const string STATUS_FORREVIEW = "0001";

        /// <summary>
        /// 0002	已驳回 被驳回的违法数据需要再次审核
        /// </summary>
        public const string STATUS_REJECTED = "0002";

        /// <summary>
        /// 0003	待二审 违法数据初次审核后的状态
        /// </summary>
        public const string STATUS_FORSECONDREVIEW = "0003";

        /// <summary>
        /// 1001	已审核 二审通过或无需二审的初审数据
        /// </summary>
        public const string STATUS_REVIEWPASSED = "1001";

        /// <summary>
        /// 1002	待上传 已审核后上传排队中的数据
        /// </summary>
        public const string STATUS_FORUPLOAD = "1002";

        /// <summary>
        /// 1003	上传中 正在上传六合一的违法数据
        /// </summary>
        public const string STATUS_UPLOADING = "1003";

        /// <summary>
        /// 1004	已上传 已经上传六合一的违法数据
        /// </summary>
        public const string STATUS_UPLOADED = "1004";

        /// <summary>
        /// 1005	上传失败 六合一上传失败的数据
        /// </summary>
        public const string STATUS_UPLOADFAILED = "1005";

        /// <summary>
        /// 2001	已废弃 已被确认为废弃的数据
        /// </summary>
        public const string STATUS_SCRAPPED = "2001";

        /// <summary>
        /// 3000	已过期 已被确认为已过期的数据
        /// </summary>
        public const string STATUS_OVERDAY = "3000";

        [NotMapped]
        public int RowNum { get; set; }
        ///<summary>
        ///违法ID-主键
        ///</summary>
        //[DataColumn(PrimaryKey = true)]
        [Key, Column("PROCESSID", TypeName = "VARCHAR2")]
        public string ProcessId { get; set; }

        ///<summary>
        ///点位编号
        ///</summary>
        [Column("SPOTTINGID", TypeName = "VARCHAR2"), Required]
        public string SpottingId { get; set; }

        ///<summary>
        ///点位名号（违法地点）
        ///</summary>
        [Column("SPOTTINGNAME")]
        public string SpottingName { get; set; }

        ///<summary>
        ///方向ID
        ///</summary>
        [Column("DIRECTIONID", TypeName = "VARCHAR2"), Required]
        public string DirectionId { get; set; }

        ///<summary>
        ///方向名称
        ///</summary>
        [Column("DIRECTIONNAME")]
        public string DirectionName { get; set; }

        ///<summary>
        ///车道编号
        ///</summary>
        [Column("LANENO", TypeName = "VARCHAR2")]
        public string LaneNo { get; set; }

        ///<summary>
        ///违法时间
        ///</summary>
        [Column("OCCURTIME", TypeName = "DATE")]
        public DateTime? OccurTime { get; set; }

        ///<summary>
        ///运行速度
        ///</summary>
        [Column("RUNSPEED")]
        public int? RunSpeed { get; set; }

        ///<summary>
        ///车牌号码
        ///</summary>
        [Column("PLATENO", TypeName = "VARCHAR2")]
        [Required]
        public string PlateNo { get; set; }

        ///<summary>
        ///号牌颜色代码（字典表 Kind 为1008 类型 存放 DictionaryNo 字段值）
        ///</summary>
        [Column("PLATECOLOR", TypeName = "VARCHAR2")]
        [Required]
        public string PlateColor { get; set; }     

        ///<summary>
        ///车辆品牌
        ///</summary>
        [Column("VEHICLEBRAND", TypeName = "VARCHAR2")]
        public string VehicleBrand { get; set; }

        ///<summary>
        ///车身颜色（字典表 Kind 为 1011 类型 存放 DictionaryNo 字段值）A 白 B 灰等 此字段可能会是组合值如白灰色
        ///</summary>
        [Column("VEHICLECOLOR", TypeName = "VARCHAR2")]
        public string VehicleColor { get; set; }

        ///<summary>
        ///违法图片地址URL1
        ///</summary>
        [Column("PANORAMAIMAGEURL1", TypeName = "VARCHAR2"), Required]
        public string PanoramaimageUrl1 { get; set; }

        ///<summary>
        ///违法图片地址URL2
        ///</summary>
        [Column("PANORAMAIMAGEURL2", TypeName = "VARCHAR2")]
        public string PanoramaimageUrl2 { get; set; }

        ///<summary>
        ///违法图片地址URL3
        ///</summary>
        [Column("PANORAMAIMAGEURL3", TypeName = "VARCHAR2")]
        public string PanoramaimageUrl3 { get; set; }

        ///<summary>
        ///违法图片地址URL4
        ///</summary>
        [Column("PANORAMAIMAGEURL4", TypeName = "VARCHAR2")]
        public string PanoramaimageUrl4 { get; set; }

        ///<summary>
        ///特征图片URL
        ///</summary>
        [Column("FEATUREIMAGEURL", TypeName = "VARCHAR2")]
        public string FeatureimageUrl { get; set; }

        ///<summary>
        ///违法类型NO
        ///</summary>
        [Column("ILLEGALTYPENO", TypeName = "VARCHAR2"), Required]
        public string IllegalTypeNo { get; set; }

        ///<summary>
        ///录入人员
        ///</summary>
        [Column("INPUTER", TypeName = "VARCHAR2")]
        public string Inputer { get; set; }

        ///<summary>
        ///录入时间
        ///</summary>
        [Column("INPUTTIME", TypeName = "DATE")]
        public DateTime? InputTime { get; set; }

        ///<summary>
        ///道路所在行政区划代码
        ///</summary>
        [Column("AREACODE", TypeName = "CHAR")]
        public string AreaCode { get; set; }

        ///<summary>
        ///采集单位（管理部门）
        ///</summary>
        [Column("DEPARTMENTID", TypeName = "VARCHAR2")]
        public string DepartmentId { get; set; }

        ///<summary>
        ///创建时间
        ///</summary>
        [Column("CREATEDTIME", TypeName = "DATE")]
        public DateTime? CreatedTime { get; set; }

        ///<summary>
        ///检录时间
        ///</summary>
        [Column("CHECKTIME", TypeName = "DATE")]
        public DateTime? CheckTime { get; set; }

        ///<summary>
        ///检录人
        ///</summary>
        [Column("CHECKER", TypeName = "VARCHAR2")]
        public string Checker { get; set; }

        ///<summary>
        ///废弃时间
        ///</summary>
        [Column("SCRAPTIME", TypeName = "DATE")]
        public DateTime? ScrapTime { get; set; }

        ///<summary>
        ///废弃人
        ///</summary>
        [Column("SCRAPER", TypeName = "VARCHAR2")]
        public string Scraper { get; set; }

        ///<summary>
        ///废弃理由编号-PUNISH_SCRAPREASON 表的主键）
        ///</summary>
        [Column("SCRAPREASONNO", TypeName = "VARCHAR2")]
        public string ScrapreasonNo { get; set; }

        ///<summary>
        ///未确定-1,未录入0,黄标车未确认1,未审核 10, 已废弃30,已审核40,已违法45,等待上传46,检测失败50,已上传60,上传失败70,
        ///</summary>
        [Column("STATUS", TypeName = "CHAR")]
        [Required]
        public string Status { get; set; }

        ///<summary>
        ///处理状态中文
        ///</summary>
        [NotMapped]
        public string StatusCn { get; set; }

        ///<summary>
        ///是否需要再次验证限行规则（0 不需要 1 需要）常规电子警察违法不需要，通过规则判断出来的则需要
        ///</summary>
        [Column("INQUIRYFLAG")]
        public int? InquiryFlag { get; set; }

        ///<summary>
        ///是否为未被识别出来的号牌（无牌、XXXXXXX 等属于此类）0 否 1 是
        ///</summary>
        [Column("ISUNRECOGNIZEDPLATENO")]
        public int? IsUnrecognizedPlateNo { get; set; }

        ///<summary>
        ///车辆流水号-车辆登记库中的唯一号
        ///</summary>
        [Column("SERIALID", TypeName = "VARCHAR2")]
        public string SerialId { get; set; }

        ///<summary>
        ///车辆有效截止日期
        ///</summary>
        [Column("EFFECTIVEDATE", TypeName = "DATE")]
        public DateTime? EffectiveDate { get; set; }

        ///<summary>
        ///发动机号
        ///</summary>
        [Column("ENGINECODE", TypeName = "VARCHAR2")]
        public string EngineCode { get; set; }

        ///<summary>
        ///证件名称（A 身份证 B 暂住证）
        ///</summary>
        [Column("IDENTIFICATIONNAME", TypeName = "VARCHAR2")]
        public string IdentificationName { get; set; }

        ///<summary>
        ///证件号码
        ///</summary>
        [Column("IDENTIFICATIONNO", TypeName = "VARCHAR2")]
        public string IdentificationNo { get; set; }

        ///<summary>
        ///车主手机号码
        ///</summary>
        [Column("OWNERMOBILENUMBER", TypeName = "VARCHAR2")]
        public string OwnerMobileNumber { get; set; }

        ///<summary>
        ///车辆所有人
        ///</summary>
        [Column("OWNERNAME", TypeName = "VARCHAR2")]
        public string OwnerName { get; set; }

        ///<summary>
        ///车辆载人量
        ///</summary>
        [Column("BUSLOAD", TypeName = "VARCHAR2")]
        public string BusLoad { get; set; }

        ///<summary>
        ///车辆品牌别名（如英文品牌名称）
        ///</summary>
        [Column("VEHICLEBRANDALIAS", TypeName = "VARCHAR2")]
        public string VehicleBrandAlias { get; set; }

        ///<summary>
        ///车辆识别代号
        ///</summary>
        [Column("VEHICLEIDENTIFICATIONNO", TypeName = "VARCHAR2")]
        public string VehicleIdentificationNo { get; set; }

        ///<summary>
        ///车辆状态-车辆登记库中的状态（字典表 Kind 为 1012 类型 存放 DictionaryNo 字段值）A 正常 B 转出 C 被盗抢 E 注销 G 违法未处理 等 此字段可能会是组合值如CG
        ///</summary>
        [Column("VEHICLESTATUS", TypeName = "VARCHAR2")]
        public string VehicleStatus { get; set; }

        ///<summary>
        ///车辆发证机关（一般为车牌号码的前2位，直辖市除外，直辖市一般为车牌第一个字符加 A）
        ///</summary>
        [Column("LICENCEISSUINGAUTHORITY", TypeName = "VARCHAR2")]
        public string LicenceIssuingAuthority { get; set; }

        ///<summary>
        ///车辆品牌中文名称
        ///</summary>
        [Column("VEHICLEBRANDINTRAFFICS", TypeName = "VARCHAR2")]
        public string VehicleBrandInTraffics { get; set; }

        ///<summary>
        ///车身颜色
        ///</summary>
        [Column("VEHICLECOLORINTRAFFICS", TypeName = "VARCHAR2")]
        public string VehicleColorInTraffics { get; set; }

        ///<summary>
        ///车辆型号
        ///</summary>
        [Column("VEHICLEMODEL", TypeName = "VARCHAR2")]
        public string VehicleModel { get; set; }

        ///<summary>
        ///家庭地址
        ///</summary>
        [Column("HOMEADDRESS", TypeName = "VARCHAR2")]
        public string HomeAddress { get; set; }

        ///<summary>
        ///车主家庭住址所在行政区划
        ///</summary>
        [Column("HOMEAREA", TypeName = "VARCHAR2")]
        public string HomeArea { get; set; }

        ///<summary>
        ///联系电话
        ///</summary>
        [Column("PHONENUMBER", TypeName = "VARCHAR2")]
        public string PhoneNumber { get; set; }

        ///<summary>
        ///车辆类型代码（字典表 Kind 为 3 类型 存放 DictionaryNo 字段值）
        ///</summary>
        [Column("VEHICLETYPE", TypeName = "VARCHAR2")]
        public string VehicleType { get; set; }

        ///<summary>
        ///号牌类型（字典表 Kind 为 5 类型 存放 DictionaryNo 字段值）
        ///</summary>
        [Column("PLATETYPE", TypeName = "VARCHAR2")]
        public string PlateType { get; set; }

        ///<summary>
        ///号牌类型中文
        ///</summary>
        [NotMapped]
        public string PlateTypeCn { get; set; }

        ///<summary>
        ///使用性质（字典表 Kind 为 1013 类型 存放 DictionaryNo 字段值）A 非运营 B 公路客运 C 公交客运 D 出租客运 E 旅游客运 等
        ///</summary>
        [Column("USEPROPERTY", TypeName = "VARCHAR2")]
        public string UseProperty { get; set; }

        ///<summary>
        ///导入外部系统后生成的编号
        ///</summary>
        [Column("FOREIGNVOUCHERNO", TypeName = "VARCHAR2")]
        public string ForeIgnvoucherNo { get; set; }

        ///<summary>
        ///上传人
        ///</summary>
        [Column("UPLOADPERSON", TypeName = "VARCHAR2")]
        public string UploadPerson { get; set; }

        ///<summary>
        ///上传时间
        ///</summary>
        [Column("UPLOADTIME", TypeName = "DATE")]
        public DateTime? UploadTime { get; set; }

        ///<summary>
        ///上传状态
        ///</summary>
        [Column("UPLOADSTATUS")]
        public string UploadStatus { get; set; }

        ///<summary>
        ///备注
        ///</summary>
        [Column("REMARK", TypeName = "VARCHAR2")]
        public string Remark { get; set; }

        ///<summary>
        ///罚款金额
        ///</summary>
        [Column("PENALTYAMOUNT")]
        public int? PenaltyAmount { get; set; }

        ///<summary>
        ///违法扣分值
        ///</summary>
        [Column("DEDUCTIONSCORE")]
        public int? DeductionScore { get; set; }

        ///<summary>
        ///交款标记（0 未交款 1 已经交款 9 不需交款）
        ///</summary>
        [Column("PENALTYAMOUNTFLAG")]
        public int? PenaltyAmountFlag { get; set; }

        ///<summary>
        ///处理部门
        ///</summary>
        [Column("PROCESSINGDEPARTMENT", TypeName = "VARCHAR2")]
        public string ProcessingDepartment { get; set; }

        ///<summary>
        ///标准违法代码
        ///</summary>
        [Column("LEGALIZEILLEGALTYPENO", TypeName = "VARCHAR2")]
        public string LegalizeIllegalTypeNo { get; set; }

        ///<summary>
        ///上传成功返回的违法编号
        ///</summary>
        [Column("UPLOADNO", TypeName = "VARCHAR2")]
        public string UploadNo { get; set; }

        ///<summary>
        ///检录代码
        ///</summary>
        [Column("CHECKCODE", TypeName = "VARCHAR2")]
        public string CheckCode { get; set; }

        ///<summary>
        ///是否需要重复检测（0 否 1 需要）
        ///</summary>
        [Column("REPEATCHECK")]
        public int? RepeatCheck { get; set; }

        ///<summary>
        ///车牌号码所在图片的坐标值
        ///</summary>
        [Column("PLATENOLOCATION")]
        public string PlatenoLocation { get; set; }

        ///<summary>
        ///是否为可辩认的（0 否 1 可以）
        ///</summary>
        [Column("DISTINGUISHABLE")]
        public int? DistingUishable { get; set; }

        ///<summary>
        ///上传时间
        ///</summary>
        [Column("UPDATETIME", TypeName = "DATE")]
        public DateTime? UpdateTime { get; set; }

        ///<summary>
        ///抓拍设备编号
        ///</summary>
        [Column("ASSETSNO", TypeName = "VARCHAR2")]
        public string AssetsNo { get; set; }

        ///<summary>
        ///设备名称
        ///</summary>
        [Column("ASSETSNAME", TypeName = "VARCHAR2")]
        public string AssetsName { get; set; }

        ///<summary>
        ///警察检录人员
        ///</summary>
        [Column("POLICECHECKER")]
        public string PoliceChecker { get; set; }

        ///<summary>
        ///警察检录时间
        ///</summary>
        [Column("POLICECHECKTIME", TypeName = "DATE")]
        public DateTime? PoliceCheckTime { get; set; }

        ///<summary>
        ///上传失败次数
        ///</summary>
        [Column("UPLOADFAILCOUNT")]
        public int? UploadFailCount { get; set; }

        ///<summary>
        ///处罚标记
        ///</summary>
        [Column("PUNISHFLAG")]
        public int? PunishFlag { get; set; }

        ///<summary>
        ///视频地址
        ///</summary>
        [Column("VIDEOURL", TypeName = "VARCHAR2")]
        public string VideoUrl { get; set; }

        ///<summary>
        ///是否为视频（0 否 1 视频）
        ///</summary>
        [Column("ISVIDEO")]
        public int? IsVideo { get; set; }

        ///<summary>
        ///采集方式（字典表 Kind 为 1004 类型 存放 DictionaryNo 字段值）
        ///</summary>
        [Column("COLLECTMODE", TypeName = "CHAR")]
        public string CollectMode { get; set; }

        ///<summary>
        ///车辆长度
        ///</summary>
        [Column("VEHICLELENGTH")]
        public double? VehicleLength { get; set; }

        [Column("EXPORTEDFLAG")]
        public int? ExportedFlag { get; set; }

        [Column("EXPORTEDTIME", TypeName = "DATE")]
        public DateTime? ExportedTime { get; set; }

        /// <summary>
        /// 标记是否常规违法数据
        /// </summary>
        [Column("ISCOMMON")]
        public int? IsCommon { get; set; }

        /// <summary>
        /// 废弃方式 0 单条废弃  1 批量废弃
        /// </summary>
        [Column("SCRAPTYPE", TypeName = "VARCHAR2")]
        public string ScrapType { get; set; }

        /// <summary>
        /// 比中的限行方案
        /// </summary>
        [Column("RULEID", TypeName = "VARCHAR2")]
        public string RuleId { get; set; }

        /// <summary>
        /// 点位方向名称
        /// </summary>
        [NotMapped]
        public string SpottingDirectionName
        {
            get
            {
                return SpottingName + "-" + DirectionName;
            }
        }

        /// <summary>
        /// 违法类型描述
        /// </summary>
        [NotMapped]
        public string IllegalTypeRemark { get; set; }

        /// <summary>
        /// 驳回人
        /// </summary>
        [NotMapped]
        public string Rejecter { get; set; }

        /// <summary>
        /// 驳回时间
        /// </summary>
        [NotMapped]
        public DateTime? RejectTime { get; set; }

        /// <summary>
        ///采集单位名称
        /// </summary>
        [NotMapped]
        public string CjDepartmentName { get; set; }

        /// <summary>
        /// 处理单位名称
        /// </summary>
        [NotMapped]
        public string ProcessingDepartmentName { get; set; }

        ///<summary>
        ///使用性质中文
        ///</summary>
        [NotMapped]
        public string UsePropertyCn { get; set; }

        ///<summary>
        ///废弃理由说明
        ///</summary>
        [NotMapped]
        public string ScrapreasonName { get; set; }

        ///<summary>
        ///限行方案名称
        ///</summary>
        [NotMapped]
        public string SchemeName { get; set; }

        ///<summary>
        ///限行方案备注
        ///</summary>
        [NotMapped]
        public string SchemeRemark { get; set; }

        /// <summary>
        /// 数据是否沉淀标记(0表示未沉淀,1表示沉淀)
        /// </summary>
        [Column("ISSEDIMENT")]
        public int? IsSediment { get; set; }

        ///<summary>
        ///限速速度
        ///</summary>
        [Column("LIMITEDSPEED")]
        public int? LimitedSpeed { get; set; }

        /// <summary>
        /// 上传转换违法代码时间(违停转换)
        /// </summary>
        [Column("CONVERTEDTIME")]
        public DateTime? ConvertedTime { get; set; }

        /// <summary>
        /// 上传时使用的违法代码(违停转换)
        /// </summary>
        [Column("CONVERTEDILLEGALTYPENO", TypeName = "VARCHAR2")]
        public string ConvertedIllegalTypeNo { get; set; }
    }

    /// <summary>
    /// 违法审核步骤枚举
    /// </summary>
    public enum IllegalApproveStatus
    {
        /// <summary>
        /// 废弃
        /// </summary>
        [Description("废弃")]
        Abandon = 1,
        /// <summary>
        /// 驳回
        /// </summary>
        [Description("驳回")]
        Reject = 2,
        /// <summary>
        /// 一审
        /// </summary>
        [Description("一审")]
        FirstApprove = 3,
        /// <summary>
        /// 二审
        /// </summary>
        [Description("二审")]
        SecondApprove = 4,
        /// <summary>
        /// 车辆信息查询
        /// </summary>
        [Description("车辆信息查询")]
        VehicleSearch = 5,
        /// <summary>
        /// 修正车辆信息
        /// </summary>
        [Description("修正车辆信息")]
        ModifyIllegalVehicleInfo = 6,
        /// <summary>
        /// 违法上传
        /// </summary>
        [Description("违法上传")]
        Upload = 7,
        /// <summary>
        /// 暂不处理
        /// </summary>
        [Description("暂不处理")]
        UnHandler = 8
    }
}