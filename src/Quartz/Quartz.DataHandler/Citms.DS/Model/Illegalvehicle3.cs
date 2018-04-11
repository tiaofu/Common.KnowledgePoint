//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity.ModelConfiguration;

//namespace Citms.Traffics.Punishment.Models
//{
//    ///<summary>
//    ///
//    ///</summary>
//    [Table("PUNISH_ILLEGALVEHICLE")]
//    public class  IllegalVehicle
//    {
//    ///<summary>
//        /// Processid
//        ///</summary> 
//        [Key,Column("ProcessId", TypeName = "RAW")]
//        public byte[]  Processid { get; set; }
	      
//        ///<summary>
//        /// Tollgateid
//        ///</summary> 
//        [Column("TollgateId", TypeName = "VARCHAR2")]
//        public string Tollgateid { get; set; }
	      
//        ///<summary>
//        /// Tollgatename
//        ///</summary> 
//        [Column("TollgateName", TypeName = "VARCHAR2")]
//        public string Tollgatename { get; set; }
	      
//        ///<summary>
//        /// Crossingid
//        ///</summary> 
//        [Column("CrossingId", TypeName = "VARCHAR2")]
//        public string Crossingid { get; set; }
	      
//        ///<summary>
//        /// Crossingname
//        ///</summary> 
//        [Column("CrossingName", TypeName = "VARCHAR2")]
//        public string Crossingname { get; set; }
	      
//        ///<summary>
//        /// Directionid
//        ///</summary> 
//        [Column("DirectionId", TypeName = "VARCHAR2")]
//        public string Directionid { get; set; }
	      
//        ///<summary>
//        /// Directionname
//        ///</summary> 
//        [Column("DirectionName", TypeName = "VARCHAR2")]
//        public string Directionname { get; set; }
	      
//        ///<summary>
//        /// Laneid
//        ///</summary> 
//        [Column("LaneId", TypeName = "VARCHAR2")]
//        public string Laneid { get; set; }
	      
//        ///<summary>
//        /// Timestamp
//        ///</summary> 
//        [Column("Timestamp", TypeName = "DATE")]
//        public DateTime?  Timestamp { get; set; }
	      
//        ///<summary>
//        /// Runspeed
//        ///</summary> 
//        [Column("RunSpeed", TypeName = "NUMBER")]
//        public int?  Runspeed { get; set; }
	      
//        ///<summary>
//        /// Plateno
//        ///</summary> 
//        [Column("PlateNo", TypeName = "VARCHAR2")]
//        public string Plateno { get; set; }
	      
//        ///<summary>
//        /// Platecolorid
//        ///</summary> 
//        [Column("PlateColorId", TypeName = "VARCHAR2")]
//        public string Platecolorid { get; set; }
	      
//        ///<summary>
//        /// Platetypeid
//        ///</summary> 
//        [Column("PlateTypeId", TypeName = "VARCHAR2")]
//        public string Platetypeid { get; set; }
	      
//        ///<summary>
//        /// Vehicletypeid
//        ///</summary> 
//        [Column("VehicleTypeId", TypeName = "VARCHAR2")]
//        public string Vehicletypeid { get; set; }
	      
//        ///<summary>
//        /// Vehicleshape
//        ///</summary> 
//        [Column("VehicleShape", TypeName = "VARCHAR2")]
//        public string Vehicleshape { get; set; }
	      
//        ///<summary>
//        /// Vehiclelength
//        ///</summary> 
//        [Column("VehicleLength", TypeName = "VARCHAR2")]
//        public string Vehiclelength { get; set; }
	      
//        ///<summary>
//        /// Vehiclebrand
//        ///</summary> 
//        [Column("VehicleBrand", TypeName = "VARCHAR2")]
//        public string Vehiclebrand { get; set; }
	      
//        ///<summary>
//        /// Vehiclecolor
//        ///</summary> 
//        [Column("VehicleColor", TypeName = "VARCHAR2")]
//        public string Vehiclecolor { get; set; }
	      
//        ///<summary>
//        /// Panoramaimageurl
//        ///</summary> 
//        [Column("PanoramaImageUrl", TypeName = "VARCHAR2")]
//        public string Panoramaimageurl { get; set; }
	      
