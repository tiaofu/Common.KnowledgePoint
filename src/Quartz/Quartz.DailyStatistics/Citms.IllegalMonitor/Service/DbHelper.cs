using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.Configuration;

namespace Citms.IllegalMonitor
{
    public static class DbHelper
    {
        static string sqlConn = ConfigurationManager.AppSettings["sqlconn"].ToString().Replace("|DataDirectory|", AppDomain.CurrentDomain.BaseDirectory + "App_Data");
        public static List<T> GetList<T>(string strSql, SQLiteParameter para)
        {
            DataTable dataTable = new DataTable();      
            using (SQLiteConnection conn = new SQLiteConnection(sqlConn))
            {
                SQLiteDataAdapter sda = new SQLiteDataAdapter(strSql, conn);
                if (para != null)
                    sda.SelectCommand.Parameters.Add(para);
                sda.Fill(dataTable);
            }
            return DataTableListHelper.ToList<T>(dataTable);
        }
        public static int InsertValue(string sql, SQLiteParameter[] paras)
        {
            using (SQLiteConnection conn = new SQLiteConnection(sqlConn))
            {
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                if (paras != null && paras.Length > 0)
                {                   
                    cmd.Parameters.AddRange(paras);
                }
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
