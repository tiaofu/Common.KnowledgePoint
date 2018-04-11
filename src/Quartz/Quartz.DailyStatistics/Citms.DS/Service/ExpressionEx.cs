using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Citms.Data
{
	public static class ExpressionEx
	{
		public static string ParseOracleSql(this Expression expression,out List<object> paramers)
		{
			return ConvertString(expression,out paramers);
		}
		private static string ConvertString(Expression expression,out List<object> paramers)
		{
			paramers = new List<object>();
			if(expression.CanReduce)
				expression = expression.Reduce();
			if(expression is MethodCallExpression)
			{
				var method = (MethodCallExpression)expression;
				if(method.Method.Name.StartsWith("OrderBy"))
				{
					if(method.Arguments.Count>1)
						return string.Format("Order By {0} {1}",ConvertString(method.Arguments[1],out paramers), method.Method.Name.EndsWith("OrderByDescending") ? "desc" : "");
				}
				return "";
			}
			else if(expression is LambdaExpression)
			{
				var lambda = (LambdaExpression)expression;
				return ConvertString(lambda.Body, out paramers);
			}
			else if(expression is ParameterExpression)
			{
				var parameter = (ParameterExpression)expression;
				return parameter.Type.Name;
			}
			else if(expression is BinaryExpression)
			{
				var binary = (BinaryExpression)expression;
				return ConvertString(binary.Left, out paramers) + " " + binary.NodeType.ToSQL() + " " + ConvertString(binary.Right, out paramers);
			}
			else if(expression is MemberExpression)
			{
				var member = (MemberExpression)expression;
				if(member.Member.MemberType == System.Reflection.MemberTypes.Field)
				{
					var constant = (ConstantExpression)member.Expression;
					if(constant != null)
					{
						paramers.Add(constant.Value.GetType().GetField(member.Member.Name).GetValue(constant.Value));
						return string.Format("{{{0}}}",paramers.Count-1);
					}
					return "null";
				}
				return string.Format("{0}.\"{1}\"",ConvertString(member.Expression, out paramers),member.Member.Name);
			}
			else if(expression is ConstantExpression)
			{
				var constant = (ConstantExpression)expression;
				paramers.Add(constant.Value);
				return string.Format("{{{0}}}", paramers.Count - 1);
			}
			else if(expression is NewExpression)
			{
				var newExpression = (NewExpression)expression;
				var list = new List<string>();
				foreach(var item in newExpression.Arguments)
				{
					list.Add(ConvertString(item, out paramers));
				}
				return string.Join(",", list);
			}
			else if(expression is UnaryExpression)
			{
				var newExpression = (UnaryExpression)expression;
				if(newExpression.NodeType == ExpressionType.Convert)
				{
					return string.Format("_{0}.Convert({1})", newExpression.Type.Name, ConvertString(newExpression.Operand, out paramers));
				}
				return "_?";
			}
			else
				return "_" + expression.GetType().FullName;
		}
	}

	public static class ExpressionTypeEx
	{
		/// <summary>
		/// 把操作符转换成加减乘除什么的。
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string ToSQL(this ExpressionType type)
		{
			switch(type)
			{
				case ExpressionType.Add:
					return "+";
				case ExpressionType.AndAlso:
					return "and";
				case ExpressionType.Equal:
					return "=";
				case ExpressionType.GreaterThan:
					return ">";
				case ExpressionType.GreaterThanOrEqual:
					return ">=";
				case ExpressionType.LessThan:
					return "<";
				case ExpressionType.LessThanOrEqual:
					return "<=";
				case ExpressionType.Multiply:
					return "*";
				case ExpressionType.NotEqual:
					return "is not";
				case ExpressionType.OrElse:
					return "or";
				case ExpressionType.Subtract:
					return "-";
				case ExpressionType.Divide:
					return "/";
				default:
					return "不知道是什么";
			}
		}
	}
}
