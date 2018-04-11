/*********************************************************
 * CopyRight: tiaoshuidenong. 
 * Author: tiaoshuidenong
 * Address: wuhan
 * Create: 2017-05-10 17:44:16
 * Modify: 2017-05-10 17:44:16
 * Blog: http://www.cnblogs.com/tiaoshuidenong/
 * Description: 
 *********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using log4net;
using QuartzJob.QuartzJobs;
using QuartzJob.Contexts;
using QuartzJob.Vehicle;
using Oracle.ManagedDataAccess.Types;
using Oracle.ManagedDataAccess.Client;
using System.Data;
namespace QuartzJob
{
    /// <summary>
    /// 新能源车的数据同步
    /// </summary>
    public static class CopyNewenergyData
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(TestJob));
        private static readonly ILog _faillogger = LogManager.GetLogger("ErrorLog");
        private static string strConnV4 = ConfigurationManager.AppSettings["V4"];//"DATA SOURCE=192.168.0.217:1521/TJIMS;PASSWORD=citms;PERSIST SECURITY INFO=True;USER ID=tjepp_s_t";
        private static string strConnV2 = ConfigurationManager.AppSettings["V2"];//"DATA SOURCE=192.168.0.236:1521/orcl;PASSWORD=citms;PERSIST SECURITY INFO=True;USER ID=tjepp_s_d";

        public static void HandlerData()
        {
            _logger.InfoFormat(string.Format("开始进行新能源车的数据导入,运行时间{0}!", DateTime.Now.ToString()));
            List<VEHICLE_NEWENERGY_4> V4List = null;
            List<VEHICLE_NEWENERGY_2> V2List = null;
            DataTable table = new DataTable();
            List<VEHICLE_NEWENERGY_4> AddList = new List<VEHICLE_NEWENERGY_4>();
            try
            {
                using (var cdb = new DbContext<VEHICLE_NEWENERGY_4>(strConnV4))
                {
                    V4List = cdb.Set<VEHICLE_NEWENERGY_4>().ToList();
                    cdb.Dispose();
                }
            }
            catch (Exception e)
            {
                _faillogger.Error(string.Format("V4新能源车查询失败,失败原因:{0}", e.Message));
            }
            _logger.InfoFormat(string.Format("V4查询到新平台的新能源车的信息数目{0}条!", V4List.Count));
            try
            {
                using (OracleConnection conn = new OracleConnection(strConnV2))
                {
                    OracleDataAdapter oda = new OracleDataAdapter("select * from V_CLXH", conn);
                    oda.Fill(table);
                    conn.Dispose();
                }
            }
            catch (Exception e)
            {
                _faillogger.Error(string.Format("V2新能源车查询失败,失败原因:{0}", e.Message));
            }
            _logger.InfoFormat(string.Format("V2查询到新能源车库的新能源车的信息数目{0}条!", table.Rows.Count));
            DateTime CreatedTime = DateTime.Now;
            int Count = 0;
            StringBuilder sb = new StringBuilder();
            foreach (DataRow item in table.Rows)
            {
                string A = item["A"].ToString();
                string B = item["B"].ToString();
                string C = item["C"].ToString();
                if (!string.IsNullOrEmpty(A) && !string.IsNullOrEmpty(B) && !string.IsNullOrEmpty(C))
                {
                    VEHICLE_NEWENERGY_4 v4 = V4List.Where(p => p.A == A && p.B == B && p.C == C).FirstOrDefault();
                    if (v4 == null)
                    {
                        Count++;
                        sb.Append(A + B + C + ",");
                        AddList.Add(new VEHICLE_NEWENERGY_4
                        {
                            ID = Guid.NewGuid().ToString("N"),
                            A = A,
                            B = B,
                            C = C,
                            CREATEDDATE = CreatedTime
                        });
                    }
                }
            }
            try
            {
                using (var cdb = new DbContext<VEHICLE_NEWENERGY_4>(strConnV4))
                {
                    foreach (var item in AddList)
                    {
                        cdb.Set<VEHICLE_NEWENERGY_4>().Add(item);
                    }
                    cdb.SaveChanges();
                    _logger.InfoFormat(string.Format("本次同步任务共同步{0}条!，详细信息{1}", Count, sb.ToString()));
                    cdb.Dispose();
                }
            }
            catch (Exception e)
            {
                _faillogger.Error(string.Format("V4新能源车写入失败,失败原因:{0}", e.Message));
            }
        }
    }
}
