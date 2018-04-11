using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace QuartzJob.Data
{
	public class MappingInfo
	{
		private XElement _xml;
		private string _mappingFilePath;
		private DateTime _lastUpdateMappingFileTime;
		private List<ClassInfo> _mappingList;
		private readonly object _lock_MappingList = new object();
		private bool _flag_MappingList;

		private MappingInfo()
		{
		}

		public MappingInfo(string directoryName, string configName)
		{
			string fileName;
			if(string.IsNullOrEmpty(directoryName))
				fileName = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(GetType()).Location), configName + ".mapping");
			else
				fileName = Path.Combine(directoryName, configName + ".mapping");

			_mappingFilePath = fileName;
			var fi = new System.IO.FileInfo(fileName);
			if(!fi.Exists)
				throw new FileNotFoundException("指定的目录下未找到当前文件",fileName);
			_lastUpdateMappingFileTime = fi.LastWriteTime;
			Init(fi);
		}

		private void Init(FileInfo fi)
		{
			var read = fi.OpenText();
			var txt = read.ReadToEnd();
			read.Close();
			_xml = XElement.Parse(txt);

			_mappingList = new List<ClassInfo>();
			Dictionary<string, string> dicJoin = new Dictionary<string, string>();
			foreach(var item in _xml.Elements())
			{
				ClassInfo info = new ClassInfo();
				info.ClassName = item.Name.ToString();
				if(item.Attribute("Table") != null && !string.IsNullOrEmpty(item.Attribute("Table").Value))
					info.TableName = item.Attribute("Table").Value.Trim();
				else if(item.Attribute("Procedure") != null && !string.IsNullOrEmpty(item.Attribute("Procedure").Value))
				{
					info.TableName = item.Attribute("Procedure").Value.Trim();
					info.IsProcedure = true;
				}
				else
					continue;
				if(item.Attribute("Assembly") != null && !string.IsNullOrEmpty(item.Attribute("Assembly").Value))
					info.Assembly = item.Attribute("Assembly").Value;
				else
					info.Assembly = "System.Object,mscorlib";
				info.PropertyInfoList = new List<ClassPropertyInfo>();
				foreach(var property in item.Elements())
				{
					ClassPropertyInfo propertyInfo = new ClassPropertyInfo();
					propertyInfo.ClassPropertyName = property.Name.ToString();
					if(property.Attribute("ConstructorName") != null && !string.IsNullOrEmpty(property.Attribute("ConstructorName").Value))
					{
						propertyInfo.ConstructorName = property.Attribute("ConstructorName").Value;
						propertyInfo.PassedIntoConstructor = true;
					}

					if(property.Attribute("PKColumn") != null && !string.IsNullOrEmpty(property.Attribute("PKColumn").Value))
					{
						propertyInfo.TableColumnName = property.Attribute("PKColumn").Value;
						propertyInfo.IsPKColumn = true;
					}
					if(property.Attribute("OutPut") != null && !string.IsNullOrEmpty(property.Attribute("OutPut").Value.Trim()))
					{
						propertyInfo.TableColumnName = property.Attribute("OutPut").Value.Trim();
						propertyInfo.IsOutPutParamer = true;
					}
					if(property.Attribute("Column") != null )
						propertyInfo.TableColumnName = property.Attribute("Column").Value.Trim();
					if(property.Attribute("Join") != null && !string.IsNullOrEmpty(property.Attribute("Join").Value.Trim()))
					{
						propertyInfo.IsFKColumn = true;
						dicJoin.Add(string.Format("{0},{1}",info.ClassName,propertyInfo.ClassPropertyName), property.Attribute("Join").Value);
					}
					if(property.Attribute("Set") != null && !string.IsNullOrEmpty(property.Attribute("Set").Value.Trim()))
						propertyInfo.SetClassPropertyName = property.Attribute("Set").Value.Trim();

					if(property.Attribute("Nullable") != null && !string.IsNullOrEmpty(property.Attribute("Nullable").Value.Trim()))
					{
						bool flag;
						propertyInfo.Nullable = bool.TryParse(property.Attribute("Nullable").Value, out flag) ? flag : false;
					}
					else
						propertyInfo.Nullable = false;

					if(property.Attribute("DbType") != null && !string.IsNullOrEmpty(property.Attribute("DbType").Value.Trim()))
						propertyInfo.DbType = property.Attribute("DbType").Value.Trim();
					if(property.Attribute("Size") != null && !string.IsNullOrEmpty(property.Attribute("Size").Value.Trim()))
					{
						int size;
						propertyInfo.Size = int.TryParse(property.Attribute("Size").Value.Trim(),out size)?(int?)size:null;
					}

					info.PropertyInfoList.Add(propertyInfo);
				}
				_mappingList.Add(info);
			}
			_mappingList.ForEach(p =>
			{
				p.PropertyInfoList.ForEach(pp =>
				{
					if(pp.IsFKColumn)
					{
						var joinTableName=dicJoin[string.Format("{0},{1}", p.ClassName, pp.ClassPropertyName)];
						var classInfo = _mappingList.FirstOrDefault(c => c.TableName == joinTableName);
						if(classInfo != null)
						{
							pp.Join = classInfo;
							pp.JoinColumn = classInfo.PropertyInfoList.FirstOrDefault(pi => pi.IsPKColumn);
						}
						else
						{
							classInfo = _mappingList.FirstOrDefault(c => joinTableName.StartsWith(c.TableName));
							if(classInfo != null)
							{
								pp.Join = classInfo;
								pp.JoinColumn = classInfo.PropertyInfoList.FirstOrDefault(pi => joinTableName.EndsWith(pi.TableColumnName));
							}
						}
					}
				});
			});
		}
		/// <summary>
		/// 对应关系集合
		/// </summary>
		public List<ClassInfo> MappingList
		{
			get
			{
				var fi = new System.IO.FileInfo(_mappingFilePath);
				if(_lastUpdateMappingFileTime != fi.LastWriteTime)
				{
					lock(_lock_MappingList)
					{
						while(_flag_MappingList)
						{ }
						_flag_MappingList = true;
					}
					if(_lastUpdateMappingFileTime == fi.LastWriteTime)
					{
						_flag_MappingList = false;
						return _mappingList;
					}
					_lastUpdateMappingFileTime = fi.LastWriteTime;
					Init(fi);
					_flag_MappingList = false;
				}
				return _mappingList;
			}
		}
	}
	/// <summary>
	/// 类详情
	/// </summary>
	public class ClassInfo
	{
		/// <summary>
		/// 类信息
		/// </summary>
		public string Assembly
		{
			get;
			set;
		}
		/// <summary>
		/// 类名
		/// </summary>
		public string ClassName
		{
			get;
			set;
		}
		/// <summary>
		/// 表名
		/// </summary>
		public string TableName
		{
			get;
			set;
		}
		/// <summary>
		/// 是否是存储过程
		/// </summary>
		public bool IsProcedure
		{
			get;
			set;
		}
		/// <summary>
		/// 属性集合
		/// </summary>
		public List<ClassPropertyInfo> PropertyInfoList
		{
			get;
			set;
		}
	}

	/// <summary>
	/// 属性详情
	/// </summary>
	public class ClassPropertyInfo
	{
		/// <summary>
		/// 类中属性名称
		/// </summary>
		public string ClassPropertyName
		{
			get;
			set;
		}
		/// <summary>
		/// 表中列名
		/// </summary>
		public string TableColumnName
		{
			get;
			set;
		}
		/// <summary>
		/// 数据类型
		/// </summary>
		public string DbType
		{
			get;
			set;
		}
		/// <summary>
		/// 数据大小
		/// </summary>
		public int? Size
		{
			get;
			set;
		}
		/// <summary>
		/// 是否为主键
		/// </summary>
		public bool IsPKColumn
		{
			get;
			set;
		}

		/// <summary>
		/// 是否传入构造函数
		/// </summary>
		public bool PassedIntoConstructor
		{
			get;
			set;
		}
		/// <summary>
		/// 构造函数对应参数名称
		/// </summary>
		public string ConstructorName
		{
			get;
			set;
		}
		/// <summary>
		/// 是否为输出参数
		/// </summary>
		public bool IsOutPutParamer
		{
			get;
			set;
		}
		/// <summary>
		/// 是否为外键
		/// </summary>
		public bool IsFKColumn
		{
			get;
			set;
		}
		/// <summary>
		/// 是否可为空
		/// </summary>
		public bool Nullable
		{
			get;
			set;
		}
		/// <summary>
		/// 外键关联主表
		/// </summary>
		public ClassInfo Join
		{
			get;
			set;
		}
		/// <summary>
		/// 外键关联列
		/// </summary>
		public ClassPropertyInfo JoinColumn
		{
			get;
			set;
		}
		/// <summary>
		/// 要将关联对像赋值给当前名称指定的属性
		/// </summary>
		public string SetClassPropertyName
		{
			get;
			set;
		}
	}
}
