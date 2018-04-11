using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Data;
using System.Configuration;

using System.Data.OracleClient;
namespace QuartzJob.Data
{
	internal class DataAccess
	{
		private string _configName;

		public Configuration Config
		{
			set
			{
				DBHelper.Default.Config = value;
			}
		}

		public string ConfigName
		{
			set
			{
				_configName = value;
			}
		}

		/// <summary>
		/// 执行增删改操作
		/// </summary>
		/// <param name="formatSql">复合格式查询语句</param>
		/// <param name="paramers"></param>
		/// <returns></returns>
		public int Execute(string formatSql, params Parameter[] paramers)
		{
			if(string.IsNullOrEmpty(formatSql))
				throw new ArgumentNullException("formatSql");

			WriteSQL(formatSql, paramers);

			using(var connection = DBHelper.Default.GetConnection(_configName))
			{
				var command = this.CreateCommand(formatSql, paramers, connection);
				connection.Open();
				try
				{
					command.Transaction = connection.BeginTransaction();
					var i = command.ExecuteNonQuery();
					if(i > 0)
						command.Transaction.Commit();
					else
						command.Transaction.Rollback();
					return i;
				}
				finally
				{
					if(connection != null)
						connection.Close();
				}
			}
		}

		/// <summary>
		/// 写入Blob数据
		/// </summary>
		/// <param name="selectByPKFormatSql">查询要修改某行数据的语句</param>
		/// <param name="blobColumnNames">Blob字段的列名</param>
		/// <param name="paramers">查询的条件和Blob字段的值</param>
		/// <returns>返回成功的行数</returns>
		public int WriteBlob(string selectByPKFormatSql,List<string> blobColumnNames, Dictionary<Parameter[],List<byte[]>> paramers)
		{
			int result = 0;
			using(var connection = DBHelper.Default.GetConnection(_configName))
			{
				foreach(var paramer in paramers)
				{
					try
					{
						var command = this.CreateCommand(selectByPKFormatSql, paramer.Key , connection);
						connection.Open();
						command.Transaction = connection.BeginTransaction();
						using(var reader = command.ExecuteReader())
						{
							int i=0;
							foreach(var columnName in blobColumnNames)
							{
								var buffer = paramer.Value[i];
								while(reader.Read())
									reader.GetOracleLob(reader.GetOrdinal(columnName)).Write(buffer,0,buffer.Length);
								i++;
							}
						}
						command.Transaction.Commit();
						command.Transaction.Dispose();
						result++;
					}
					catch
					{
					}
					finally
					{
						if(connection != null)
							connection.Close();
					}
				}
			}
			return result;
		}

		private OracleCommand CreateCommand(string formatSql, Parameter[] paramers, OracleConnection connection)
		{
			return CreateCommand(formatSql, false, paramers, connection);
		}

		private OracleCommand CreateCommand(string formatSql, bool isStoreProcedure, Parameter[] paramers, OracleConnection connection)
		{
			connection = connection ?? DBHelper.Default.GetConnection(_configName);
			var command = connection.CreateCommand();

			if(isStoreProcedure)
				command.CommandType = CommandType.StoredProcedure;
			if(paramers != null && paramers.Length != 0)
			{
				var dic = paramers.Select((p, i) => new
				{
					Name = string.IsNullOrEmpty(p.Name) ? ("p" + i) : p.Name,
					p.DbType,
					p.IsOutPut,
					p.IsInoutPut,
					p.Value,
					p.Size
				}).ToDictionary(p => isStoreProcedure ? p.Name : (":" + p.Name), p =>
				{
					OracleParameter paramer = null;
					if(p.Value is QuartzJob.Data.OracleType&&(QuartzJob.Data.OracleType)p.Value== QuartzJob.Data.OracleType.Cursor)
					{
						paramer = new OracleParameter(p.Name, ((QuartzJob.Data.OracleType)p.Value).ToDbType());
						paramer.Direction = ParameterDirection.Output;
						if(p.Size.HasValue)
							paramer.Size = p.Size.Value;
						return paramer;
					}
					if(string.IsNullOrEmpty(p.DbType) && p.Value != null)
					{
						if(p.Value is byte[])
						{
							paramer = new OracleParameter(p.Name, System.Data.OracleClient.OracleType.Blob);
							paramer.Value = p.Value;
						}
						else
							paramer = new OracleParameter(p.Name, ConvertValue(p.Value));
					}
					else if(!string.IsNullOrEmpty(p.DbType))
					{
						paramer = new OracleParameter(p.Name, GetDbType(p.DbType));
						paramer.Value = ConvertValue(p.Value);
					}
					else
						return null;
					if(p.Size.HasValue)
						paramer.Size = p.Size.Value;
					if(p.IsOutPut)
						paramer.Direction = ParameterDirection.Output;
					else if(p.IsInoutPut)
						paramer.Direction = ParameterDirection.InputOutput;
					return paramer;
				});
				command.CommandText = string.Format(formatSql, dic.Keys.ToArray());
				command.Parameters.AddRange(dic.Values.Where(p => p != null).ToArray());
			}
			else
				command.CommandText = formatSql;
			return command;
		}

