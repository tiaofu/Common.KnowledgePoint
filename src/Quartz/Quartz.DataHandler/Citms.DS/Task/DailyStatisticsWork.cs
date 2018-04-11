using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using log4net;
using Citms.PIS.Model.Vehicle;
using Oracle.ManagedDataAccess.Types;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Reflection;
using Newtonsoft.Json;
using Citms.PIS.Model.Punish;

namespace Citms.DailyStatistics
{
    /// <summary>
    /// 统计
    /// </summary>
    public static class DailyStatisticsWork
    {
        //static string sql = ConfigManage.dbConfig.Sql;
        //private static string DbConn = ConfigManage.dbConfig.DbConn;
        private static List<T> TableToEntity<T>(DataTable dt) where T : class, new()
        {
            Type type = typeof(T);
            List<T> list = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                PropertyInfo[] pArray = type.GetProperties();
                T entity = new T();
                foreach (PropertyInfo p in pArray)
                {
                    if (row[p.Name] is Int64)
                    {
                        p.SetValue(entity, Convert.ToInt32(row[p.Name]), null);
                        continue;
                    }
                    p.SetValue(entity, row[p.Name], null);
                }
                list.Add(entity);
            }
            return list;
        }
        public static void HandlerData()
        {
            #region 统计            
            //分组查询获取昨天的数据值
            string sql = ConfigManage.dbConfig.Sql;
            LogHelper.WriteInfo("查询sql：" + sql);
            string selectSql = string.Format(sql);
            DataTable oldData = new DataTable();
            try
            {
                using (OracleConnection conn = new OracleConnection(ConfigManage.dbConfig.DbConn))
                {
                    OracleDataAdapter oda = new OracleDataAdapter(selectSql, conn);
                    oda.Fill(oldData);
                    conn.Dispose();
                }
            }
            catch (Exception e)
            {
                LogHelper.Error(string.Format("查询数据异常，异常信息：{0}", e.ToString()));
            }
            DataTable table = oldData.Copy();
            if (table.Rows.Count > 0)
            {
                LogHelper.WriteInfo(string.Format("获得数据{0}条", oldData.Rows.Count.ToString()));
                if (ConfigManage.dbConfig.DataType == "1")
                {
                    //转成对象
                    // string json=JsonConvert.SerializeObject(table);
                    List<IllegalVehicleReal> list = null;
                    try
                    {
                        list = DataTableListHelper.ToList<IllegalVehicleReal>(table);
                    }
                    catch (Exception e)
                    {

                    }
                    // List<IllegalVehicleReal> list = JsonConvert.DeserializeObject<List<IllegalVehicleReal>>(json); //TableToEntity<IllegalVehicleReal>(table);
                    //发送mq
                    foreach (IllegalVehicleReal item in list)
                    {
                        item.Remark += "CDP流程重走.";
                        item.Status = "0000";
                        item.ProcessId = Guid.NewGuid().ToString("N");
                        dynamic obj = new { DataType = "IllegalVehicle", Data = item };
                        string value = JsonConvert.SerializeObject(obj);
                        string routingkey = ConfigManage.dbConfig.RoutingKey + "." + item.SpottingId + "." + item.DirectionId + "." + item.LaneNo;
                        RabbitMQClient.SendMessage(routingkey, value, false);
                    }
                }
                else if (ConfigManage.dbConfig.DataType == "2")
                {
                    //转成对象
                    // string json=JsonConvert.SerializeObject(table);
                    List<VehiclePassing> list = null;
                    try
                    {
                        list = DataTableListHelper.ToList<VehiclePassing>(table);
                    }
                    catch (Exception e)
                    {

                    }
                    // List<IllegalVehicleReal> list = JsonConvert.DeserializeObject<List<IllegalVehicleReal>>(json); //TableToEntity<IllegalVehicleReal>(table);
                    //发送mq
                    Parallel.ForEach(list, (item) =>
                    {
                        item.Remark += "过车重发.";
                        dynamic obj = new { DataType = "VehiclePassing", Data = item , ErrorCount = 0 };
                        string value = JsonConvert.SerializeObject(obj);
                        string routingkey = ConfigManage.dbConfig.RoutingKey + "." + item.SpottingId + "." + item.DirectionId + "." + item.LaneNo;
                        RabbitMQClient.SendMessage(routingkey, value, false);
                    });                    
                }
            }
            LogHelper.WriteInfo(string.Format("数据转发处理完成"));
            #endregion
        }
    }
}
