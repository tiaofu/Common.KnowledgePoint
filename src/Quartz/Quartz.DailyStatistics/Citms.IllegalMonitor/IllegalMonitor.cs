/*
 * Model: 违法检测
 * Desctiption: 针对违法数据进行检测
 * Author: 向常冬
 * Created: 2017/7/4
 * Copyright：
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Data.SQLite;
using Citms.DailyStatistics;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Citms.IllegalMonitor
{
    public class IllegalMonitor
    {
        /// <summary>
        /// calculation 
        /// </summary>
        /// <param name="table"></param>     
        /// <param name="IllegalDate"></param>
        /// <returns></returns>
        public static void HandlerIllegal(DataTable table, string IllegalDate)
        {
            try
            {
                string token = UserTokenHelper.GetToken();
                //handle illegledata
                #region handle illegledata
                if (table == null || table.Rows.Count == 0)
                {
                    LogHelper.WriteInfo(string.Format("违法检测-没有违法统计数据，不进行数据检测，检测时间:{0}", IllegalDate));
                    return;
                }
                LogHelper.WriteInfo(string.Format("违法检测-开始违法检测，检测时间:{0}", IllegalDate));
                #region calculation basicdata
                LogHelper.WriteInfo(string.Format("违法检测-获取检测数据"));
                //1.basic
                string getSvgSql = string.Format("select * from p_IllegalMonitor order by SpottingId ");
                //SQLiteParameter para = new SQLiteParameter(DbType.String, "") { Value=""};
                List<IllegalMonitorModel> svglist = DbHelper.GetList<IllegalMonitorModel>(getSvgSql, null);
                //2.yesterday
                string yesDate = Convert.ToDateTime(IllegalDate).AddDays(-1).ToString("yyyy-MM-dd");
                string getyesData = string.Format("select * from PUNISH_ILLEGALVEHICLECOUNT where OCCERDATE='{0}'", yesDate);
                DataTable yestable = new DataTable();
                using (OracleConnection conn = new OracleConnection(ConfigManage.SysConfig.DbConn))
                {
                    OracleDataAdapter sda = new OracleDataAdapter(getyesData, conn);
                    sda.Fill(yestable);
                }
                List<IllegalVehicleCount> yesDataList = DataTableListHelper.ToList<IllegalVehicleCount>(yestable);
                //3.lastweek
                string lastweekDate = Convert.ToDateTime(IllegalDate).AddDays(-7).ToString("yyyy-MM-dd");
                string getlastweekData = string.Format("select * from PUNISH_ILLEGALVEHICLECOUNT where OCCERDATE='{0}'", lastweekDate);
                DataTable lastweektable = new DataTable();
                using (OracleConnection conn = new OracleConnection(ConfigManage.SysConfig.DbConn))
                {
                    OracleDataAdapter sda = new OracleDataAdapter(getlastweekData, conn);
                    sda.Fill(lastweektable);
                }
                List<IllegalVehicleCount> lastweekDataList = DataTableListHelper.ToList<IllegalVehicleCount>(lastweektable);
                #endregion

                foreach (DataRow item in table.Rows)
                {
                    string SpottingId = item["SPOTTINGID"].ToString();
                    try
                    {
                        #region illegal compute                       
                        string IllegalTypeNo = item["ILLEGALTYPENO"].ToString();
                        string LegalizeIllegalTypeNo = item["LEGALIZEILLEGALTYPENO"].ToString();
                        int IllegalCount = Convert.ToInt32(item["COUNT"].ToString());
                        //find illegl svg
                        IllegalMonitorModel model = svglist.Where(p => p.SpottingId == SpottingId && p.IllegalTypeNo == IllegalTypeNo && p.LegalizeIllegalTypeNo == LegalizeIllegalTypeNo).FirstOrDefault();
                        IllegalVehicleCount yesCount = yesDataList.Where(p => p.SpottingId == SpottingId && p.IllegalTypeNo == IllegalTypeNo && p.LegalIzeIllegalTypeNo == LegalizeIllegalTypeNo).FirstOrDefault();
                        IllegalVehicleCount lastweekCount = lastweekDataList.Where(p => p.SpottingId == SpottingId && p.IllegalTypeNo == IllegalTypeNo && p.LegalIzeIllegalTypeNo == LegalizeIllegalTypeNo).FirstOrDefault();
                        //calculation
                        #region
                        if (model == null)
                            LogHelper.WriteInfo(
                                string.Format("违法检测-检测路口：{0},检测违法自定义编号:{1},标准代码{2},不存在比对基础",
                                SpottingId, IllegalTypeNo, LegalizeIllegalTypeNo));
                        else
                            LogHelper.WriteInfo(
                                string.Format("违法检测-检测路口：{0},检测违法自定义编号:{1},标准代码{2},存在比对基础",
                                SpottingId, IllegalTypeNo, LegalizeIllegalTypeNo));
                        #endregion
                        #region
                        int H = 0, L = 0, X = IllegalCount;
                        decimal V = 0, Ho = 0, Ao = 0, Lo = 0, Bo = 0, Vo = 0, Co = 0;
                        if (model == null && yesCount == null && lastweekCount == null)
                        {
                            LogHelper.WriteInfo(
                                string.Format("违法检测-数据不进行检测比对，所有比对基础参数无法获得" + SpottingId));
                            continue;
                        }
                        if (model != null)
                        {
                            Ho = model.SvgH;
                            Lo = model.SvgL;
                            Vo = model.SvgV;
                            V = model.SvgC;
                        }
                        else
                        {
                            V = X;
                        }
                        if (lastweekCount == null)
                            H = X;
                        else
                            H = Convert.ToInt32(lastweekCount.Count);
                        if (yesCount == null)
                            L = X;
                        else
                            L = Convert.ToInt32(yesCount.Count);
                        if (model != null && yesCount != null && lastweekCount != null)
                        {
                            Ao = 0.20m;
                            Bo = 0.35m;
                            Co = 0.45m;
                        }
                        else if (model != null && yesCount != null)
                        {
                            Ao = 0.00m;
                            Bo = 0.40m;
                            Co = 0.60m;
                        }
                        else if (model != null && lastweekCount != null)
                        {
                            Ao = 0.30m;
                            Bo = 0.00m;
                            Co = 0.70m;
                        }
                        else if (yesCount != null && lastweekCount != null)
                        {
                            Ao = 0.25m;
                            Bo = 0.75m;
                            Co = 0.00m;
                        }
                        else if (yesCount != null)
                        {
                            Ao = 0.00m;
                            Bo = 1.00m;
                            Co = 0.00m;
                        }
                        else if (lastweekCount != null)
                        {
                            Ao = 1.00m;
                            Bo = 0.00m;
                            Co = 0.00m;
                        }
                        else if (model != null)
                        {
                            Ao = 0.00m;
                            Bo = 0.00m;
                            Co = 1.00m;
                        }
                        #endregion
                        LogHelper.WriteInfo(
                                string.Format(SpottingId + "违法检测-该路口检测中，检测参数为：X:{0},H:{1},L:{2},V:{3},Ho:{4},Lo:{5},Vo:{6},Ao:{7},Bo:{8},Co:{9}",
                                X, H, L, V, Ho, Lo, Vo, Ao, Bo, Co));
                        decimal fx =
                            ((Math.Abs((decimal)Math.Abs(X - H) / (decimal)H - Ho)) * Ao) +
                            ((Math.Abs((decimal)Math.Abs(X - L) / (decimal)L - Lo)) * Bo) +
                            ((Math.Abs((decimal)Math.Abs(X - V) / (decimal)V - Vo)) * Co);
                        //update model
                        #region
                        string sql = string.Empty;
                        SQLiteParameter[] paras;
                        if (model == null)
                        {
                            decimal h = lastweekCount == null ? 1.00m : ((decimal)Math.Abs(X - H)) / (decimal)H;
                            decimal l = yesCount == null ? 1.00m : ((decimal)Math.Abs(X - L)) / (decimal)L;
                            sql = @"insert into p_IllegalMonitor(Id,SpottingId,IllegalTypeNo,LegalizeIllegalTypeNo,SvgC,SvgH,SvgL,SvgV,UpdateDate,Remark) 
                            values(@Id,@SpottingId,@IllegalTypeNo,@LegalizeIllegalTypeNo,@SvgC,@SvgH,@SvgL,@SvgV,@UpdateDate,@Remark)";
                            paras = new SQLiteParameter[]
                            {
                            new SQLiteParameter("@Id",DbType.String) { Value=Guid.NewGuid().ToString("N")},
                            new SQLiteParameter("@SpottingId",DbType.String) { Value=SpottingId},
                            new SQLiteParameter("@IllegalTypeNo",DbType.String) { Value=IllegalTypeNo},
                            new SQLiteParameter("@LegalizeIllegalTypeNo",DbType.String) { Value=LegalizeIllegalTypeNo},
                            new SQLiteParameter("@SvgC",DbType.Decimal) { Value=IllegalCount},
                            new SQLiteParameter("@SvgH",DbType.Decimal) { Value=h},
                            new SQLiteParameter("@SvgL",DbType.Decimal) { Value=l},
                            new SQLiteParameter("@SvgV",DbType.Decimal) { Value=((decimal)Math.Abs(X-V))/(decimal)V},
                            new SQLiteParameter("@UpdateDate",DbType.DateTime) { Value=DateTime.Now},
                            new SQLiteParameter("@Remark",DbType.String) { Value=string.Empty}
                            };
                        }
                        else
                        {
                            decimal sv = model.SvgV == 0.00m ? (((decimal)Math.Abs(X - V)) / (decimal)V) : ((decimal)model.SvgV + ((decimal)Math.Abs(X - V)) / (decimal)V) / (decimal)2;
                            sql = @"update p_IllegalMonitor 
                            set SvgC=@SvgC,SvgH=@SvgH,SvgL=@SvgL,SvgV=@SvgV,UpdateDate=@UpdateDate where Id=@Id";
                            paras = new SQLiteParameter[]
                            {
                            new SQLiteParameter("@Id",DbType.String) { Value=model.Id},
                            new SQLiteParameter("@SvgC",DbType.Decimal) { Value=(decimal)(model.SvgC+IllegalCount)/(decimal)2},
                            new SQLiteParameter("@SvgH",DbType.Decimal) { Value=((decimal)model.SvgH +((decimal)Math.Abs(X-H))/(decimal)H)/(decimal)2},
                            new SQLiteParameter("@SvgL",DbType.Decimal) { Value=((decimal)model.SvgL +((decimal)Math.Abs(X-L))/(decimal)L)/(decimal)2},
                            new SQLiteParameter("@SvgV",DbType.Decimal) { Value=sv},
                            new SQLiteParameter("@UpdateDate",DbType.DateTime) { Value=DateTime.Now}
                            };
                        }
                        int result = DbHelper.InsertValue(sql, paras);
                        #endregion

                        #region send message
                        bool isSend = true;
                        var count = IllegalCount + IllegalCount * fx;
                        if (count <= ConfigManage.SysConfig.FloatingRange && IllegalCount <= ConfigManage.SysConfig.FloatingRange)
                        {
                            isSend = false;
                        }
                        if (Math.Round(Math.Abs(fx), 2) > Math.Round(ConfigManage.SysConfig.NoticePercent, 2) && isSend)
                        {
                            LogHelper.WriteInfo(
                                string.Format(SpottingId + "违法检测-该路口增/减比例超过设定比例，检测值{0}，设置比例{1}，进行报警，发送消息。", fx, ConfigManage.SysConfig.NoticePercent));
                            Spotting spo = CommonInfo.GetSpottingName(SpottingId);
                            //api
                            var SysMessage = new SysMessage
                            {
                                MessageId = Guid.NewGuid().ToString("N"),
                                Content = string.Format("违法每日检测，检测违法日期:{7},路口:{5}(编号:{6},Id:{0}),标准违法代码:{1},自定义违法代码:{2},违法数量:{4},检测浮动比例:{3}",
                                SpottingId, LegalizeIllegalTypeNo, IllegalTypeNo, fx.ToString("p2"), IllegalCount, spo == null ? SpottingId + "?" : spo.SpottingName,
                                spo == null ? SpottingId + "?" : spo.SpottingNo, IllegalDate),
                                Sender = "admin",
                                NoticeType = "IllegalDailyStatistics",
                                FromType = "每日违法检测工具"
                            };
                            using (var httpClient = new HttpClient())
                            {
                                if (!string.IsNullOrEmpty(token))
                                {
                                    try
                                    {
                                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
                                        var responseJson = httpClient.PostAsync(ConfigManage.SysConfig.ApiAddress, SysMessage,
                                                            new System.Net.Http.Formatting.JsonMediaTypeFormatter()).Result.Content.ReadAsAsync<ApiResult<string>>().Result;
                                        if (!responseJson.HasError)
                                        {
                                            LogHelper.WriteInfo(
                                                     string.Format(SpottingId + "违法检测-消息推送完成，消息：{0}", JsonConvert.SerializeObject(SysMessage)));
                                        }
                                        else
                                        {
                                            LogHelper.WriteInfo(
                                                     string.Format(SpottingId + "违法检测-消息推送错误，错误：{0}", responseJson.Message));
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        LogHelper.WriteInfo(
                                                     string.Format(SpottingId + "违法检测-消息推送异常，异常：{0}", e.ToString()));
                                    }
                                }
                                else
                                {
                                    LogHelper.WriteInfo(
                                string.Format(SpottingId + "违法检测-IMS Token获取失败"));
                                }
                            }
                        }
                        #endregion

                        LogHelper.WriteInfo(
                                string.Format(SpottingId + "违法检测-该路口检测完毕，检测值为：{0}", fx));
                        #endregion
                    }
                    catch (Exception e)
                    {
                        LogHelper.WriteInfo(string.Format("违法检测异常:路口-{0}", SpottingId));
                    }
                }
                #endregion
            }
            catch (Exception e)
            {
                LogHelper.Error(string.Format("违法检测-检测异常:{0}", e.ToString()));
            }
        }
    }
    ///<summary>
    ///系统消息实体
    ///</summary>
    public class SysMessage
    {
        ///<summary>
        ///消息ID(主键) 
        ///</summary>     
        public string MessageId { get; set; }

        ///<summary>
        ///消息内容 
        ///</summary>      
        public string Content { get; set; }

        ///<summary>
        ///消息发送人(用户Code) 
        ///</summary>     
        public string Sender { get; set; }

        ///<summary>
        ///消息接收人(用户Code) 
        ///</summary>  
        public string Receiver { get; set; }

        ///<summary>
        ///消息创建日期 
        ///</summary>      
        public DateTime? CreatedTime { get; set; }

        ///<summary>
        ///通知类型 
        ///</summary>      
        public string NoticeType { get; set; }

        ///<summary>
        ///消息来源(eg:设备异常报警 用户登录...) 
        ///</summary> 
        public string FromType { get; set; }

        ///<summary>
        ///消息来源GUID(eg:用户存储业务数据GUID,可用来做跳转使用) 
        ///</summary>       
        public string FkGUID { get; set; }

        ///<summary>
        ///消息是否已读 默认为0(0:未读,1:已读) 
        ///</summary>      
        public bool? IsRead { get; set; }

        ///<summary>
        ///备注 
        ///</summary>     
        public string Remark { get; set; }

    }
    public class ApiResult<TResult>
    {
        /// <summary>
        /// 执行是否成功：true false
        /// </summary>
        /// <remarks></remarks>
        [Obsolete("请考虑使用 ErrorCode 返回是否有错误的信息，约定 ErrorCode = 0 为没有错误")]
        public bool HasError { get; set; }

        private int _errorCode;

        /// <summary>
        /// 错误码，不同的 api 接口自己定义，调用方需要根据具体接口的 ErrorCode 来进行处理，后期会定义一系列标准错误码.
        /// 
        /// 通用错误码
        /// 错误码         错误信息
        /// 
        /// </summary>
        public int ErrorCode
        {
            get { return this._errorCode; }
            set
            {
                this._errorCode = value;
#pragma warning disable CS0618
                if (value == 0)
                {
                    this.HasError = false;
                }
                else
                {
                    this.HasError = true;
                }
#pragma warning restore CS061
            }
        }

        private string _message;

        /// <summary>
        /// 执行返回消息
        /// </summary>
        /// <remarks></remarks>
        public string Message
        {
            get
            {
                if (this._message == null)
                {
                    switch (this.ErrorCode)
                    {
                        case -1: return "未知错误";
                        case 0: return "成功";
                        case 1: return "参数错误";
                        case 2: return "参数不能为空";
                        case 1001: return "连接数据库失败";
                    }
                }
                return this._message;
            }
            set { this._message = value; }
        }

        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 返回的主要内容.
        /// </summary>
        public TResult Result { get; set; }

        /// <summary>
        /// 数据总条数
        /// </summary>
        public int? TotalCount { get; set; }

        /// <summary>
        /// 数据总页数
        /// </summary>
        public int? TotalPage { get; set; }

        public ApiResult()
        {
            Result = default(TResult);
            //HasError = false;
        }

        ///// <summary>
        ///// 根据每页显示数与总记录数计算出总页数
        ///// </summary>
        ///// <param name="rows">每页显示数</param>
        ///// <param name="totalRecord">结果总记录数</param>
        ///// <param name="isPagination">是否分页 如果分页则进行计算 不分页则返回null</param>
        ///// <returns></returns>
        //public int? CalculateTotalPage(int rows, int totalRecord, bool isPagination)
        //{
        //    if (isPagination && rows > 0)
        //    {
        //        return Convert.ToInt32(Math.Ceiling((double)totalRecord / (double)rows));
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
    }
}