		private object ConvertValue(object value)
		{
			if(value == null||value is QuartzJob.Data.OracleType&&(QuartzJob.Data.OracleType)value== QuartzJob.Data.OracleType.DbNull)
				return null;
			Type type = value.GetType();
			if(type == typeof(Guid))
				return ((Guid)value).ToByteArray();
			else if(type == typeof(Guid?) && ((Guid?)value).HasValue)
				return ((Guid?)value).Value.ToByteArray();
			return value;
		}

		private System.Data.OracleClient.OracleType GetDbType(string dbType)
		{
			if(dbType.Equals("BFile", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.BFile;
			if(dbType.Equals("Blob", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.Blob;
			if(dbType.Equals("Char", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.Char;
			if(dbType.Equals("Clob", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.Clob;
			if(dbType.Equals("Cursor", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.Cursor;
			if(dbType.Equals("DateTime", StringComparison.OrdinalIgnoreCase) || dbType.Equals("Date", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.DateTime;
			if(dbType.Equals("IntervalDayToSecond", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.IntervalDayToSecond;
			if(dbType.Equals("IntervalYearToMonth", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.IntervalYearToMonth;
			if(dbType.Equals("LongRaw", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.LongRaw;
			if(dbType.Equals("LongVarChar", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.LongVarChar;
			if(dbType.Equals("NChar", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.NChar;
			if(dbType.Equals("NClob", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.NClob;
			if(dbType.Equals("Number", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.Number;
			if(dbType.Equals("NVarChar", StringComparison.OrdinalIgnoreCase)||dbType.Equals("NVarChar2", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.NVarChar;
			if(dbType.Equals("Raw", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.Raw;
			if(dbType.Equals("RowId", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.RowId;
			if(dbType.Equals("Timestamp", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.Timestamp;
			if(dbType.Equals("TimestampLocal", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.TimestampLocal;
			if(dbType.Equals("TimestampWithTZ", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.TimestampWithTZ;
			if(dbType.Equals("VarChar", StringComparison.OrdinalIgnoreCase)||dbType.Equals("VarChar2", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.VarChar;
			if(dbType.Equals("Byte", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.Byte;
			if(dbType.Equals("UInt16", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.UInt16;
			if(dbType.Equals("UInt32", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.UInt32;
			if(dbType.Equals("SByte", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.SByte;
			if(dbType.Equals("Int16", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.Int16;
			if(dbType.Equals("Int32", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.Int32;
			if(dbType.Equals("Float", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.Float;
			if(dbType.Equals("Double", StringComparison.OrdinalIgnoreCase))
				return System.Data.OracleClient.OracleType.Double;
			throw new ArgumentOutOfRangeException();
		}

		/// <summary>
		/// 执行查找操作
		/// </summary>
		/// <param name="formatSql">复合格式查询语句</param>
		/// <param name="paramers"></param>
		/// <returns></returns>
		public DataTable Select(string formatSql, params Parameter[] paramers)
		{
			if(string.IsNullOrEmpty(formatSql))
				throw new ArgumentNullException("formatSql");

			WriteSQL(formatSql, paramers);

			var table = new DataTable();
			using(var connection = DBHelper.Default.GetConnection(_configName))
			{
				var command = CreateCommand(formatSql, paramers, connection);
				var adapter = new OracleDataAdapter(command);
				adapter.Fill(table);
				adapter.Dispose();
				command.Dispose();
			}
			return table;
		}
		/// <summary>
		/// 执行存储过程
		/// </summary>
		/// <param name="procedureName">存储过程名</param>
		/// <param name="paramers">输入参数</param>
		/// <param name="outParamers">输出参数</param>
		/// <returns></returns>
		public DataTable ExecuteProcedure(string procedureName, Parameter[] paramers, out Dictionary<string, object> outParamers)
		{
			if(string.IsNullOrEmpty(procedureName))
				throw new ArgumentNullException("procedureName");
			var table = new DataTable();
			using(var connection = DBHelper.Default.GetConnection(_configName))
			{
				var command = CreateCommand(procedureName, true, paramers, connection);
				var adapter = new OracleDataAdapter(command);
				adapter.Fill(table);
				outParamers = command.Parameters.Cast<OracleParameter>()
					.Where(p => p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.InputOutput)
					.ToDictionary(p => p.ParameterName, p => p.Value);
				adapter.Dispose();
				command.Dispose();
			}
			return table;
		}
		/// <summary>
		/// 执行存储过程
		/// </summary>
		/// <param name="procedureName">存储过程名</param>
		/// <param name="paramers">输入参数</param>
		/// <param name="outParamers">输出参数</param>
		public int ExecuteProcedure(string procedureName, Parameter[] paramers, out Dictionary<string, object> outParamers,out OracleString rowid)
		{
			if(string.IsNullOrEmpty(procedureName))
				throw new ArgumentNullException("procedureName");
			var table = new DataTable();
			using(var connection = DBHelper.Default.GetConnection(_configName))
			{
				var command = CreateCommand(procedureName, true, paramers, connection);
				connection.Open();
				try
				{
					var count = command.ExecuteOracleNonQuery(out rowid);
					outParamers = command.Parameters.Cast<OracleParameter>()
						.Where(p => p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.InputOutput)
						.ToDictionary(p => p.ParameterName, p => p.Value is OracleDataReader? CursorUtitly.Resolve(p.Value):p.Value);
					return count;
				}
				finally
				{
					if(connection != null)
						connection.Close();
				}
			}
		}

		public void WriteSQL(string sql, params Parameter[] paramers)
		{
			if(!DBHelper.Default.WriteSql)
				return;
			try
			{
				var paramersStr = paramers == null ? "" :
					string.Join(Environment.NewLine, paramers.Select(p => string.Format("Name={0},Size={1},Value={2},DbType={3},IsInoutPut={4},IsOutPut={5}", p.Name, p.Size, p.Value, p.DbType, p.IsInoutPut, p.IsOutPut)));

				var str = string.Format("{0}/////////////////{0}{1}{0}/////////////////{0}{2}{0}{3}{0}", Environment.NewLine,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), sql, paramersStr);
				var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Citms.Data.sql.txt");
				System.IO.File.AppendAllText(path, str,Encoding.UTF8);

				var fi = new System.IO.FileInfo(path);
				if(fi.Length > 1024 * 1024)
				{
					var oldPath = path.Insert(path.LastIndexOf('.'), "(old)");
					if(System.IO.File.Exists(oldPath))
						System.IO.File.Delete(oldPath);
					fi.MoveTo(oldPath);
				}
			}
			catch
			{ }
		}

		/// <summary>
		/// 参数
		/// </summary>
		public class Parameter
		{
			public Parameter(object value, string dbType = null, string name = null, bool isOutPut = false, bool isInOutPut = false, int? size = null)
			{
				Value = value;
				Name = name;
				DbType = dbType;
				IsOutPut = isOutPut;
				IsInoutPut = isInOutPut;
				Size = size;
			}
			/// <summary>
			/// 名称
			/// </summary>
			public string Name
			{
				get;
				set;
			}
			/// <summary>
			/// 类型
			/// </summary>
			public string DbType
			{
				get;
				set;
			}
			/// <summary>
			/// 是否是输出参数
			/// </summary>
			public bool IsOutPut
			{
				get;
				set;
			}
			/// <summary>
			/// 是否是输入输出参数
			/// </summary>
			public bool IsInoutPut
			{
				get;
				set;
			}
			/// <summary>
			/// 值
			/// </summary>
			public object Value
			{
				get;
				set;
			}
			/// <summary>
			/// 大小
			/// </summary>
			public int? Size
			{
				get;
				set;
			}
		}
	}
}
