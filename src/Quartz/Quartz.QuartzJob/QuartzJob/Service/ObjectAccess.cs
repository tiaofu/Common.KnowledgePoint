using System;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Zongsoft.Data;
using Zongsoft.Data.Entities;
using Zongsoft.ComponentModel;

namespace QuartzJob.Data
{
	public class ObjectAccess : Zongsoft.Data.Entities.IObjectAccess
	{
		#region 私有变量
		private DataAccess _db;
		private MappingInfo _mappingInfo;
		private Configuration _configuration;
		private string _configName;
		#endregion

		#region 构造函数
		public ObjectAccess(string name, Configuration configuration)
		{
			_configuration = configuration;
			_configName = name;
		}
		#endregion

		#region 属性
		private DataAccess DB
		{
			get
			{
				if(_db == null)
				{
					_db = new DataAccess();
					_db.Config = _configuration;
					_db.ConfigName = _configName;
				}
				return _db;
			}
		}
		private MappingInfo MappingInfo
		{
			get
			{
				if(_mappingInfo == null)
				{
					var element = _configuration.AppSettings.Settings["MappingFileDirectoryName"];
					_mappingInfo = new MappingInfo(element == null ? "" : element.Value,_configName);
				}
				return _mappingInfo;
			}
		}
		#endregion

		#region 查询方法
		public IEnumerable Select(ICondition condition)
		{
			if(Selecting != null)
				Selecting(this, new SelectingEventArgs(condition));

			var ps = Math.Max(condition.PageSize, 1);
			var pi = Math.Max(condition.PageIndex, 1);

			var info = this.MappingInfo.MappingList.FirstOrDefault(p => p.ClassName.Equals(condition.Name, StringComparison.CurrentCultureIgnoreCase));
			Type type;
			if(info == null)
				return null;
			else
				type = Type.GetType(info.Assembly);
			var properties = GetColumnFromEntity(condition).Where(p => p.Value != null).ToDictionary(p => p.Key, p => p.Value);

			if(info.IsProcedure)
			{
				IDictionary<string, object> outParameters;
				var procedureResult = this.ExecuteProcedure<object>(condition.Name, properties, out outParameters);
				foreach(var item in outParameters)
				{
					if(condition.Output.ContainsKey(item.Key))
						condition.Output[item.Key] = item.Value;
					else
						condition.Output.Add(item);
				}
				return procedureResult;
			}

			List<string> selectColumns = new List<string>();
			int tableCount = 0;
			Dictionary<string, string> tableExMapping = new Dictionary<string, string>();
			var joinSql = GetJoinSql("T2", "J", ref tableCount, "", info, selectColumns, tableExMapping, "");

			var parameters = new List<object>();
			var where = GetWhere("T", condition.ConditionCombine.ToString(), properties, parameters, condition.FuzzyInquiryEnabled, tableExMapping);

			List<object> orderByOutParameter;
			var tableName = GetTableNameFromEntityName(condition.Name);
			var dbParams = parameters.Select(p => new DataAccess.Parameter(p)).ToArray();
			var orderyStr = condition.Expression is Expression ? ((Expression)condition.Expression).ParseOracleSql(out orderByOutParameter) : "";
			var table = DB.Select(string.Format("select {0} from (select T1.*,ROWNUM RN from (select T.* from \"{1}\" T {3} {4}) T1 where {5}>=ROWNUM) T2 {2} where T2.RN>{6} {7}"
				, string.Join(",", selectColumns)
				, tableName
				, joinSql
				, where
				, orderyStr.Replace(string.Format("{0}.", condition.Name), "T.")
				, ps * pi
				, ps * (pi - 1)
				, orderyStr.Replace(string.Format("{0}.", condition.Name), "T2.")
				)
				, dbParams);

			var cols = table.Columns.Cast<DataColumn>();
			var result = table.Rows.Cast<DataRow>().Select(row =>
				cols.ToDictionary(col => col.ColumnName, col => row[col.ColumnName])
				).Select(p => SetEntityValue("", type, p, tableExMapping));

			table = DB.Select(string.Format("select count(0) from \"{0}\" T {1}"
				, tableName
				//, joinSql
				, where)
				, dbParams);
			if(table != null && table.Rows.Count != 0)
			{
				condition.TotalCount = Convert.ToInt32(table.Rows[0][0]);
				condition.PageCount = (int)Math.Ceiling(condition.TotalCount * 1.0 / condition.PageSize);
			}

			if(Selected != null)
				Selected(this, new SelectedEventArgs(condition, result));
			return result;
		}

		public IEnumerable<TEntity> Select<TEntity>(ICondition condition)
		{
			var result = this.Select(condition);
			if(result == null)
				return null;
			return result.Cast<TEntity>();
		}

		public IEnumerable<TEntity> Select<TEntity>(IDictionary<string, object> inParameters)
		{
			IDictionary<string, object> outParameters;
			return this.Select<TEntity>(inParameters, out outParameters);
		}

		public IEnumerable Select(string name, IDictionary<string, object> inParameters)
		{
			IDictionary<string, object> outParameters;
			return this.Select(name, inParameters, out outParameters);
		}

