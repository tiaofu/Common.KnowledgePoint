using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using log4net;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Citms;
using Citms.IllegalMonitor;

namespace Citms.DailyStatistics
{
    /// <summary>
    /// 统计
    /// </summary>
    public static class DailyStatisticsWork
    {
        private static string DbConn = ConfigManage.SysConfig.DbConn;

        public static void HandlerData()
        {
            #region 统计
            string startTime = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " 00:00:00";
            string datestring = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            string endTime = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
            //分组查询获取昨天的数据值
            string selectSql = string.Format(@"select  SPOTTINGID,ILLEGALTYPENO,LEGALIZEILLEGALTYPENO,count(1) as COUNT from punish_illegalvehicle_real
                                 where OCCURTIME>=to_date('{0}','yyyy-MM-dd HH24:mi:ss')
                                 and OCCURTIME< to_date('{1}', 'yyyy-MM-dd HH24:mi:ss')
                                 group by SPOTTINGID,LEGALIZEILLEGALTYPENO,ILLEGALTYPENO", startTime, endTime);
            DataTable oldData = new DataTable();
            try
            {
                using (OracleConnection conn = new OracleConnection(DbConn))
                {
                    OracleDataAdapter oda = new OracleDataAdapter(selectSql, conn);
                    oda.Fill(oldData);
                    conn.Dispose();
                }
            }
            catch (Exception e)
            {
                LogHelper.Error(string.Format("统计{0}至{1}的违法数据异常，异常信息：{2}", startTime, endTime, e.ToString()));
            }
            DataTable table = oldData.Copy();
            LogHelper.WriteInfo(string.Format("统计{0}至{1}的违法数据，得到数据{2}条", startTime, endTime, table.Rows.Count.ToString()));
            if (table.Rows.Count > 0)
            {
                #region 处理
                table.Columns.Add("OCCERDATE", typeof(System.String));
                table.Columns.Add("SGUID", typeof(System.String));
                table.Columns.Add("CREATEDTIME", typeof(System.DateTime));
                DateTime time = DateTime.Now;               
                //处理违法数据
                foreach (DataRow item in table.Rows)
                {
                    item["OCCERDATE"] = datestring;
                    item["SGUID"] = Guid.NewGuid().ToString("N");
                    item["CREATEDTIME"] = time;
                }
               
                LogHelper.WriteInfo(string.Format("处理统计{0}至{1}的违法数据，处理成功数据{2}条", startTime, endTime, table.Rows.Count.ToString()));
                //插入昨天的统计数据到新表中   
                string delS1l = string.Format("delete PUNISH_ILLEGALVEHICLECOUNT where OCCERDATE='{0}'", datestring);
                int result = 0;
                try
                {
                    using (OracleConnection conn = new OracleConnection(DbConn))
                    {
                        conn.Open();
                        OracleCommand cmd = new OracleCommand(delS1l, conn);
                        result = cmd.ExecuteNonQuery();
                        conn.Dispose();
                    }
                }
                catch (Exception e)
                {
                    LogHelper.Error(string.Format("删除{0}的违法数据异常，异常信息{1}", datestring, e.ToString()));
                }
                LogHelper.WriteInfo(string.Format("删除{0}的违法数据{1}条", datestring, result));
                #endregion
                #region 插入
                //插入
                OracleConnection connection = null;
                OracleCommand command = null;
                OracleTransaction _transcation = null;               
                try
                {
                    using (connection = new OracleConnection(DbConn))
                    {
                        string SQLString = "SELECT SPOTTINGID,ILLEGALTYPENO,LEGALIZEILLEGALTYPENO,COUNT,OCCERDATE,SGUID,CREATEDTIME FROM PUNISH_ILLEGALVEHICLECOUNT where 1=0";
                        using (OracleCommand cmd = new OracleCommand(SQLString, connection))
                        {
                            try
                            {
                                connection.Open();
                                OracleDataAdapter myDataAdapter = new OracleDataAdapter();
                                myDataAdapter.SelectCommand = new OracleCommand(SQLString, connection);
                                myDataAdapter.UpdateBatchSize = 0;
                                DataTable dt = table.Copy();
                                DataTable dtTemp = new DataTable();
                                OracleCommandBuilder custCB = new OracleCommandBuilder(myDataAdapter);
                                myDataAdapter.Fill(dtTemp);                              
                                int times = 0;
                                for (int count = 0; count < dt.Rows.Count;)
                                {
                                    for (int i = 0; i < 400 && 400 * times + i < dt.Rows.Count; i++)
                                    {                                       
                                        dtTemp.Rows.Add(dt.Rows[count].ItemArray);
                                        count++;                                       
                                    }
                                    myDataAdapter.Update(dtTemp);
                                    times++;
                                    dtTemp.Rows.Clear();
                                }
                                dt.Dispose();
                                dtTemp.Dispose();
                                myDataAdapter.Dispose();
                            }
                            catch (System.Data.OracleClient.OracleException E)
                            {
                                connection.Close();
                                LogHelper.Error(string.Format("插入违法统计数据异常，异常信息{0}", E.ToString()));
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    LogHelper.Error(string.Format("插入违法统计数据异常，异常信息{0}", e.ToString()));
                }



                //try
                //{
                //    using (connection = new OracleConnection(DbConn))
                //    {
                //        connection.Open();
                //        _transcation = connection.BeginTransaction();
                //        using (command = connection.CreateCommand())
                //        {
                //            command.CommandText = "SELECT SPOTTINGID,ILLEGALTYPENO,LEGALIZEILLEGALTYPENO,COUNT,OCCERDATE,SGUID,CREATEDTIME FROM PUNISH_ILLEGALVEHICLECOUNT";// string.Format("SELECT {0} FROM PUNISH_ILLEGALVEHICLECOUNT", string.Join(",", columns));
                //            command.Transaction = _transcation;
                //            OracleDataAdapter da = new OracleDataAdapter(command);
                //            OracleCommandBuilder cb = new OracleCommandBuilder(da);
                //            da.InsertCommand = cb.GetInsertCommand(true);
                //            da.Update(table);
                //            _transcation.Commit();
                //        }
                //    }
                //}
                //catch (System.Exception ex)
                //{
                //    LogHelper.Error(string.Format("插入违法统计数据异常，异常信息{0}", ex.ToString()));
                //    if (_transcation != null)
                //    {
                //        _transcation.Rollback();
                //    }
                //    throw ex;
                //}
                #endregion
            }
            LogHelper.WriteInfo(string.Format("违法统计数据处理完成，本次操作{0}数据{1}条", datestring, table.Rows.Count.ToString()));
            IllegalMonitor.IllegalMonitor.HandlerIllegal(table, datestring);
            #endregion
        }
    }
}
