using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OracleClient;

namespace QuartzJob.Data
{
	/// <summary>
	/// Oracle类型
	/// </summary>
	public enum OracleType
	{
		/// <summary>
		/// 游标
		/// </summary>
		Cursor,
		/// <summary>
		/// 空
		/// </summary>
		DbNull
	}
	public static class OracleTypeEx
	{
		public static System.Data.OracleClient.OracleType ToDbType(this OracleType type)
		{
			switch(type)
			{
				case OracleType.Cursor:
					return System.Data.OracleClient.OracleType.Cursor;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