		public IEnumerable Select(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParamters)
		{
			if(Selecting != null)
				Selecting(this, new SelectingEventArgs(name, inParameters, null));

			var __Key__ = inParameters.Where(p => p.Key.StartsWith("__") && p.Key.EndsWith("__") && p.Key != "__" || p.Value is Expression).ToDictionary(p => p.Key, p => p.Value);
			inParameters = inParameters.Where(p => !__Key__.ContainsKey(p.Key) && p.Value != null).ToDictionary(p => p.Key, p => p.Value);
			var info = this.MappingInfo.MappingList.FirstOrDefault(p => p.ClassName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
			if(info == null)
			{
				outParamters = new Dictionary<string, object>();
				return null;
			}
			var dic = inParameters.ToDictionary(p =>
			{
				var item = info.PropertyInfoList.FirstOrDefault(pp => pp.ClassPropertyName.Equals(p.Key,StringComparison.CurrentCultureIgnoreCase) || !string.IsNullOrEmpty(pp.SetClassPropertyName) && p.Key.StartsWith(pp.SetClassPropertyName));
				if(item != null)
					return string.Format("{0}{1}", !string.IsNullOrEmpty(item.SetClassPropertyName) && p.Key.StartsWith(item.SetClassPropertyName) ? item.SetClassPropertyName + "." : "", item.TableColumnName);
				return p.Key;
			}, p => p.Value);

			if(info.IsProcedure)
				return ExecuteProcedure<object>(name, dic, out outParamters);

			string criteria = __Key__.ContainsKey("__Criteria__") ? __Key__["__Criteria__"].ToString() : "and";
			int? pageIndex = __Key__.ContainsKey("__PageIndex__") ? (int?)__Key__["__PageIndex__"] : null;
			if(pageIndex.HasValue)
				pageIndex = Math.Max(pageIndex.Value, 1);
			int? pageSize = __Key__.ContainsKey("__PageSize__") ? (int?)__Key__["__PageSize__"] : null;
			if(pageSize.HasValue)
				pageSize = Math.Max(pageSize.Value, 1);
			bool fuzzyInquiryEnabled = __Key__.ContainsKey("__FuzzyInquiryEnabled__") ? (bool)__Key__["__FuzzyInquiryEnabled__"] : false;


			outParamters = new Dictionary<string, object>();

			List<string> selectColumns = new List<string>();
			int tableCount = 0;
			Dictionary<string, string> tableExMapping = new Dictionary<string, string>();
			var joinSql = GetJoinSql("T2", "J", ref tableCount, "", info, selectColumns, tableExMapping, "");
			List<object> parameters = new List<object>();
			var where = GetWhere("T", criteria, dic, parameters, fuzzyInquiryEnabled, tableExMapping);

			var orderby = __Key__.FirstOrDefault(p => p.Value is Expression);
			//var orderby = __Key__.FirstOrDefault(p => p.Value is Expression&&((MethodCallExpression)p.Value).Method.Name.StartsWith("OrderBy"));
			List<object> orderByOutParameter;
			var orderbyStr = orderby.Value != null ? ((Expression)orderby.Value).ParseOracleSql(out orderByOutParameter) : "";
			var sql = string.Format("select T.* from \"{0}\" T {1} {2}", info.TableName, where, orderbyStr.Replace(string.Format("{0}.", name), "T."));
			if(!pageIndex.HasValue || !pageSize.HasValue)
				sql = string.Format("select {0} from ({1}) T2 {2} {3}", string.Join(",", selectColumns), sql, joinSql, orderbyStr.Replace(string.Format("{0}.", name), "T2."));
			else
				sql = string.Format("select {1} from (select T1.*,ROWNUM RN from ({0}) T1 where {3}>=ROWNUM) T2 {2} where T2.RN>{4} {5}", sql, string.Join(",", selectColumns), joinSql, pageIndex.Value * pageSize.Value, pageSize.Value * (pageIndex.Value - 1), orderbyStr.Replace(string.Format("{0}.", name), "T2."));
			var table = this.DB.Select(sql, parameters.Select(p => new DataAccess.Parameter(p)).ToArray());

			var cols = table.Columns.Cast<DataColumn>();
			var result = table.Rows.Cast<DataRow>().Select(row =>
				cols.ToDictionary(col => col.ColumnName, col => row[col.ColumnName])
				).Select(p => SetEntityValue("", Type.GetType(info.Assembly), p, tableExMapping));
			
			if(pageIndex.HasValue && pageSize.HasValue)
			{
				table = this.DB.Select(string.Format("select count(0) from \"{0}\" T {1}", info.TableName, where), parameters.Select(p => new DataAccess.Parameter(p)).ToArray());
				if(table != null && table.Rows.Count != 0)
				{ 
					var totalCount=Convert.ToInt32(table.Rows[0][0]);
					outParamters.Add("TotalCount", totalCount);
					outParamters.Add("PageCount", (int)Math.Ceiling(totalCount * 1.0 / pageSize.Value));
				}
			}

			if(Selected != null)
				Selected(this, new SelectedEventArgs(name, inParameters, null, result));

			return result;
		}

		public IEnumerable<TEntity> Select<TEntity>(IDictionary<string, object> inParameters, out IDictionary<string, object> outParamters)
		{
			var result = Select(typeof(TEntity).Name, inParameters, out outParamters);
			if(result == null)
				return null;
			return result.Cast<TEntity>();
		}
		#endregion

		#region 删除方法
		public int Delete(ICondition condition)
		{
			var dic = GetColumnFromEntity(condition.Name, condition);
			foreach(var item in typeof(ICondition).GetProperties())
			{
				dic.Remove(dic.Keys.FirstOrDefault(p => p.TableColumnName == item.Name));
			}
			return Delete(condition.Name, dic.ToDictionary(p => p.Key.TableColumnName, p => p.Value));
		}

		public int Delete(string name, IDictionary<string, object> parameters)
		{
			string where = "";
			var list = new List<object>();
			if(parameters != null && parameters.Count != 0)
			{
				where = string.Join(" and ", parameters.Where(p => !string.IsNullOrEmpty(p.Key)).Select(p =>
				{
					var str = "";
					if(p.Value == null)
						str = string.Format("\"{0}\" is null", p.Key);
					else if(p.Value is Array)
					{
						var array = ((Array)p.Value).Cast<object>();
						str = string.Format("\"{0}\" in ({1})", p.Key, string.Join(",", array.Select((pp, i) => "{" + (list.Count + i) + "}")));
						list.AddRange(array);
					}
					else
					{
						str = string.Format("\"{0}\"={{{1}}}", p.Key, list.Count);
						list.Add(p.Value);
					}
					return str;
				}));
			}

			if(!string.IsNullOrEmpty(where))
				where = " where " + where;

			return DB.Execute(string.Format("delete from \"{0}\" {1}", GetTableNameFromEntityName(name), where), list.Select(p => new DataAccess.Parameter(p)).ToArray());
		}

		public int Delete(string name, IEnumerable entities)
		{
			var info = this.MappingInfo.MappingList.FirstOrDefault(p => p.ClassName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
			if(info == null)
				return -1;

			StringBuilder sb = new StringBuilder();
			sb.Append("begin ");
			List<object> paramers = new List<object>();
			var property = info.PropertyInfoList.FirstOrDefault(p => p.IsPKColumn);
			var pkColumns = info.PropertyInfoList.Where(p => p.IsPKColumn);
			foreach(var item in entities)
			{
				var where = string.Join(" and ", pkColumns.Select(p =>
				{
					var propertyInfo = item.GetType().GetProperty(p.ClassPropertyName);
					return new
					{
						Name = p.TableColumnName,
						Value = (object)(propertyInfo != null ? propertyInfo.GetValue(item, null) : null)
					};
				}).Where(p => p.Value != null).Select(p =>
				{
					var str = "";
					if(p.Value is Array)
					{
						var array = ((Array)p.Value).Cast<object>();
						str = string.Format("\"{0}\" in ({1})", p.Name, string.Join(",", array.Select((pp, i) => "{" + (paramers.Count + i) + "}")));
						paramers.AddRange(array);
					}
					else
					{
						str = string.Format("\"{0}\"={{{1}}}", p.Name, paramers.Count);
						paramers.Add(p.Value);
					}
					return str;
				}));

				sb.Append(string.Format("delete from \"{0}\" where {1};", info.TableName, where));
			}
			sb.Append(" end;");
			return this.DB.Execute(sb.ToString(), paramers.Select(p => new DataAccess.Parameter(p)).ToArray());
		}

		public int Delete(string name, object parameters)
		{
			return this.Delete(name, parameters is IEnumerable?(IEnumerable)parameters:new object[] { parameters });
		}

		public int Delete(IEnumerable entities)
		{
			return this.Delete(entities.Cast<object>().FirstOrDefault().GetType().Name, entities);
		}

		public int Delete(object entity)
		{
			return this.Delete(entity.GetType().Name, new object[] { entity });
		}

		public int Delete<TEntity>(IDictionary<string, object> parameters)
		{
			return this.Delete(typeof(TEntity).Name, parameters);
		}

		public int Delete<TEntity>(object parameters)
		{
			return this.Delete(typeof(TEntity).Name, parameters);
		}
		#endregion

		#region 插入方法
		public int Insert(object entity)
		{
			return this.Insert(entity.GetType().Name, new object[] { entity });
		}

		public int Insert(IEnumerable entities)
		{
			return this.Insert(entities.Cast<object>().FirstOrDefault().GetType().Name, entities);
		}

		public int Insert(string name, object entity)
		{
			return this.Insert(name, new object[] { entity });
		}

		public int Insert<TEntity>(IEnumerable entities)
		{
			return this.Insert(typeof(TEntity).Name, entities);
		}

		public int Insert<TEntity>(object entity)
		{
			return this.Insert(typeof(TEntity).Name, entity);
		}

		public int Insert(string name, IEnumerable entities)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("begin ");
			List<DataAccess.Parameter> paramers = new List<DataAccess.Parameter>();
			var tableName = GetTableNameFromEntityName(name);
			List<string> blobColumnNames = null;
			List<string> pkColumnNames = new List<string>();
			var writeBlobDic = new Dictionary<DataAccess.Parameter[], List<byte[]>>();
			foreach(var item in entities)
			{
				var dic = GetColumnFromEntity(name, item).Where(p => p.Value != null).ToDictionary(p => p.Key, p => p.Value);
				var temp = dic.Where(p => !string.IsNullOrWhiteSpace(p.Key.DbType) && p.Key.DbType.Equals("blob", StringComparison.OrdinalIgnoreCase));
				if(blobColumnNames == null)
				{
					blobColumnNames = temp.Select(p => p.Key.TableColumnName).ToList();
					pkColumnNames = dic.Where(p => p.Key.IsPKColumn).Select(p => p.Key.TableColumnName).OrderBy(p => p).ToList();
				}
				writeBlobDic.Add(dic.Where(p => p.Key.IsPKColumn).OrderBy(p => p.Key.TableColumnName).Select(p => new DataAccess.Parameter(p.Value)).ToArray(), temp.Select(p => (byte[])p.Value).ToList());
				sb.Append(string.Format("insert into \"{0}\"({1}) values({2});",
					tableName,
					string.Join(",", dic.Keys
						.Where(p => string.IsNullOrWhiteSpace(p.DbType) || !p.DbType.Equals("blob", StringComparison.OrdinalIgnoreCase))
						.Select(p => string.Format("\"{0}\"", p.TableColumnName))
						.Concat(dic.Keys
							.Where(p => !string.IsNullOrWhiteSpace(p.DbType) && p.DbType.Equals("blob", StringComparison.OrdinalIgnoreCase))
							.Select(p => string.Format("\"{0}\"", p.TableColumnName))
						)
					),
					string.Join(",", dic
						.Where(p => string.IsNullOrWhiteSpace(p.Key.DbType) || !p.Key.DbType.Equals("blob", StringComparison.OrdinalIgnoreCase))
						.Select((p, i) => string.Format("{{{0}}}", paramers.Count + i))
						.Concat(dic
							.Where(p => !string.IsNullOrWhiteSpace(p.Key.DbType) && p.Key.DbType.Equals("blob", StringComparison.OrdinalIgnoreCase))
							.Select(p => "empty_blob()")
						)
					)
				));
				paramers.AddRange(dic
					.Where(p => string.IsNullOrWhiteSpace(p.Key.DbType) || !p.Key.DbType.Equals("blob", StringComparison.OrdinalIgnoreCase))
					.Select(p => new DataAccess.Parameter(p.Value, p.Key.DbType, size: p.Key.Size)));
			}
			sb.Append(" end;");
			var insertCount = DB.Execute(sb.ToString(), paramers.ToArray());
			if(blobColumnNames.Count != 0)
				DB.WriteBlob(string.Format("select * from \"{0}\" where {1} for update", tableName, string.Join(" and ", pkColumnNames.Select((p, i) => string.Format("\"{0}\"={{{1}}}", p, i)))), blobColumnNames, writeBlobDic);
			return insertCount;
		}
		#endregion

		#region 更新方法
		public int Update(object entity)
		{
			return this.Update(entity.GetType().Name, new object[] { entity });
		}

		public int Update(IEnumerable entities)
		{
			return this.Update(entities.Cast<object>().FirstOrDefault().GetType().Name, entities);
		}

		public int Update(string name, object entity)
		{
			return this.Update(name, new object[] { entity });
		}

		public int Update<TEntity>(IEnumerable entities, object parameters)
		{
			return this.Update(typeof(TEntity).Name, entities, parameters);
		}

		public int Update<TEntity>(object entity, object parameters)
		{
			return this.Update(typeof(TEntity).Name, entity, parameters);
		}

		public int Update<TEntity>(IEnumerable entities, IDictionary<string, object> parameters)
		{
			return this.Update(typeof(TEntity).Name, entities, parameters);
		}

		public int Update<TEntity>(object entity, IDictionary<string, object> parameters)
		{
			return this.Update(typeof(TEntity).Name, entity, parameters);
		}

		public int Update<TEntity>(IEnumerable entities)
		{
			return this.Update(typeof(TEntity).Name, entities);
		}

		public int Update<TEntity>(object entity)
		{
			return this.Update(typeof(TEntity).Name, entity);
		}

		public int Update(string name, IEnumerable entities, object parameters)
		{
			return this.Update(name, entities, parameters.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(parameters, null)));
		}