//        ///<summary>
//        /// Featureimageurl
//        ///</summary> 
//        [Column("FeatureImageUrl", TypeName = "VARCHAR2")]
//        public string Featureimageurl { get; set; }
	      
//        ///<summary>
//        /// Illegaltypeid
//        ///</summary> 
//        [Column("IllegalTypeId", TypeName = "VARCHAR2")]
//        public string Illegaltypeid { get; set; }
	      
//        ///<summary>
//        /// Inputer
//        ///</summary> 
//        [Column("Inputer", TypeName = "VARCHAR2")]
//        public string Inputer { get; set; }
	      
//        ///<summary>
//        /// Inputtime
//        ///</summary> 
//        [Column("InputTime", TypeName = "DATE")]
//        public DateTime?  Inputtime { get; set; }
	      
//        ///<summary>
//        /// Areacode
//        ///</summary> 
//        [Column("AreaCode", TypeName = "CHAR")]
//        public string Areacode { get; set; }
	      
//        ///<summary>
//        /// Departmentid
//        ///</summary> 
//        [Column("DepartmentId", TypeName = "NUMBER")]
//        public int?  Departmentid { get; set; }
	      
//        ///<summary>
//        /// Createdtime
//        ///</summary> 
//        [Column("CreatedTime", TypeName = "DATE")]
//        public DateTime?  Createdtime { get; set; }
	      
//        ///<summary>
//        /// Checktime
//        ///</summary> 
//        [Column("CheckTime", TypeName = "DATE")]
//        public DateTime?  Checktime { get; set; }
	      
//        ///<summary>
//        /// Checker
//        ///</summary> 
//        [Column("Checker", TypeName = "VARCHAR2")]
//        public string Checker { get; set; }
	      
//        ///<summary>
//        /// Scraptime
//        ///</summary> 
//        [Column("ScrapTime", TypeName = "DATE")]
//        public DateTime?  Scraptime { get; set; }
	      
//        ///<summary>
//        /// Scraper
//        ///</summary> 
//        [Column("Scraper", TypeName = "VARCHAR2")]
//        public string Scraper { get; set; }
	      
//        ///<summary>
//        /// Scrapreason
//        ///</summary> 
//        [Column("ScrapReason", TypeName = "NUMBER")]
//        public int?  Scrapreason { get; set; }
	      
//        ///<summary>
//        /// Status
//        ///</summary> 
//        [Column("Status", TypeName = "NUMBER")]
//        public int?  Status { get; set; }
	      
//        ///<summary>
//        /// Inquiryflag
//        ///</summary> 
//        [Column("InquiryFlag", TypeName = "NUMBER")]
//        public int?  Inquiryflag { get; set; }
	      
//        ///<summary>
//        /// Isunrecognizedplateno
//        ///</summary> 
//        [Column("IsUnrecognizedPlateNo", TypeName = "NUMBER")]
//        public int?  Isunrecognizedplateno { get; set; }
	      
//        ///<summary>
//        /// Serialid
//        ///</summary> 
//        [Column("SerialId", TypeName = "VARCHAR2")]
//        public string Serialid { get; set; }
	      
//        ///<summary>
//        /// Effectivedate
//        ///</summary> 
//        [Column("EffectiveDate", TypeName = "DATE")]
//        public DateTime?  Effectivedate { get; set; }
	      
//        ///<summary>
//        /// Enginecode
//        ///</summary> 
//        [Column("EngineCode", TypeName = "VARCHAR2")]
//        public string Enginecode { get; set; }
	      
//        ///<summary>
//        /// Identificationname
//        ///</summary> 
//        [Column("IdentificationName", TypeName = "VARCHAR2")]
//        public string Identificationname { get; set; }
	      
//        ///<summary>
//        /// Identificationno
//        ///</summary> 
//        [Column("IdentificationNo", TypeName = "VARCHAR2")]
//        public string Identificationno { get; set; }
	      
