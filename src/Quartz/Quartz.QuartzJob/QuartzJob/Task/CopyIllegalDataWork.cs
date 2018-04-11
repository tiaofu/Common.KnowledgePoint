using log4net;
using QuartzJob.QuartzJobs;
using System;
using QuartzJob.Punish;
using System.Collections.Generic;
using System.Linq;
using QuartzJob.Contexts;
using QuartzJob.Models;
using System.Configuration;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using Citms.PIS.Model.Common;
using QuartzJob.Data;
using Zongsoft.Data.Entities;
using Zongsoft.Services;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace QuartzJob
{
    /// <summary>
    /// 数据复制
    /// </summary>
    public static class CopyIllegalDataWork
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(TestJob));
        private static readonly ILog _faillogger = LogManager.GetLogger("ErrorLog");
        private static string strConnV4 = ConfigManage.dbConfig.V4;// ConfigurationManager.AppSettings["V4"];// "DATA SOURCE=192.168.0.217:1521/TJIMS;PASSWORD=citms;PERSIST SECURITY INFO=True;USER ID=tjepp_s_t";
        private static string strConnV2 = ConfigManage.dbConfig.V2;//ConfigurationManager.AppSettings["V2"];//"DATA SOURCE=192.168.0.236:1521/orcl;PASSWORD=citms;PERSIST SECURITY INFO=True;USER ID=tjepp_s_d";
        private static string Sql = ConfigManage.dbConfig.Sql;//ConfigurationManager.AppSettings["SQL"];
        private static DateTime Start = Convert.ToDateTime(ConfigManage.dbConfig.ST);//ConfigurationManager.AppSettings["ST"]);
        private static DateTime? lastTime = null;
        private static DateTime EndTime = Convert.ToDateTime(ConfigManage.dbConfig.ET);//ConfigurationManager.AppSettings["ET"]);
        private static List<IllegalType> typeList = null;
        private static int count = Convert.ToInt32(ConfigManage.dbConfig.Range);//ConfigurationManager.AppSettings["Count"]);

        private static IServiceProviderFactory _serviceProviderFactory;
        private static IObjectAccess _objectAccess;

        public static void HandlerData()
        {
            //_objectAccess = _serviceProviderFactory.Default.Resolve("OracleDbContext") as IObjectAccess;
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
            DataTable table = new DataTable();
            DateTime time = ((DateTime)lastTime).AddMinutes(count);
            DateTime newtime = time.AddMinutes(-count);
            string newsql = string.Empty;
            try
            {
                _logger.InfoFormat("开始查询V2数据!");
                _logger.InfoFormat(string.Format("查询V2数据，开始时间{0},结束时间{1}", newtime.ToString(), time.ToString()));
                lastTime = time;
                var where = new Dictionary<string, object>();
                where.Add("Timestamp", new DateTime?[] { });
                where.Add("Status", 6);

                //realList = _objectAccess.Select<IllegalVehicle>(where).ToList();
                using (OracleConnection conn = new OracleConnection(strConnV2))
                {
                    if (string.IsNullOrEmpty(Sql))
                    {
                        //newsql = "select * from punish_illegalvehicle where \"Status\" = 60 and \"Timestamp\" > = to_date({0},'yyyy/MM/dd HH24:mi:ss') and \"Timestamp\" < to_date({1},'yyyy/MM/dd HH24:mi:ss')";
                        newsql = "select * from punish_illegalvehicle where \"Status\" = 60 and \"Timestamp\" > = to_date(:starttime,'yyyy/MM/dd HH24:mi:ss') and \"Timestamp\" < to_date(:endtime,'yyyy/MM/dd HH24:mi:ss')";
                        //newsql = "select * from punish_illegalvehicle";
                    }
                    else
                    {
                        newsql = Sql;
                    }
                    OracleDataAdapter oda = new OracleDataAdapter(newsql, conn);
                    //newsql = string.Format(newsql, newtime.ToString("yyyy-MM-dd HH:mm:ss"),EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    oda.SelectCommand.Parameters.Add(new OracleParameter(":starttime", newtime.ToString()));
                    oda.SelectCommand.Parameters.Add(new OracleParameter(":endtime", time.ToString()));
                    oda.Fill(table);
                }
                //using (var cdb = new DbContext<IllegalVehicle>(strConnV2))
                //{
                //    realList = cdb.Set<IllegalVehicle>().Where(p => p.Status == 6 && p.Timestamp >= newtime && p.Timestamp < time).ToList();
                //    cdb.Dispose();
                //}
            }
            catch (Exception e)
            {
                _faillogger.Error(string.Format("处理违法异常,开始时间{0},结束时间{1}!", newtime.ToString(), time.ToString()));
                _faillogger.Error("查询数据异常，异常原因!" + e.ToString() + ",异常sql:" + newsql);
            }
            if (table != null && table.Rows.Count > 0)
            {
                _logger.InfoFormat(string.Format("获取到V2违法数据,开始时间{0},结束时间{1},总共{2}条！", newtime.ToString(), time.ToString(), table.Rows.Count));
                try
                {
                    DataTable valueSet = new DataTable();
                    using (OracleConnection conn = new OracleConnection(strConnV4))
                    {
                        OracleDataAdapter oda = new OracleDataAdapter("select * from PUNISH_ILLEGALVEHICLE_REAL where 1=0", conn);
                        oda.Fill(valueSet);
                    }
                    #region table的操作方式
                    //进行过违法数据转换，入库
                    //foreach (DataRow item in table.Rows)
                    //{
                    //    #region
                    //    DataRow rovaw = valueSet.NewRow();
                    //    rovaw["PROCESSID"] = Guid.NewGuid().ToString("N");
                    //    rovaw["SPOTTINGID"] = item["CrossingId"].ToString();
                    //    rovaw["SPOTTINGNAME"] = item["CrossingName"].ToString();
                    //    rovaw["DIRECTIONID"] = item["DirectionId"].ToString();
                    //    rovaw["DIRECTIONNAME"] = item["DirectionName"].ToString();
                    //    rovaw["LANENO"] = item["LaneId"].ToString();
                    //    rovaw["OCCURTIME"] = string.IsNullOrEmpty(item["Timestamp"].ToString()) ? DateTime.MinValue : Convert.ToDateTime(item["Timestamp"].ToString());
                    //    rovaw["RUNSPEED"] = Convert.ToInt32(item["RunSpeed"].ToString());
                    //    rovaw["PLATENO"] = item["PlateNo"].ToString();
                    //    rovaw["PLATECOLOR"] = GetPlateColor(item["PlateColorId"].ToString());
                    //    rovaw["PLATETYPE"] = item["PlateType"].ToString();
                    //    rovaw["VEHICLEBRAND"] = item["VehicleBrand"].ToString();
                    //    rovaw["VEHICLECOLOR"] = item["VehicleColor"].ToString();
                    //    rovaw["PANORAMAIMAGEURL1"] = item["PanoramaImageUrl"].ToString();
                    //    rovaw["PANORAMAIMAGEURL2"] = "";
                    //    rovaw["PANORAMAIMAGEURL3"] = "";
                    //    rovaw["PANORAMAIMAGEURL4"] = "";
                    //    rovaw["FEATUREIMAGEURL"] = item["FeatureImageUrl"].ToString();

                    //    rovaw["ILLEGALTYPENO"] = GetIllegalCode(item["LegalizeIllegalTypeNo"].ToString(), item["IllegalTypeId"].ToString());
                    //    //IllegalTypeNo=item["IllegalTypeId"].ToString(),
                    //    rovaw["Inputer"] = "";
                    //    rovaw["InputTime"] = null;
                    //    rovaw["AreaCode"] = item["AreaCode"].ToString();
                    //    //存在问题                               
                    //    rovaw["DepartmentId"] = GetDepartment(item["DepartmentId"].ToString());
                    //    rovaw["CreatedTime"] = string.IsNullOrEmpty(item["CreatedTime"].ToString()) ? DateTime.MinValue : Convert.ToDateTime(item["CreatedTime"].ToString());
                    //    rovaw["CheckTime"] = string.IsNullOrEmpty(item["CheckTime"].ToString()) ? DateTime.MinValue : Convert.ToDateTime(item["CheckTime"].ToString());
                    //    rovaw["Checker"] = item["Checker"].ToString();
                    //    rovaw["Scraper"] = item["Scraper"].ToString();
                    //    rovaw["ScrapTime"] = string.IsNullOrEmpty(item["ScrapTime"].ToString()) ? DateTime.MinValue : Convert.ToDateTime(item["ScrapTime"].ToString());
                    //    rovaw["ScrapreasonNo"] = item["ScrapReason"].ToString();
                    //    rovaw["Status"] = "1004";
                    //    rovaw["InquiryFlag"] = Convert.ToInt32(item["InquiryFlag"].ToString());
                    //    rovaw["IsUnrecognizedPlateNo"] = Convert.ToInt32(item["IsUnrecognizedPlateNo"].ToString());
                    //    rovaw["SerialId"] = item["SerialId"].ToString();
                    //    rovaw["EffectiveDate"] = string.IsNullOrEmpty(item["EffectiveDate"].ToString()) ? DateTime.MinValue : Convert.ToDateTime(item["EffectiveDate"].ToString());
                    //    rovaw["EngineCode"] = item["EngineCode"].ToString();
                    //    rovaw["IdentificationName"] = item["IdentificationName"].ToString();
                    //    rovaw["IdentificationNo"] = item["IdentificationNo"].ToString();
                    //    rovaw["OwnerMobileNumber"] = item["OwnerMobileNumber"].ToString();
                    //    rovaw["OwnerName"] = item["OwnerName"].ToString();
                    //    rovaw["BusLoad"] = item["Busload"].ToString();
                    //    rovaw["VehicleBrandAlias"] = item["VehicleBrandAlias"].ToString();
                    //    rovaw["VehicleIdentificationNo"] = item["VehicleIdentificationNo"].ToString();
                    //    rovaw["VehicleStatus"] = item["VehicleStatus"].ToString();
                    //    rovaw["LicenceIssuingAuthority"] = item["LicenceIssuingAuthority"].ToString();
                    //    rovaw["VehicleBrandInTraffics"] = item["VehicleBrandInTraffics"].ToString();
                    //    rovaw["VehicleColorInTraffics"] = item["VehicleColorInTraffics"].ToString();
                    //    rovaw["VehicleModel"] = item["VehicleModel"].ToString();
                    //    rovaw["HomeAddress"] = item["HomeAddress"].ToString();
                    //    rovaw["HomeArea"] = item["HomeArea"].ToString();
                    //    rovaw["PhoneNumber"] = item["PhoneNumber"].ToString();
                    //    rovaw["VehicleType"] = item["VehicleType"].ToString();
                    //    rovaw["UseProperty"] = item["UseProperty"].ToString();
                    //    rovaw["ForeIgnvoucherNo"] = "";
                    //    rovaw["UploadPerson"] = item["UploadPerson"].ToString();
                    //    rovaw["UpdateTime"] = string.IsNullOrEmpty(item["UpdateTime"].ToString()) ? DateTime.MinValue : Convert.ToDateTime(item["UpdateTime"].ToString());
                    //    rovaw["UploadStatus"] = item["UploadStatus"].ToString();
                    //    rovaw["Remark"] = "V2平台数据导入，" + item["Remark"].ToString();
                    //    rovaw["PenaltyAmount"] = Convert.ToInt32(item["PenaltyAmount"].ToString());
                    //    rovaw["DeductionScore"] = Convert.ToInt32(item["DeductionScore"].ToString());
                    //    rovaw["PenaltyAmountFlag"] = Convert.ToInt32(item["PenaltyAmountFlag"].ToString());
                    //    rovaw["ProcessingDepartment"] = GetDepartment(item["ProcessingDepartment"].ToString());
                    //    rovaw["LegalizeIllegalTypeNo"] = item["LegalizeIllegalTypeNo"].ToString();
                    //    rovaw["UploadNo"] = item["UploadNo"].ToString();
                    //    rovaw["CheckCode"] = item["CheckCode"].ToString();
                    //    rovaw["RepeatCheck"] = Convert.ToInt32(item["RepeatCheck"].ToString());
                    //    rovaw["PlatenoLocation"] = item["PlateNoLocation"].ToString();
                    //    rovaw["DistingUishable"] = 1;
                    //    rovaw["UploadTime"] = string.IsNullOrEmpty(item["UpdateTime"].ToString()) ? DateTime.MinValue : Convert.ToDateTime(item["UpdateTime"].ToString());
                    //    rovaw["AssetsNo"] = item["AssetsId"].ToString();
                    //    rovaw["AssetsName"] = item["AssetsName"].ToString();
                    //    rovaw["PoliceChecker"] = "";
                    //    rovaw["PoliceCheckTime"] = null;
                    //    rovaw["UploadFailCount"] = Convert.ToInt32(item["UploadFailCount"].ToString());
                    //    rovaw["PunishFlag"] = null;
                    //    rovaw["VideoUrl"] = "";
                    //    rovaw["IsVideo"] = null;
                    //    rovaw["CollectMode"] = "";
                    //    rovaw["VehicleLength"] = null;
                    //    rovaw["ExportedFlag"] = null;
                    //    rovaw["ExportedTime"] = null;
                    //    rovaw["IsCommon"] = Convert.ToInt32(item["RepeatCheck"].ToString()) == 0 ? 1 : 0;
                    //    rovaw["RuleId"] = "";
                    //    rovaw["IsSediment"] = 0;
                    //    rovaw["vertedTime"] = null;
                    //    rovaw["vertedIllegalTypeNo"] = "";
                    //    rovaw["itedSpeed"] = null;
                    //    rovaw["apType"] = "";
                    //    #endregion
                    //}
                    #endregion
                    #region EF的操作方式
                    using (var cdb = new DbContext<IllegalVehicleReal>(strConnV4))
                    {
                        //进行过违法数据转换，入库
                        foreach (DataRow item in table.Rows)
                        {
                            //ASCIIEncoding condding = new ASCIIEncoding();
                            //string guid = System.Text.Encoding.Default.GetString((byte[])item["ProcessId"]);

                            IllegalVehicleReal real = new IllegalVehicleReal()
                            {
                                #region
                                ProcessId = Guid.NewGuid().ToString("N"),
                                SpottingId = item["CrossingId"].ToString().Trim().Trim(),
                                SpottingName = item["CrossingName"].ToString().Trim(),
                                DirectionId = item["DirectionId"].ToString().Trim(),
                                DirectionName = item["DirectionName"].ToString().Trim(),
                                LaneNo = item["LaneId"].ToString().Trim(),
                                OccurTime = string.IsNullOrEmpty(item["Timestamp"].ToString().Trim()) ? DateTime.MinValue : Convert.ToDateTime(item["Timestamp"].ToString().Trim()),
                                RunSpeed = Convert.ToInt32(item["RunSpeed"].ToString().Trim()),
                                PlateNo = item["PlateNo"].ToString().Trim(),
                                PlateColor = DataFormat("PlateColor", item["PlateColorId"].ToString().Trim()),
                                PlateType = DataFormat("PlateType", item["PlateTypeId"].ToString().Trim()),
                                VehicleBrand = item["VehicleBrand"].ToString().Trim(),
                                VehicleColor = item["VehicleColor"].ToString().Trim(),
                                PanoramaimageUrl1 = item["PanoramaImageUrl"].ToString().Trim(),
                                PanoramaimageUrl2 = "",
                                PanoramaimageUrl3 = "",
                                PanoramaimageUrl4 = "",
                                FeatureimageUrl = item["FeatureImageUrl"].ToString().Trim(),
                                IllegalTypeNo = GetIllegalCode(item["LegalizeIllegalTypeNo"].ToString().Trim(), item["IllegalTypeId"].ToString().Trim()),
                                //IllegalTypeNo=item["IllegalTypeId"].ToString().Trim(),

                                Inputer = item["PoliceChecker"].ToString().Trim(),
                                InputTime = string.IsNullOrEmpty(item["PoliceCheckTime"].ToString().Trim()) ? DateTime.MinValue : Convert.ToDateTime(item["PoliceCheckTime"].ToString().Trim()),
                                CheckTime = string.IsNullOrEmpty(item["CheckTime"].ToString().Trim()) ? DateTime.MinValue : Convert.ToDateTime(item["CheckTime"].ToString().Trim()),
                                Checker = item["Checker"].ToString().Trim(),
                                Scraper = item["Scraper"].ToString().Trim(),
                                ScrapTime = string.IsNullOrEmpty(item["ScrapTime"].ToString().Trim()) ? DateTime.MinValue : Convert.ToDateTime(item["ScrapTime"].ToString().Trim()),
                                ScrapreasonNo = item["ScrapReason"].ToString().Trim(),
                                UploadPerson = item["UploadPerson"].ToString().Trim(),
                                UpdateTime = string.IsNullOrEmpty(item["UpdateTime"].ToString().Trim()) ? DateTime.MinValue : Convert.ToDateTime(item["UpdateTime"].ToString().Trim()),

                                AreaCode = item["AreaCode"].ToString().Trim(),
                                DepartmentId = DataFormat("DepartmentId", item["DepartmentId"].ToString().Trim()),
                                CreatedTime = string.IsNullOrEmpty(item["CreatedTime"].ToString().Trim()) ? DateTime.MinValue : Convert.ToDateTime(item["CreatedTime"].ToString().Trim()),                                                               
                                Status = "1004",
                                InquiryFlag = Convert.ToInt32(item["InquiryFlag"].ToString().Trim()),
                                IsUnrecognizedPlateNo = Convert.ToInt32(item["IsUnrecognizedPlateNo"].ToString().Trim()),
                                SerialId = item["SerialId"].ToString().Trim(),
                                EffectiveDate = string.IsNullOrEmpty(item["EffectiveDate"].ToString().Trim()) ? DateTime.MinValue : Convert.ToDateTime(item["EffectiveDate"].ToString().Trim()),
                                EngineCode = item["EngineCode"].ToString().Trim(),
                                IdentificationName = item["IdentificationName"].ToString().Trim(),
                                IdentificationNo = item["IdentificationNo"].ToString().Trim(),
                                OwnerMobileNumber = item["OwnerMobileNumber"].ToString().Trim(),
                                OwnerName = item["OwnerName"].ToString().Trim(),
                                BusLoad = item["Busload"].ToString().Trim(),
                                VehicleBrandAlias = item["VehicleBrandAlias"].ToString().Trim(),
                                VehicleIdentificationNo = item["VehicleIdentificationNo"].ToString().Trim(),
                                VehicleStatus = item["VehicleStatus"].ToString().Trim(),
                                LicenceIssuingAuthority = item["LicenceIssuingAuthority"].ToString().Trim(),
                                VehicleBrandInTraffics = item["VehicleBrandInTraffics"].ToString().Trim(),
                                VehicleColorInTraffics = item["VehicleColorInTraffics"].ToString().Trim(),
                                VehicleModel = item["VehicleModel"].ToString().Trim(),
                                HomeAddress = item["HomeAddress"].ToString().Trim(),
                                HomeArea = item["HomeArea"].ToString().Trim(),
                                PhoneNumber = item["PhoneNumber"].ToString().Trim(),
                                VehicleType = item["VehicleType"].ToString().Trim(),
                                UseProperty = item["UseProperty"].ToString().Trim(),
                                ForeIgnvoucherNo = "",
                                UploadStatus = item["UploadStatus"].ToString().Trim(),
                                Remark = "V2平台数据导入，" + item["Remark"].ToString().Trim() + "导入时间：" + DateTime.Now.ToString().Trim(),
                                PenaltyAmount = Convert.ToInt32(item["PenaltyAmount"].ToString().Trim()),
                                DeductionScore = Convert.ToInt32(item["DeductionScore"].ToString().Trim()),
                                PenaltyAmountFlag = Convert.ToInt32(item["PenaltyAmountFlag"].ToString().Trim()),
                                ProcessingDepartment = DataFormat("DepartmentId", item["ProcessingDepartment"].ToString().Trim()),
                                LegalizeIllegalTypeNo = item["LegalizeIllegalTypeNo"].ToString().Trim(),
                                UploadNo = item["UploadNo"].ToString().Trim(),
                                CheckCode = item["CheckCode"].ToString().Trim(),
                                RepeatCheck = Convert.ToInt32(item["RepeatCheck"].ToString().Trim()),
                                PlatenoLocation = item["PlateNoLocation"].ToString().Trim(),
                                DistingUishable = 1,
                                UploadTime = string.IsNullOrEmpty(item["UpdateTime"].ToString().Trim()) ? DateTime.MinValue : Convert.ToDateTime(item["UpdateTime"].ToString().Trim()),
                                AssetsNo = item["AssetsId"].ToString().Trim(),
                                AssetsName = item["AssetsName"].ToString().Trim(),
                                PoliceChecker = "",
                                PoliceCheckTime = null,
                                UploadFailCount = Convert.ToInt32(item["UploadFailCount"].ToString().Trim()),
                                PunishFlag = null,
                                VideoUrl = "",
                                IsVideo = null,
                                CollectMode = "",
                                VehicleLength = null,
                                ExportedFlag = null,
                                ExportedTime = null,
                                IsCommon = Convert.ToInt32(item["RepeatCheck"].ToString().Trim()) == 0 ? 1 : 0,
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
                        _logger.InfoFormat(string.Format("处理违法转换到V4版本库成功{2}条,开始时间{0},结束时间{1}!", newtime.ToString(), time.ToString(), table.Rows.Count));
                    }
                    #endregion
                }
                catch (Exception e)
                {
                    _faillogger.Error(string.Format("处理违法转换到V4版本库异常,开始时间{0},结束时间{1}!,异常原因{2}", newtime.ToString(), time.ToString(), e.ToString()));
                }
            }
            else
            {
                _logger.InfoFormat(string.Format("未获取到V2违法数据,开始时间{0},结束时间{1}！", newtime.ToString(), time.ToString()));
            }
            #endregion
        }

        public static string DataFormat(string type, string value)
        {
            FormatOption option = ConfigManage.dbConfig.Format.Where(p => p.Field == type).FirstOrDefault();
            if (option == null)
                return value;
            string news = option.Values.Where(p => p.Split('|')[0] == value).FirstOrDefault();
            if (string.IsNullOrEmpty(news))
                return value;
            return news.Split('|')[1];
        }


        //public static string GetDepartment(string oldvalue)
        //{
        //    string value = ConfigManage.dbConfig.Department.Where(p => p.Split('|')[0] == oldvalue).FirstOrDefault();
        //    if (string.IsNullOrEmpty(value))
        //    {
        //        return oldvalue;
        //    }
        //    return value;
        //    //if (string.IsNullOrEmpty(oldId))
        //    //    return "420100000000";
        //    //switch (oldId)
        //    //{
        //    //    case "420107000000": return "d3501e59145f4c0a9fb2517a370b5410";
        //    //    case "420108000000": return "9d2540ba8bae4b2fb840ae51e8112766";
        //    //    case "420111000000": return "4a64330e91094c229d472f9353a13682";
        //    //    case "420112000000": return "4d74d519a0b64ecfb151df5c2244f5e0";
        //    //    case "420114000000": return "edbc63955f2043b4857e4e6eb7b0c828";
        //    //    case "420105005700": return "fa8f0a01a27746dbb89db1837354f1e0";
        //    //    case "420115000000": return "09e38c9e88924b50a6dffce23f7ead80";
        //    //    case "420116000000": return "00aa0edcec344a05be58d02069443eca";
        //    //    case "420117000000": return "0119efdbfcf7420aa69cdbf73c7f92ca";
        //    //    case "420118000000": return "b2ee108afd37418b8e1933047891a619";
        //    //    case "420119000000": return "80666e3ffde0415b8f5eb54cfe02b1b2";
        //    //    case "420128000000": return "939566b7b65548cf888f6e37c04f99f0";
        //    //    case "420129000000": return "bf4c7efae72a460eb12bb1bfef6346fb";
        //    //    case "420130000010": return "99cd81bfef2f4f88aea4f0e0f98508d5";
        //    //    case "420132000000": return "5ef648a73c504f71a781179f7330d268";
        //    //    case "420140000000": return "a84f79196e8c4ebd9dc8907c5816e94b";
        //    //    case "420102000000": return "0ee7e92dfd234d84aa21852280bca19a";
        //    //    case "420103000000": return "5ab17acd6a734d7b9c9c7fbb01f522c6";
        //    //    case "420105000000": return "164c6f9dfd5d4afdae06c65a7de641fe";
        //    //    case "420104000000": return "bdcf94408a0542339c88fb96578b87c2";
        //    //    case "420106000000": return "cfbec31cc65947a5b690fc09a886460d";
        //    //    case "420100000000": return "420100000000";
        //    //    default: return oldId;
        //    //}
        //}

        //public static string GetPlateType(string oldvalue)
        //{
        //    string value = ConfigManage.dbConfig.PlateType.Where(p => p.Split('|')[0] == oldvalue).FirstOrDefault();
        //    if (string.IsNullOrEmpty(value))
        //    {
        //        return oldvalue;
        //    }
        //    return value;
        //}

        //public static string GetPlateColor(string oldvalue)
        //{
        //    string value = ConfigManage.dbConfig.PlateColor.Where(p => p.Split('|')[0] == oldvalue).FirstOrDefault();
        //    if (string.IsNullOrEmpty(value))
        //    {
        //        return oldvalue;
        //    }
        //    return value;
        //    //if (string.IsNullOrEmpty(oldvalue))
        //    //    return "9";
        //    //switch (oldvalue)
        //    //{
        //    //    case "420100000000000128": return "2";
        //    //    case "420100000000000129": return "3";
        //    //    case "26": return "0";
        //    //    case "27": return "1";
        //    //    case "28": return "2";
        //    //    case "29": return "3";
        //    //    case "30": return "9";
        //    //    case "420100000000000125": return "0";
        //    //    case "420100000000000127": return "1";
        //    //    default: return oldvalue;
        //    //}
        //}

        public static string GetIllegalCode(string LegalizeIllegalTypeNo, string illegaltypeid)
        {
            IllegalType type = typeList.Where(p => p.IllegalCode == LegalizeIllegalTypeNo).FirstOrDefault();
            if (type == null)
                return illegaltypeid;
            return type.IllegalTypeNo;
        }
    }
}