		public int Update(string name, object entity, object parameters)
		{
			return this.Update(name, new object[]{entity}, parameters);
		}

		public int Update(string name, IEnumerable entities, IDictionary<string, object> parameters)
		{
			var sb = new StringBuilder();
			sb.Append("begin ");
			List<DataAccess.Parameter> paramers = new List<DataAccess.Parameter>();
			var info = this.MappingInfo.MappingList.FirstOrDefault(p => p.ClassName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
			var tempList = new List<object>();
			if(info != null)
				parameters = parameters.ToDictionary(p =>
				{
					var item = info.PropertyInfoList.FirstOrDefault(pp => pp.ClassPropertyName.Equals(p.Key, StringComparison.CurrentCultureIgnoreCase));
					if(item == null)
						return p.Key;
					return item.TableColumnName;
				}, p => p.Value);
			var where = GetWhere("T", "AND", parameters, tempList, false, null);
			paramers.AddRange(tempList.Select(p => new DataAccess.Parameter(p)));

			string tableName = GetTableNameFromEntityName(name);
			foreach(var item in entities)
			{
				var dic = GetColumnFromEntity(name, item);
				var temp = dic.Where(p => p.Value != null);
				var list = temp.Select((p, i) => string.Format("\"{0}\"={{{1}}}", p.Key.TableColumnName, paramers.Count + i)).ToList();
				paramers.AddRange(temp.Select(p => new DataAccess.Parameter(p.Value, p.Key.DbType, size: p.Key.Size)));

				list.AddRange(dic.Where(p => p.Value == null).Select(p => string.Format("\"{0}\"=NULL", p.Key.TableColumnName)));

				var sql = string.Format("update \"{0}\" T set {1}{2};", tableName, string.Join(",", list), where);
				sb.Append(sql);
			}
			sb.Append(" end;");
			return DB.Execute(sb.ToString(), paramers.ToArray());
		}

		public int Update(string name, object entity, IDictionary<string, object> parameters)
		{
			return this.Update(null, new object[] { entity }, parameters);
		}

		public int Update(string name, IEnumerable entities)
		{
			var sb = new StringBuilder();
			sb.Append("begin ");
			List<DataAccess.Parameter> paramers = new List<DataAccess.Parameter>();
			string tableName = GetTableNameFromEntityName(name);
			foreach(var item in entities)
			{
				var dic = GetColumnFromEntity(name, item);
				var pkList = GetPKFromEntity(name, item);
				var temp = dic.Where(p => p.Value != null && !pkList.Contains(p.Key.TableColumnName));
				var list = temp.Select((p, i) => string.Format("\"{0}\"={{{1}}}", p.Key.TableColumnName, paramers.Count + i)).ToList();
				paramers.AddRange(temp.Select(p => new DataAccess.Parameter(p.Value, p.Key.DbType, size: p.Key.Size)));

				list.AddRange(dic.Where(p => p.Value == null && !pkList.Contains(p.Key.TableColumnName)).Select(p => string.Format("\"{0}\"=NULL", p.Key.TableColumnName)));

				temp = dic.Where(p => p.Value != null && pkList.Contains(p.Key.TableColumnName));
				var tempParamers=paramers.Select(p=>(object)p).ToList();
				var where = GetWhere("T", "AND", temp.ToDictionary(p => p.Key.TableColumnName, p => p.Value), tempParamers, false, null);
				paramers = tempParamers.Select(p => p is DataAccess.Parameter ? (DataAccess.Parameter)p : new DataAccess.Parameter(p)).ToList();

				var sql = string.Format("update \"{0}\" T set {1}{2};", tableName, string.Join(",", list), where);
				sb.Append(sql);
			}
			sb.Append(" end;");
			return DB.Execute(sb.ToString(), paramers.ToArray());
		}
		#endregion

		#region 执行
		public void Execute(ICondition condition)
		{
			var properties = GetColumnFromEntity(condition.Name, condition);
			foreach(var item in typeof(ICondition).GetProperties())
			{
				properties.Remove(properties.Keys.FirstOrDefault(p => p.TableColumnName == item.Name));
			}
			IDictionary<string, object> outParamters;
			Execute(condition.Name, properties.ToDictionary(p => p.Key.TableColumnName, p => p.Value), out  outParamters);
			foreach(var item in outParamters)
			{
				condition.Output.Add(item);
			}
		}

		public void Execute(string name, IDictionary<string, object> inParameters)
		{
			IDictionary<string, object> outParameters;
			this.Execute(name, inParameters, out outParameters);
		}

		public void Execute(string name, IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters)
		{
			var info = this.MappingInfo.MappingList.FirstOrDefault(p => p.ClassName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
			IEnumerable<ClassPropertyInfo> outPropertyInfos;
			if(info != null)
			{
				name = info.TableName;
				outPropertyInfos = info.PropertyInfoList.Where(p => p.IsOutPutParamer);
			}
			else
				outPropertyInfos = new List<ClassPropertyInfo>();
			Dictionary<string, object> dic;
			var paramers = inParameters.Where(p => p.Value != null).Select(p =>
			{
				var parameterName = p.Key;
				var dbType = "";
				bool isInOutPut = false;
				int? size = null;
				if(info != null)
				{
					var item = info.PropertyInfoList.FirstOrDefault(pp => pp.ClassPropertyName.Equals(p.Key,StringComparison.CurrentCultureIgnoreCase));
					if(item != null)
					{
						parameterName = item.TableColumnName;
						dbType = item.DbType;
						isInOutPut = item.IsOutPutParamer;
						size = item.Size;
					}
				}
				return new DataAccess.Parameter(p.Value, dbType, parameterName, false, isInOutPut, size);
			}).ToList();
			paramers.AddRange(outPropertyInfos.Where(p => !inParameters.ContainsKey(p.ClassPropertyName)).Select(p => new DataAccess.Parameter(null, p.DbType, p.TableColumnName, true, false, p.Size)));
			System.Data.OracleClient.OracleString rowid;
			DB.ExecuteProcedure(name, paramers.ToArray(), out dic, out rowid);
			outParameters = dic.ToDictionary(p => outPropertyInfos.FirstOrDefault(pp => pp.TableColumnName == p.Key).ClassPropertyName, p => p.Value);
		}

		public IEnumerable<TResult> ExecuteProcedure<TResult>(string entityName, IDictionary<string, object> inParameters, out IDictionary<string, object> outParamters)
		{
			var info = this.MappingInfo.MappingList.FirstOrDefault(p => p.ClassName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase));
			string name;
			Type type = null;
			IEnumerable<ClassPropertyInfo> outPropertyInfos;
			if(info != null)
			{
				name = info.TableName;
				type = Type.GetType(info.Assembly);
				outPropertyInfos = info.PropertyInfoList.Where(p => p.IsOutPutParamer);
			}
			else
			{
				outParamters = new Dictionary<string, object>();
				return null;
			}
			Dictionary<string, object> dic;
			var paramers = inParameters.Where(p => p.Value != null).Select(p =>
			{
				var parameterName = p.Key;
				var dbType = "";
				bool isInOutPut = false;
				int? size = null;
				if(info != null)
				{
					var item = info.PropertyInfoList.FirstOrDefault(pp => pp.TableColumnName.Equals(p.Key,StringComparison.CurrentCultureIgnoreCase));
					if(item != null)
					{
						parameterName = item.TableColumnName;
						dbType = item.DbType;
						isInOutPut = item.IsOutPutParamer;
						size = item.Size;
					}
				}
				return new DataAccess.Parameter(p.Value, dbType, parameterName, false, isInOutPut, size);
			}).ToList();
			paramers.AddRange(info.PropertyInfoList.Where(p => !inParameters.ContainsKey(p.TableColumnName)).Select(p => new DataAccess.Parameter(null, p.DbType, p.TableColumnName, p.IsOutPutParamer, false, p.Size)));

			var table = DB.ExecuteProcedure(name, paramers.ToArray(), out dic);
			outParamters = dic.ToDictionary(p => outPropertyInfos.FirstOrDefault(pp => pp.TableColumnName == p.Key).ClassPropertyName, p => p.Value);
			var cols = table.Columns.Cast<DataColumn>();
			
			var result = table.Rows.Cast<DataRow>().Select(row => (TResult)SetEntityValue("", type, cols.ToDictionary(col => col.ColumnName, col => row[col.ColumnName])));
			return result;
		}
		#endregion

		#region 私有方法
		private object SetEntityValue(string parent, Type entityType, Dictionary<string, object> values, Dictionary<string, string> tableExMapping = null, string propertyName = null)
		{
			var info = this.MappingInfo.MappingList.FirstOrDefault(p => p.ClassName.Equals(entityType.Name, StringComparison.CurrentCultureIgnoreCase));

			var properties=entityType.GetProperties();

			Dictionary<string, object> tempValues = null;
			var tableExMappingKey = (string.IsNullOrEmpty(parent) ? "" : (parent + ".")) + (string.IsNullOrEmpty(propertyName) ? entityType.Name : propertyName);
			var tableExMappingValue = "";
			if(tableExMapping != null)
			{
				if(tableExMapping.ContainsKey(tableExMappingKey))
				{
					tableExMappingValue = tableExMapping[tableExMappingKey];
					if(!string.IsNullOrEmpty(tableExMappingValue))
						tableExMappingValue += "_";
					tempValues = values.Where(p => p.Key.StartsWith(tableExMappingValue)).ToDictionary(p => p.Key, p => p.Value);
				}
			}
			else
			{
				#region 这样的情况下也可以为导行属性赋值
				/*
				 * class A
				 * {
				 *		public string P{get;set;}
				 *		public B B{get;set;}
				 * }
				 * class B
				 * {
				 *		public string P{get;set;}
				 * }
				 * class C
				 * {
				 *		public string P{get;set;}
				 *		public B B{get;set;}
				 *		public A A{get;set;}
				 * }
				 * 以上三个类分别对应名字相同的三张表
				 * sql 语句
				 * select c.P,a.P,b.P,b.P from A a,B b,C c;
				 * 查询结果为C类型实例赋值，C.P，C.A,C.A.P,C.A.B,C.A.B.P,C.B,C.B.P都会有值
				 * 为导行属性赋值的顺序为属性名字的升序排列，
				 * 所以C类的A属性先赋值，C类的B属性后赋值，
				 * 所以查询语句的列先是所有C的非导行属性，然后是A的非导行属性，然后是A里面B的非导行属性，然后再是B的非导行属性
				 */
				#endregion

				var count = properties.Where(p =>
				{
					var propertyInfo = info == null ? null : info.PropertyInfoList.FirstOrDefault(pp => pp.ClassPropertyName.Equals(p.Name, StringComparison.CurrentCultureIgnoreCase));
					var isClass = p.PropertyType.IsClass && p.PropertyType != typeof(string) && !p.PropertyType.IsArray;
					return p.CanWrite &&!isClass&& (propertyInfo == null || !string.IsNullOrEmpty(propertyInfo.TableColumnName));
				}).Count();
				tempValues = values.Take(count).ToDictionary(p=>p.Key,p=>p.Value);
				var temp = values.Skip(count).ToList();
				values.Clear();
				foreach(var item in temp)
				{
					values.Add(item.Key, item.Value);
				}
			}
			if(tempValues == null)
				tempValues = values;
			//获取当前类型公共构造函数中参数最少的构造函数的参数集合
			var cpinfo = entityType.GetConstructors().Where(p => p.IsPublic).Select(p => p.GetParameters()).OrderBy(p => p.Length).FirstOrDefault();
			var instanceArgs = new object[cpinfo.Length];
			if(info != null)
			{
				info.PropertyInfoList.Where(p => p.PassedIntoConstructor).ToList().ForEach(p =>
				{
					var tempValue = tempValues.FirstOrDefault(pp => pp.Key == tableExMappingValue+p.ClassPropertyName);
					if(cpinfo != null && cpinfo.Any(pp => pp.Name == p.ConstructorName))
					{
						var args = cpinfo.FirstOrDefault(pp => pp.Name == p.ConstructorName);
						if(args != null)
							instanceArgs[args.Position] = tempValue.Value;
					}
				});
			}
			var entity = Activator.CreateInstance(entityType, instanceArgs);

			foreach(var entityTypeProperty in properties.OrderBy(p=>p.Name))
			{
				if(!entityTypeProperty.CanWrite)
					continue;
				var isClass = entityTypeProperty.PropertyType.IsClass && entityTypeProperty.PropertyType != typeof(string) && !entityTypeProperty.PropertyType.IsArray;
				if(tableExMapping != null && isClass)
				{
					var property = info == null ? null : info.PropertyInfoList.FirstOrDefault(p => p.SetClassPropertyName==entityTypeProperty.Name);
					if(tableExMapping.ContainsKey(tableExMappingKey + "." + (property == null ? entityTypeProperty.Name : property.SetClassPropertyName)))
					{
						var entityValue = SetEntityValue(tableExMappingKey, entityTypeProperty.PropertyType, values, tableExMapping, property == null ? "" : property.SetClassPropertyName);
						entityTypeProperty.SetValue(entity, entityValue, null);
						continue;
					}
				}
				else if(isClass&&values.Count>0)
				{
					var entityValue = SetEntityValue("", entityTypeProperty.PropertyType, values, null, "");
					entityTypeProperty.SetValue(entity, entityValue, null);
					continue;
				}
				var propertyInfo = info == null ? null : info.PropertyInfoList.FirstOrDefault(p => p.ClassPropertyName.Equals(entityTypeProperty.Name,StringComparison.CurrentCultureIgnoreCase));

				if(propertyInfo != null && string.IsNullOrEmpty(propertyInfo.TableColumnName))
					continue;
				var tempValue = tempValues.FirstOrDefault(p => p.Key == tableExMappingValue + (propertyInfo!=null?propertyInfo.TableColumnName:entityTypeProperty.Name));
				if(tempValue.Value == null || tempValue.Value is System.DBNull)
				{
					//列重名的情况，如查询语句为select a.Name,b.Name from a,b 生成的列名分别为Name,Name1
					tempValue = tempValues.FirstOrDefault(p => p.Key.StartsWith(tableExMappingValue + (propertyInfo!=null?propertyInfo.TableColumnName:entityTypeProperty.Name)));
					if(tempValue.Value==null||tempValue.Value is System.DBNull)
						continue;
				}
				object propertyValue;

				#region 把tempValue.Value转换为propertyValue，已替换为Zongsoft.Common.Convert.ConvertValue方法
				//if(entityTypeProperty.PropertyType == tempValue.Value.GetType())
				//    propertyValue = tempValue.Value;
				//else if((entityTypeProperty.PropertyType == typeof(bool) || entityTypeProperty.PropertyType == typeof(bool?)) && tempValue.Value.GetType() == typeof(decimal))
				//    propertyValue = (decimal)tempValue.Value == 0 ? false : true;
				//else if((entityTypeProperty.PropertyType == typeof(Guid) || entityTypeProperty.PropertyType == typeof(Guid?)) && tempValue.Value.GetType() == typeof(byte[]))
				//    propertyValue = new Guid((byte[])tempValue.Value);
				//else if(entityTypeProperty.PropertyType.IsEnum)
				//{
				//    if(tempValue.Value.GetType() == typeof(decimal) || tempValue.Value.GetType() == typeof(string))
				//        propertyValue = Enum.Parse(entityTypeProperty.PropertyType, tempValue.Value.ToString());
				//    else
				//        throw new ArgumentOutOfRangeException(entityTypeProperty.Name, string.Format("无法从类型{0}转换为类型{1}", entityTypeProperty.PropertyType, tempValue.Value.GetType()));
				//}
				//else if(entityTypeProperty.PropertyType.IsValueType || entityTypeProperty.PropertyType == typeof(string))
				//{
				//    tempValue.Value = tempValue.Value.ToString();
				//    propertyValue = System.ComponentModel.TypeDescriptor.GetConverter(entityTypeProperty.PropertyType).ConvertFrom(tempValue.Value);
				//}
				//else
				//    propertyValue = System.ComponentModel.TypeDescriptor.GetConverter(entityTypeProperty.PropertyType).ConvertFrom(tempValue.Value);
				#endregion

				propertyValue = Zongsoft.Common.Convert.ConvertValue(tempValue.Value, entityTypeProperty.PropertyType);
				entityTypeProperty.SetValue(entity, propertyValue, null);
			}
			return entity;
		}

		private Dictionary<string, object> GetColumnFromEntity(object entity, string parent = null)
		{
			var entityName = ((entity is ICondition) ? ((ICondition)entity).Name : entity.GetType().Name);
			var info = this.MappingInfo.MappingList.FirstOrDefault(p => p.ClassName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase));

			Dictionary<string, object> result = new Dictionary<string, object>();
			var properties = entity.GetType().GetProperties();
			List<string> conditionProperties = (entity is ICondition) ? typeof(ICondition).GetProperties().Select(p => p.Name).ToList() : null;
			foreach(var item in properties)
			{
				object value;
				if((entity is ICondition) && item.Name == "Name" && (value = item.GetValue(entity, null)) != null && value.ToString() == entityName || (entity is ICondition) && item.Name != "Name" && conditionProperties.Contains(item.Name))
					continue;
				if(item.PropertyType.IsClass && item.PropertyType != typeof(string) && !item.PropertyType.IsArray)
				{
					ClassPropertyInfo temp = info == null ? null : info.PropertyInfoList.FirstOrDefault(p => p.SetClassPropertyName==item.Name);
					var tempValue = item.GetValue(entity, null);
					if(tempValue == null)
						continue;
					var tempDic = GetColumnFromEntity(tempValue, entityName + "." + (temp != null ? temp.SetClassPropertyName : (value = (item.GetValue(entity, null) as ICondition)) != null ? ((ICondition)value).Name : item.PropertyType.Name));
					foreach(var dicItem in tempDic)
					{
						result.Add((string.IsNullOrEmpty(parent) ? "" : (parent + ".")) + dicItem.Key, dicItem.Value);
					}
				}
				else
				{
					ClassPropertyInfo temp = info == null ? null : info.PropertyInfoList.FirstOrDefault(pp => pp.ClassPropertyName.Equals(item.Name,StringComparison.CurrentCultureIgnoreCase));
					if(temp != null)
						result.Add((string.IsNullOrEmpty(parent) ? "" : (parent + ".")) + (string.IsNullOrEmpty(temp.TableColumnName) ? item.Name : temp.TableColumnName), item.GetValue(entity, null));
					else
						result.Add((string.IsNullOrEmpty(parent) ? "" : (parent + ".")) + item.Name, item.GetValue(entity, null));
				}
			}
			return result;
		}