//        ///<summary>
//        /// Ownermobilenumber
//        ///</summary> 
//        [Column("OwnerMobileNumber", TypeName = "VARCHAR2")]
//        public string Ownermobilenumber { get; set; }
	      
//        ///<summary>
//        /// Ownername
//        ///</summary> 
//        [Column("OwnerName", TypeName = "VARCHAR2")]
//        public string Ownername { get; set; }
	      
//        ///<summary>
//        /// Busload
//        ///</summary> 
//        [Column("Busload", TypeName = "VARCHAR2")]
//        public string Busload { get; set; }
	      
//        ///<summary>
//        /// Vehiclebrandalias
//        ///</summary> 
//        [Column("VehicleBrandAlias", TypeName = "VARCHAR2")]
//        public string Vehiclebrandalias { get; set; }
	      
//        ///<summary>
//        /// Vehicleidentificationno
//        ///</summary> 
//        [Column("VehicleIdentificationNo", TypeName = "VARCHAR2")]
//        public string Vehicleidentificationno { get; set; }
	      
//        ///<summary>
//        /// Vehiclestatus
//        ///</summary> 
//        [Column("VehicleStatus", TypeName = "VARCHAR2")]
//        public string Vehiclestatus { get; set; }
	      
//        ///<summary>
//        /// Licenceissuingauthority
//        ///</summary> 
//        [Column("LicenceIssuingAuthority", TypeName = "VARCHAR2")]
//        public string Licenceissuingauthority { get; set; }
	      
//        ///<summary>
//        /// Vehiclebrandintraffics
//        ///</summary> 
//        [Column("VehicleBrandInTraffics", TypeName = "VARCHAR2")]
//        public string Vehiclebrandintraffics { get; set; }
	      
//        ///<summary>
//        /// Vehiclecolorintraffics
//        ///</summary> 
//        [Column("VehicleColorInTraffics", TypeName = "VARCHAR2")]
//        public string Vehiclecolorintraffics { get; set; }
	      
//        ///<summary>
//        /// Vehiclemodel
//        ///</summary> 
//        [Column("VehicleModel", TypeName = "VARCHAR2")]
//        public string Vehiclemodel { get; set; }
	      
//        ///<summary>
//        /// Homeaddress
//        ///</summary> 
//        [Column("HomeAddress", TypeName = "VARCHAR2")]
//        public string Homeaddress { get; set; }
	      
//        ///<summary>
//        /// Homearea
//        ///</summary> 
//        [Column("HomeArea", TypeName = "VARCHAR2")]
//        public string Homearea { get; set; }
	      
//        ///<summary>
//        /// Phonenumber
//        ///</summary> 
//        [Column("PhoneNumber", TypeName = "VARCHAR2")]
//        public string Phonenumber { get; set; }
	      
//        ///<summary>
//        /// Vehicletype
//        ///</summary> 
//        [Column("VehicleType", TypeName = "VARCHAR2")]
//        public string Vehicletype { get; set; }
	      
//        ///<summary>
//        /// Platetype
//        ///</summary> 
//        [Column("PlateType", TypeName = "VARCHAR2")]
//        public string Platetype { get; set; }
	      
//        ///<summary>
//        /// Useproperty
//        ///</summary> 
//        [Column("UseProperty", TypeName = "VARCHAR2")]
//        public string Useproperty { get; set; }
	      
//        ///<summary>
//        /// Foreignvoucherno
//        ///</summary> 
//        [Column("ForeignVoucherNo", TypeName = "VARCHAR2")]
//        public string Foreignvoucherno { get; set; }
	      
//        ///<summary>
//        /// Uploadperson
//        ///</summary> 
//        [Column("UploadPerson", TypeName = "VARCHAR2")]
//        public string Uploadperson { get; set; }
	      
//        ///<summary>
//        /// Uploadtime
//        ///</summary> 
//        [Column("UploadTime", TypeName = "DATE")]
//        public DateTime?  Uploadtime { get; set; }
	      
//        ///<summary>
//        /// Uploadstatus
//        ///</summary> 
//        [Column("UploadStatus", TypeName = "NVARCHAR2")]
//        public string Uploadstatus { get; set; }
	      
