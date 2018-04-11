using log4net;
using QuartzDemo.QuartzJobs;
using System;
using Citms.PIS.Model.Punish;
using System.Collections.Generic;
using System.Linq;
using Citms.PIS.Contexts;
using Citms.Traffics.Punishment.Models;
using System.Configuration;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using Citms.PIS.Model.Common;
using Citms.Data;
using Zongsoft.Data.Entities;
using Zongsoft.Services;

namespace QuartzDemo
{
    /// <summary>
    /// 数据复制
    /// </summary>
    public static class CopyIllegalDataWork
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(TestJob));
        private static readonly ILog _faillogger = LogManager.GetLogger("ErrorLog");
        private static string strConnV4 = ConfigurationManager.AppSettings["V4"];// "DATA SOURCE=192.168.0.217:1521/TJIMS;PASSWORD=citms;PERSIST SECURITY INFO=True;USER ID=tjepp_s_t";
        private static string strConnV2 = ConfigurationManager.AppSettings["V2"];//"DATA SOURCE=192.168.0.236:1521/orcl;PASSWORD=citms;PERSIST SECURITY INFO=True;USER ID=tjepp_s_d";
        private static DateTime Start = Convert.ToDateTime(ConfigurationManager.AppSettings["ST"]);
        private static DateTime? lastTime = null;
        private static DateTime EndTime = Convert.ToDateTime(ConfigurationManager.AppSettings["ET"]);
        private static List<IllegalType> typeList = null;
        private static int count = Convert.ToInt32(ConfigurationManager.AppSettings["Count"]);

        private static IServiceProviderFactory _serviceProviderFactory;
        private static IObjectAccess _objectAccess;

        public static void HandlerData()
        {        
            if (typeList == null)
            {
                try
                {
                    using (var cdb = new DbContext<IllegalType>(strConnV4))
                    {
                        typeList = cdb.Set<IllegalType>().ToList();
                        cdb.Dispose();
                    }
                    _logger.InfoFormat(string.Format("加载V4平台的违法类型成功!"));
                }
                catch (Exception e)
                {
                    _faillogger.Error(string.Format("加载V4平台的违法类型异常:{0}!", e.Message));
                }
            }
            #region
            if (lastTime == null)
            {
                lastTime = Start;
            }
            if (EndTime == null)
            {
                EndTime = DateTime.Now;
            }
            if (lastTime >= EndTime)
            {
                _logger.InfoFormat(string.Format("数据转换导出完成，起始时间{0},最后一次执行时间{1}", Start.ToString(), lastTime.ToString()));
                return;
            }
            List<IllegalVehicle> realList = null;

            DateTime time = ((DateTime)lastTime).AddMinutes(count);
            try
            {
                _logger.InfoFormat("开始查询V2数据!");
                DateTime newtime = time.AddMinutes(-count);
                _logger.InfoFormat(string.Format("查询V2数据，开始时间{0},结束时间{1}", newtime.ToString(), time.ToString()));
                lastTime = time;
                //var where = new Dictionary<string, object>();
                //where.Add("Timestamp", new DateTime?[] { });
                //where.Add("Status", 6);

                //realList = _objectAccess.Select<IllegalVehicle>(where).ToList();
                using (var cdb = new DbContext<IllegalVehicle>(strConnV2))
                {
                    realList = cdb.Set<IllegalVehicle>().Where(p => p.Status == 6 && p.Timestamp >= newtime && p.Timestamp < time).ToList();
                    cdb.Dispose();
                }
            }
            catch (Exception e)
            {
                _faillogger.Error(string.Format("处理违法异常,开始时间{0},结束时间{1}!", time.ToString(), time.AddMinutes(count).ToString()));
                _faillogger.Error("查询数据异常，异常原因!" + e.Message);
            }
            if (realList != null && realList.Count > 0)
            {
                _logger.InfoFormat(string.Format("获取到V2违法数据,开始时间{0},结束时间{1},总共{2}条！", time.ToString(), time.AddMinutes(count).ToString(), realList.Count));
                try
                {
                    using (var cdb = new DbContext<IllegalVehicleReal>(strConnV4))
                    {
                        //进行过违法数据转换，入库
                        foreach (var item in realList)
                        {
                            IllegalVehicleReal real = new IllegalVehicleReal()
                            {
                                #region
                                ProcessId = "",//item.Processid.ToString("N"),
                                SpottingId = item.Crossingid,
                                SpottingName = item.Crossingname,
                                DirectionId = item.Directionid,
                                DirectionName = item.Directionname,
                                LaneNo = item.Laneid,
                                OccurTime = item.Timestamp,
                                RunSpeed = item.Runspeed,
                                PlateNo = item.Plateno,
                                PlateColor = item.Platecolorid,
                                PlateType = item.Platetype,
                                VehicleBrand = item.Vehiclebrand,
                                VehicleColor = item.Vehiclecolor,
                                PanoramaimageUrl1 = item.Panoramaimageurl,
                                PanoramaimageUrl2 = "",
                                PanoramaimageUrl3 = "",
                                PanoramaimageUrl4 = "",
                                FeatureimageUrl = item.Featureimageurl,
                                IllegalTypeNo = GetIllegalCode(item.Legalizeillegaltypeno, item.Illegaltypeid),
                                //IllegalTypeNo=item.IllegalTypeId,
                                Inputer = "",
                                InputTime = null,
                                AreaCode = item.Areacode,
                                //存在问题
                                DepartmentId = item.Departmentid.ToString(),
                                CreatedTime = item.Createdtime,
                                CheckTime = item.Checktime,
                                Checker = item.Checker,
                                Scraper = item.Scraper,
                                ScrapTime = item.Scraptime,
                                ScrapreasonNo = item.Scrapreason.ToString(),
                                Status = "1004",
                                InquiryFlag = item.Inquiryflag,
                                IsUnrecognizedPlateNo = item.Isunrecognizedplateno,
                                SerialId = item.Serialid,
                                EffectiveDate = item.Effectivedate,
                                EngineCode = item.Enginecode,
                                IdentificationName = item.Identificationname,
                                IdentificationNo = item.Identificationno,
                                OwnerMobileNumber = item.Ownermobilenumber,
                                OwnerName = item.Ownername,
                                BusLoad = item.Busload.ToString(),
                                VehicleBrandAlias = item.Vehiclebrandalias,
                                VehicleIdentificationNo = item.Vehicleidentificationno,
                                VehicleStatus = item.Vehiclestatus,
                                LicenceIssuingAuthority = item.Licenceissuingauthority,
                                VehicleBrandInTraffics = item.Vehiclebrandintraffics,
                                VehicleColorInTraffics = item.Vehiclecolorintraffics,
                                VehicleModel = item.Vehiclemodel,
                                HomeAddress = item.Homeaddress,
                                HomeArea = item.Homearea,
                                PhoneNumber = item.Phonenumber,
                                VehicleType = item.Vehicletype,
                                UseProperty = item.Useproperty,
                                ForeIgnvoucherNo = "",
                                UploadPerson = item.Uploadperson,
                                UpdateTime = item.Updatetime,
                                UploadStatus = item.Uploadstatus,
                                Remark = item.Remark,
                                PenaltyAmount = item.Penaltyamount,
                                DeductionScore = item.Deductionscore,
                                PenaltyAmountFlag = item.Penaltyamountflag,
                                //ProcessingDepartment=item.ProcessingDepartment,
                                LegalizeIllegalTypeNo = item.Legalizeillegaltypeno,
                                UploadNo = item.Uploadno,
                                CheckCode = item.Checkcode,
                                RepeatCheck = Convert.ToBoolean(item.Repeatcheck) ? 1 : 0,
                                PlatenoLocation = item.Platenolocation,
                                DistingUishable = 1,
                                UploadTime = item.Updatetime,
                                AssetsNo = item.Assetsid,
                                AssetsName = item.Assetsname,
                                PoliceChecker = "",
                                PoliceCheckTime = null,
                                UploadFailCount = item.Uploadfailcount,
                                PunishFlag = null,
                                VideoUrl = "",
                                IsVideo = null,
                                CollectMode = "",
                                VehicleLength = null,
                                ExportedFlag = null,
                                ExportedTime = null,
                                IsCommon = Convert.ToBoolean(item.Repeatcheck)? 0 :1,
                                RuleId = "",
                                IsSediment = 0,
                                ConvertedTime = null,
                                ConvertedIllegalTypeNo = "",
                                LimitedSpeed = null,
                                ScrapType = ""
                                #endregion
                            };
                            cdb.Set<IllegalVehicleReal>().Add(real);
                        }
                        cdb.SaveChanges();
                        cdb.Dispose();
                        _faillogger.Error(string.Format("处理违法转换到V4版本库成功{2}条,开始时间{0},结束时间{1}!", time.ToString(), time.AddMinutes(count).ToString(), realList.Count));
                    }
                }
                catch (Exception e)
                {
                    _faillogger.Error(string.Format("处理违法转换到V4版本库异常,开始时间{0},结束时间{1}!", time.ToString(), time.AddMinutes(count).ToString()));
                }
            }
            else
            {
                _logger.InfoFormat(string.Format("未获取到V2违法数据,开始时间{0},结束时间{1}！", time.ToString(), time.AddMinutes(count).ToString()));
            }
            #endregion
        }

        public static string GetIllegalCode(string LegalizeIllegalTypeNo, string illegaltypeid)
        {
            IllegalType type = typeList.Where(p => p.IllegalCode == LegalizeIllegalTypeNo).FirstOrDefault();
            if (type == null)
                return illegaltypeid;
            return type.IllegalTypeNo;
        }
    }
}