		private Dictionary<ClassPropertyInfo, object> GetColumnFromEntity(string name, object entity)
		{
			Type type = entity.GetType();
			var info = this.MappingInfo.MappingList.FirstOrDefault(p => p.ClassName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
			var temp=type.GetProperties().Where(p => !p.PropertyType.IsClass || p.PropertyType == typeof(string) || p.PropertyType.IsArray);
			if(info == null)
				return temp.ToDictionary(p => new ClassPropertyInfo
				{
					ClassPropertyName = p.Name,
					TableColumnName = p.Name
				}, p => p.GetValue(entity, null));
			return temp.Select(p =>
			{
				var item = info.PropertyInfoList.FirstOrDefault(pp => pp.ClassPropertyName.Equals(p.Name,StringComparison.CurrentCultureIgnoreCase));
				if(item != null && string.IsNullOrEmpty(item.TableColumnName))
					return null;
				return new
				{
					key = item != null ? item : new ClassPropertyInfo()
					{
						ClassPropertyName = p.Name,
						TableColumnName = p.Name
					},
					value = p.GetValue(entity, null)
				};
			}).Where(p => p != null).ToDictionary(p => p.key, p => p.value);
		}

		private string GetTableNameFromEntityName(string entityName)
		{
			var info = this.MappingInfo.MappingList.FirstOrDefault(p => p.ClassName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase));
			if(info != null)
				return info.TableName;
			return entityName;
		}