//        ///<summary>
//        /// Remark
//        ///</summary> 
//        [Column("Remark", TypeName = "VARCHAR2")]
//        public string Remark { get; set; }
	      
//        ///<summary>
//        /// Penaltyamount
//        ///</summary> 
//        [Column("PenaltyAmount", TypeName = "NUMBER")]
//        public int?  Penaltyamount { get; set; }
	      
//        ///<summary>
//        /// Deductionscore
//        ///</summary> 
//        [Column("DeductionScore", TypeName = "NUMBER")]
//        public int?  Deductionscore { get; set; }
	      
//        ///<summary>
//        /// Penaltyamountflag
//        ///</summary> 
//        [Column("PenaltyAmountFlag", TypeName = "NUMBER")]
//        public int?  Penaltyamountflag { get; set; }
	      
//        ///<summary>
//        /// Processingdepartment
//        ///</summary> 
//        [Column("ProcessingDepartment", TypeName = "NUMBER")]
//        public int?  Processingdepartment { get; set; }
	      
//        ///<summary>
//        /// Legalizeillegaltypeno
//        ///</summary> 
//        [Column("LegalizeIllegalTypeNo", TypeName = "VARCHAR2")]
//        public string Legalizeillegaltypeno { get; set; }
	      
//        ///<summary>
//        /// Uploadno
//        ///</summary> 
//        [Column("UploadNo", TypeName = "VARCHAR2")]
//        public string Uploadno { get; set; }
	      
//        ///<summary>
//        /// Checkcode
//        ///</summary> 
//        [Column("CheckCode", TypeName = "VARCHAR2")]
//        public string Checkcode { get; set; }
	      
//        ///<summary>
//        /// Repeatcheck
//        ///</summary> 
//        [Column("RepeatCheck", TypeName = "NUMBER")]
//        public bool?  Repeatcheck { get; set; }
	      
//        ///<summary>
//        /// Platenolocation
//        ///</summary> 
//        [Column("PlateNoLocation", TypeName = "VARCHAR2")]
//        public string Platenolocation { get; set; }
	      
//        ///<summary>
//        /// Distinguishable
//        ///</summary> 
//        [Column("Distinguishable", TypeName = "NUMBER")]
//        public bool?  Distinguishable { get; set; }
	      
//        ///<summary>
//        /// Updatetime
//        ///</summary> 
//        [Column("UpdateTime", TypeName = "DATE")]
//        public DateTime?  Updatetime { get; set; }
	      
//        ///<summary>
//        /// Assetsid
//        ///</summary> 
//        [Column("AssetsId", TypeName = "VARCHAR2")]
//        public string Assetsid { get; set; }
	      
//        ///<summary>
//        /// Assetsname
//        ///</summary> 
//        [Column("AssetsName", TypeName = "VARCHAR2")]
//        public string Assetsname { get; set; }
	      
//        ///<summary>
//        /// Uploadfailcount
//        ///</summary> 
//        [Column("UploadFailCount", TypeName = "NUMBER")]
//        public int?  Uploadfailcount { get; set; }
	      
//        ///<summary>
//        /// Yellowmarkchecker
//        ///</summary> 
//        [Column("YellowMarkChecker", TypeName = "VARCHAR2")]
//        public string Yellowmarkchecker { get; set; }
	      
//        ///<summary>
//        /// Yellowmarkchecktime
//        ///</summary> 
//        [Column("YellowMarkCheckTime", TypeName = "DATE")]
//        public DateTime?  Yellowmarkchecktime { get; set; }
	      
//        ///<summary>
//        /// Preoperator
//        ///</summary> 
//        [Column("PreOperator", TypeName = "VARCHAR2")]
//        public string Preoperator { get; set; }
	      
//        ///<summary>
//        /// Preoperatetime
//        ///</summary> 
//        [Column("PreOperateTime", TypeName = "DATE")]
//        public DateTime?  Preoperatetime { get; set; }
	      
//        ///<summary>
//        /// Preoperatetype
//        ///</summary> 
//        [Column("PreOperateType", TypeName = "NUMBER")]
//        public bool?  Preoperatetype { get; set; }
	      
//    }  
//}

