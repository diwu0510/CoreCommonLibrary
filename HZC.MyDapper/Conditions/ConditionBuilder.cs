using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HZC.MyDapper.Conditions
{
    public class ConditionBuilder
    {
        #region 常量

        private const string ParameterNamePrefix = "__p_";

        #endregion

        #region 字段

        private int _index;

        private readonly string _tableName;

        // 参数的前缀
        private readonly string _parameterPrefix;

        private readonly DynamicParameters _parameters = new DynamicParameters();

        private readonly StringBuilder _whereBuilder = new StringBuilder();

        #endregion

        #region 构造函数

        public ConditionBuilder(string parameterPrefix = "@", string tableName = "")
        {
            _parameterPrefix = parameterPrefix;
            _tableName = tableName;
        }

        public static ConditionBuilder New(string parameterPrefix = "@", string tableName = "")
        {
            return new ConditionBuilder(parameterPrefix, tableName);
        }

        #endregion

        #region 私有公共方法

        private string BuildParameterName()
        {
            return $"{ParameterNamePrefix}{_index++}";
        }

        private string BuildFullFieldName(string fieldName, string tableName = "")
        {
            if (!string.IsNullOrWhiteSpace(tableName)) return $"[{tableName}].[{fieldName}]";

            return string.IsNullOrWhiteSpace(_tableName) ? $"[{fieldName}]" : $"[{_tableName}].[{fieldName}]";
        }

        private ConditionBuilder And(string conditionString)
        {
            if (string.IsNullOrWhiteSpace(conditionString))
            {
                return this;
            }

            if (_whereBuilder.Length > 0)
            {
                _whereBuilder.Append(" AND ");
            }

            _whereBuilder.Append(conditionString);

            return this;
        }

        private ConditionBuilder And(string field, string op, object value, string tableName = "")
        {
            var parameterName = BuildParameterName();
            var fullFiledName = BuildFullFieldName(field, tableName);

            And($"{fullFiledName}{op}{_parameterPrefix}{parameterName}");
            _parameters.Add(parameterName, value);

            return this;
        }

        #endregion

        #region 输出

        public string ToCondition()
        {
            return _whereBuilder.Length == 0 ? "1=1" : _whereBuilder.ToString();
        }

        public DynamicParameters ToParameters()
        {
            return _parameters;
        }

        #endregion

        #region And

        public ConditionBuilder AndEqual(string field, object value, string tableName = "")
        {
            return And(field, "=", value, tableName);
        }

        public ConditionBuilder AndNotEqual(string field, object value, string tableName = "")
        {
            return And(field, "<>", value, tableName);
        }

        public ConditionBuilder AndGreaterThan(string field, object value, string tableName = "")
        {
            return And(field, ">", value, tableName);
        }

        public ConditionBuilder AndGreaterThanEqual(string field, object value, string tableName = "")
        {
            return And(field, ">=", value, tableName);
        }

        public ConditionBuilder AndLessThan(string field, object value, string tableName = "")
        {
            return And(field, "<", value, tableName);
        }

        public ConditionBuilder AndLessThanEqual(string field, object value, string tableName = "")
        {
            return And(field, "<=", value, tableName);
        }

        public ConditionBuilder AndContains(string field, string value, string tableName = "")
        {
            return And(field, " LIKE ", $"%{value}%", tableName);
        }

        public ConditionBuilder AndStartsWith(string field, string value, string tableName = "")
        {
            return And(field, " LIKE ", $"{value}%", tableName);
        }

        public ConditionBuilder AndEndsWith(string field, string value, string tableName = "")
        {
            return And(field, " LIKE ", $"%{value}", tableName);
        }

        public ConditionBuilder AndIn(string field, IEnumerable<int> values, string tableName = "")
        {
            return And(field, " IN ", values, tableName);
        }

        public ConditionBuilder AndIn(string field, IEnumerable<decimal> values, string tableName = "")
        {
            return And(field, " IN ", values, tableName);
        }

        public ConditionBuilder AndIn(string field, IEnumerable<DateTime> values, string tableName = "")
        {
            return And(field, " IN ", values, tableName);
        }

        public ConditionBuilder AndIn(string field, IEnumerable<long> values, string tableName = "")
        {
            return And(field, " IN ", values, tableName);
        }

        public ConditionBuilder AndIn(string field, IEnumerable<Guid> values, string tableName = "")
        {
            return And(field, " IN ", values, tableName);
        }

        public ConditionBuilder AndIn(string field, IEnumerable<string> values, string tableName = "")
        {
            return And(field, " IN ", values, tableName);
        }

        public ConditionBuilder AndNotNull(string field, string tableName = "")
        {
            return And($"{BuildFullFieldName(field, tableName)} IS NOT NULL");
        }

        public ConditionBuilder AndNotNullOrEmpty(string field, string tableName = "")
        {
            var fieldName = BuildFullFieldName(field, tableName);
            return And($"({fieldName} IS NOT NULL OR {fieldName}<>'')");
        }

        public ConditionBuilder AndIsNull(string field, string tableName = "")
        {
            return And($"{BuildFullFieldName(field, tableName)} IS NULL");
        }

        public ConditionBuilder AndIsNullOrEmpty(string field, string tableName = "")
        {
            var fieldName = BuildFullFieldName(field, tableName);
            return And($"({fieldName} IS NULL OR {fieldName}<>'')");
        }

        public ConditionBuilder AndCondition(string condition)
        {
            return And(condition);
        }

        public ConditionBuilder AndContains(IEnumerable<string> fields, string value, string tableName = "")
        {
            var enumerable = fields as string[] ?? fields.ToArray();
            if (!enumerable.Any()) return this;

            var parameterName = BuildParameterName();
            _parameters.Add(parameterName, value);

            var clauses = enumerable.Select(field => $"{BuildFullFieldName(field, tableName)} LIKE {_parameterPrefix}{parameterName}").ToList();

            return And($"({string.Join(" OR ", clauses)})");
        }

        #endregion

        #region AndOr
        public ConditionBuilder AndOr(ConditionClausList mcc)
        {
            if (mcc.Count == 0)
            {
                return this;
            }

            var conditions = ResolveConditionClauses(mcc);
            return And($"({string.Join(" OR ", conditions)})");
        }
        #endregion

        #region 解析条件子句
        private string ResolveConditionClaus(ConditionClaus claus)
        {
            var op = ResolveOp(claus.Op);
            var paramName = BuildParameterName();

            var condition = "";
            if (!string.IsNullOrWhiteSpace(op))
            {
                condition = $"{BuildFullFieldName(claus.Column, claus.Table)}{op}{_parameterPrefix}{paramName}";
                _parameters.Add(paramName, claus.Value);
            }
            else switch (claus.Op)
            {
                case SqlOperator.Contains:
                    condition = BuildFullFieldName(claus.Column, claus.Table) + " LIKE " + _parameterPrefix + paramName;
                    _parameters.Add(paramName, "%" + claus.Value + "%");
                    break;
                case SqlOperator.StartsWith:
                    condition = BuildFullFieldName(claus.Column, claus.Table) + " LIKE " + _parameterPrefix + paramName;
                    _parameters.Add(paramName, claus.Value + "%");
                    break;
                case SqlOperator.EndsWith:
                    condition = BuildFullFieldName(claus.Column, claus.Table) + " LIKE " + _parameterPrefix + paramName;
                    _parameters.Add(paramName, "%" + claus.Value);
                    break;
            }
            return condition;
        }

        private IEnumerable<string> ResolveConditionClauses(ConditionClausList mcc)
        {
            var clauses = new List<string>();
            foreach (var m in mcc)
            {
                clauses.Add(ResolveConditionClaus(m));
            }
            return clauses;
        }

        private static string ResolveOp(SqlOperator op)
        {
            switch (op)
            {
                case SqlOperator.Equal:
                    return "=";
                case SqlOperator.NotEqual:
                    return "<>";
                case SqlOperator.GreaterThan:
                    return ">";
                case SqlOperator.GreaterThanEqual:
                    return ">=";
                case SqlOperator.LessThan:
                    return "<";
                case SqlOperator.LessThanEqual:
                    return "<=";
                case SqlOperator.In:
                    return "IN";
                default:
                    return "";
            }
        }
        #endregion
    }

    public class ConditionClausList : List<ConditionClaus>
    {
        public static ConditionClausList New()
        {
            return new ConditionClausList();
        }

        public ConditionClausList Add(string column, SqlOperator op, object value, string table = "")
        {
            Add(new ConditionClaus(column, op, value, table));
            return this;
        }
    }

    public class ConditionClaus
    {
        public string Column { get; set; }

        public object Value { get; set; }

        public SqlOperator Op;

        public string Table { get; set; }

        public ConditionClaus(string column, SqlOperator op, object val, string table = "")
        {
            Column = column;
            Op = op;
            Value = val;
            Table = table;
        }
    }

    public enum SqlOperator
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanEqual,
        LessThan,
        LessThanEqual,
        Contains,
        StartsWith,
        EndsWith,
        In
    }
}