		private List<string> GetPKFromEntity(string name, object entity)
		{
			var type = entity.GetType();
			var info = this.MappingInfo.MappingList.FirstOrDefault(p => p.ClassName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
			List<string> pkList = new List<string>();
			if(info != null)
				pkList = info.PropertyInfoList.Where(p => p.IsPKColumn).Select(p => p.TableColumnName).ToList();
			if(pkList == null || pkList.Count == 0)
				pkList.Add(type.GetProperties()[0].Name);
			return pkList;
		}

		private string GetJoinSql(string joinFromName, string joinToName, ref int joinCount, string parent, ClassInfo currentInfo, List<string> selectColumns, Dictionary<string, string> tableExMapping, string propertyName)
		{
			var tableExMappingKey = (string.IsNullOrEmpty(parent) ? "" : parent + ".") + (string.IsNullOrEmpty(propertyName) ? currentInfo.ClassName : propertyName);
			tableExMapping.Add(tableExMappingKey, joinFromName);
			var entityType = Type.GetType(currentInfo.Assembly);
			
			selectColumns.AddRange(currentInfo.PropertyInfoList.Where(p => !string.IsNullOrEmpty(p.TableColumnName)).
				GroupBy(p => p.TableColumnName).
				Select(p => string.Format("{0}.\"{1}\" \"{0}_{2}\"", joinFromName, p.Key, p.Key)));
			
			selectColumns.AddRange(entityType.GetProperties().Where(p => 
				(!p.PropertyType.IsClass || p.PropertyType == typeof(string) || p.PropertyType.IsArray) 
				&& !currentInfo.PropertyInfoList.Select(pp => pp.ClassPropertyName).Contains(p.Name,StringComparer.CurrentCultureIgnoreCase))
				.Select(p => string.Format("{0}.\"{1}\" \"{0}_{1}\"", joinFromName, p.Name)));

			var items = currentInfo.PropertyInfoList.Where(p => p.IsFKColumn).GroupBy(p => p.Join);
			List<string> result = new List<string>();
			foreach(var item in items)
			{
				var list = item.GroupBy(p => p.SetClassPropertyName);
				foreach(var temp in list)
				{
					string tableEx = joinToName + (joinCount++);
					var setClassPropertyName = "";
					if(temp.Count() == 1 && !string.IsNullOrEmpty(temp.FirstOrDefault().SetClassPropertyName))
						setClassPropertyName = temp.FirstOrDefault().SetClassPropertyName;

					result.Add(string.Format("{0} Join \"{1}\" {2} on {3}", temp.FirstOrDefault().Nullable ? "Left" : "Inner", item.Key.TableName, tableEx, string.Join(" and ", temp.Select(p => string.Format("{0}.\"{1}\"={2}.\"{3}\"", joinFromName, p.TableColumnName, tableEx, p.JoinColumn.TableColumnName)))));
					result.Add(GetJoinSql(tableEx, joinToName, ref joinCount, tableExMappingKey, item.Key, selectColumns, tableExMapping, setClassPropertyName));
				}
			}
			return string.Join(" ", result.Where(p => !string.IsNullOrWhiteSpace(p)));
		}

		private string GetWhere(string defaultTableEx, string criteria, IDictionary<string, object> dic, List<object> parameters, bool fuzzyInquiryEnabled, Dictionary<string, string> tableExMapping)
		{
			var where = "";
			if(dic.Count != 0)
			{
				var tempStrList = new List<string>();
				var tempValueList = new List<object>();
				var list = dic.Where(p => p.Value != null).Select(p =>
				{
					var columnName = p.Key;
					if(p.Key.Contains('.'))
					{
						return string.Empty;//由于把inner join放到分页之后，所以where条件里不能加入其它表的条件
						var lastIndex = p.Key.LastIndexOf('.');
						var key = p.Key.Substring(0, lastIndex);
						if(tableExMapping != null && tableExMapping.ContainsKey(key))
						{
							defaultTableEx = tableExMapping[key];
							columnName = p.Key.Substring(lastIndex + 1, p.Key.Length - lastIndex - 1);
						}
					}
					var str = "";
					var value = p.Value;
					if(p.Value is DateTime[])
						value = ((DateTime[])p.Value).Select(v => (DateTime?)v).ToArray();
					if(value is DateTime?[])
					{
						var tempStr = string.Empty;
						var array = (DateTime?[])value;

						if(array.Length == 1 && array[0].HasValue)
						{
							tempStr = string.Format("{0}.\"{1}\"=#", defaultTableEx, columnName);
							tempValueList.Add(array[0].Value);
						}
						else if(array.Length == 2)
						{
							if(array[0].HasValue)
							{
								tempStr = string.Format("{0}.\"{1}\" >= #", defaultTableEx, columnName);
								tempValueList.Add(array[0].Value);
							}
							if(array[1].HasValue)
							{
								tempStr += string.Format("{2}{0}.\"{1}\"<#", defaultTableEx, columnName, array[0].HasValue ? " and " : "");
								tempValueList.Add(array[1].Value);
							}
						}
						else
							return string.Empty;
						if(!string.IsNullOrEmpty(tempStr))
							tempStrList.Add(tempStr);
					}
					else if(p.Value is int?[])
					{
						var array = (int?[])p.Value;

						if(array.Length == 1 && array[0].HasValue)
						{
							str = string.Format("{0}.\"{1}\"={{{2}}}", defaultTableEx, columnName, parameters.Count);
							parameters.Add(array[0].Value);
						}
						else if(array.Length == 2)
						{
							if(array[0].HasValue)
							{
								str = string.Format("{0}.\"{1}\" >= {{{2}}}", defaultTableEx, columnName, parameters.Count);
								parameters.Add(array[0].Value);
							}
							if(array[1].HasValue)
							{
								str += string.Format("{3}{0}.\"{1}\"<{{{2}}}", defaultTableEx, columnName, parameters.Count, array[0].HasValue ? " and " : "");
								parameters.Add(array[1].Value);
							}
						}
						else
							return string.Empty;
					}
					else if(p.Value is IEnumerable && p.Value.GetType() != typeof(string))
					{
						var array = ((IEnumerable)p.Value).Cast<object>();
						if(array.Count() == 0)
							return string.Format("{0}.\"{1}\" in (null)", defaultTableEx, columnName);
						str = string.Format("{0}.\"{1}\" in ({2})", defaultTableEx, columnName, string.Join(",", array.Select((pp, i) => "{" + (parameters.Count + i) + "}")));
						parameters.AddRange(array);
					}
					else if(p.Value == DBNull.Value)
					{
						str = string.Format("{0}.\"{1}\" is NULL", defaultTableEx, columnName);
					}
					else if(p.Value is string && p.Value.ToString().Equals("NONULL", StringComparison.OrdinalIgnoreCase))
					{
						str = string.Format("{0}.\"{1}\" is not NULL", defaultTableEx, columnName);
					}
					else if(p.Value is string && fuzzyInquiryEnabled && p.Value.ToString().IndexOfAny("_%".ToArray()) >= 0)
					{
						str = string.Format("{0}.\"{1}\" like {{{2}}}", defaultTableEx, columnName, parameters.Count);
						parameters.Add(p.Value);
					}
					else
					{
						str = string.Format("{0}.\"{1}\"={{{2}}}", defaultTableEx, columnName, parameters.Count);
						parameters.Add(p.Value);
					}
					return str;
				}).Where(p => !string.IsNullOrEmpty(p)).ToList();
				list.AddRange(dic.Where(p => p.Value == null).Select(p =>
				{

					var columnName = p.Key;
					if(p.Key.Contains('.'))
					{
						return string.Empty;//由于把inner join放到分页之后，所以where条件里不能加入其它表的条件
						var lastIndex = p.Key.LastIndexOf('.');
						var key = p.Key.Substring(0, lastIndex);
						if(tableExMapping != null && tableExMapping.ContainsKey(key))
						{
							defaultTableEx = tableExMapping[key];
							columnName = p.Key.Substring(lastIndex + 1, p.Key.Length - lastIndex - 1);
						}
					}
					return string.Format("{0}.\"{1}\" is null", defaultTableEx, columnName);
				}));
				int count = 0;
				list.AddRange(tempStrList.Select((p, i) =>
				{
					var index = 0;
					while((index = p.IndexOf('#', index)) >= 0)
					{
						p = p.Remove(index, 1).Insert(index, string.Format("{{{0}}}", parameters.Count + count++));
					}
					return p;
				}));
				parameters.AddRange(tempValueList);
				where = string.Join(string.Format(" {0} ", criteria), list);
			}
			if(!string.IsNullOrWhiteSpace(where))
				where = " Where " + where;
			return where;
		}
		#endregion

		#region 事件
		public event EventHandler<SelectedEventArgs> Selected;

		public event EventHandler<SelectingEventArgs> Selecting;
		#endregion
	}
}
