
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using System.Data.Entity;

namespace QuartzJob.Contexts
{
    public class EntitiesContext : DbContext
    {
        public static string User
        {
            get;
            private set;
        }

        static OracleConnection CreateConnection(string connectionString)
        {    
            OracleConnectionStringBuilder oracleBuilder = new OracleConnectionStringBuilder();
            oracleBuilder.ConnectionString = connectionString;// SysConfig.DefaultConnection;
            EntitiesContext.User = oracleBuilder.UserID.ToUpper();
            OracleConnection conn = new OracleConnection(oracleBuilder.ConnectionString);
            return conn;
        }     

        public EntitiesContext(DbConnection conn) : base(conn, contextOwnsConnection: false)
        {
            // 自定义数据库数据库连接
        }

        public EntitiesContext(string connectionString): this(CreateConnection(connectionString))
        {

        }
    }
}
