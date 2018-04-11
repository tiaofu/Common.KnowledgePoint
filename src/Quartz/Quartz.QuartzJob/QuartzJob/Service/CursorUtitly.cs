using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuartzJob.Data
{
	public class CursorUtitly
	{
		public static List<Dictionary<string, object>> Resolve(object cursor)
		{
			var reader=cursor as System.Data.OracleClient.OracleDataReader;
			if(reader ==null)
				return null;
			var result = new List<Dictionary<string, object>>();
			while(reader.Read())
			{
				var dic = new Dictionary<string, object>();
				for(int i = 0; i < reader.FieldCount; i++)
				{
					dic.Add(reader.GetName(i), reader[i]);
				}
				result.Add(dic);
			}
			reader.Close();
			return result;
		}
	}
}
