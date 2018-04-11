using System;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.OracleClient;

namespace QuartzJob.Data
{
	internal class DBHelper
	{
		private static DBHelper _default;
		private Configuration _config;
		private bool? _writeSql;

		private DBHelper()
		{
		}

		public OracleConnection GetConnection(string configName)
		{
			var settings = _config.ConnectionStrings.ConnectionStrings[configName];

			if(settings == null)
				throw new Exception("配置文件中未找到名称为"+configName+"的配置节");

			return new OracleConnection()
			{
				ConnectionString = settings.ConnectionString
			};
		}

		public static DBHelper Default
		{
			get
			{
				if(_default==null)
					Interlocked.CompareExchange<DBHelper>(ref _default, new DBHelper(), null);
				return _default;
			}
		}

		public Configuration Config
		{
			set
			{
				_config = value;
			}
		}

		/// <summary>
		/// 是否要记录sql
		/// </summary>
		public bool WriteSql
		{
			get
			{
				if(_writeSql.HasValue)
					return _writeSql.Value;

				var flag = false;
				if(_config != null && _config.AppSettings.Settings.AllKeys.Contains("WriteSql"))
					bool.TryParse(_config.AppSettings.Settings["WriteSql"].Value, out flag);

				_writeSql = flag;
				return _writeSql.Value;
			}
		}
	}
}
