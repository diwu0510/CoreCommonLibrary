using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HZC.DbUtil
{
    public class MySearchUtil : ISearchUtil
    {
        #region �����ֶ�
        private readonly string _sqlParameterPrefix = "@";                      // SqlParameter��ǰ׺
        private readonly string _sqlParameterNamePrefix = "__p";
        private int _idx;                                                       // ��������
        #endregion

        #region �����Ӿ�Ͳ�ѯ�����ֶ�
        private string _conditonClauses = "";                                       // �����Ӿ�
        private DynamicParameters _params = new DynamicParameters();                // Dapper�Ĳ�ѯ����
        private List<string> _cols = new List<string>();
        private List<string> _orderby = new List<string>();                         // �����ֶ�
        #endregion

        #region ���캯��
        public MySearchUtil()
        { }

        public MySearchUtil(string parameterName)
        {
            _sqlParameterNamePrefix = parameterName;
        }
        #endregion

        #region ����-��ѯ���ͽ��
        /// <summary>
        /// �����Ӿ�
        /// </summary>
        public string ToWhere()
        {
            if (!string.IsNullOrWhiteSpace(_conditonClauses))
            {
                return _conditonClauses;
            }
            else
            {
                return "1=1";
            }
        }

        /// <summary>
        /// ��ѯ����
        /// </summary>
        public DynamicParameters ToDynamicParameters(bool isPaging = false)
        {
            if (isPaging)
            {
                _params.Add("RecordCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
                return _params;
            }
            else
            {
                return _params;
            }
        }

        /// <summary>
        /// �����Ӿ�
        /// </summary>
        public string ToOrderBy()
        {
            if (_orderby.Count == 0)
            {
                return "Id";
            }
            else
            {
                return string.Join(",", _orderby);
            }
        }

        /// <summary>
        /// ��ѯ���е��Ӿ�
        /// </summary>
        public string Columns
        {
            get
            {
                if (_cols.Count == 0)
                {
                    return "*";
                }
                return string.Join(",", _cols);
            }
        }
        #endregion

        #region ����һ��ʵ��
        public static MySearchUtil New()
        {
            return new MySearchUtil();
        }

        public static MySearchUtil NewExists()
        {
            return new MySearchUtil("__o");
        }
        #endregion

        #region �����Ӿ�
        /// <summary>
        /// And�Ӿ�
        /// </summary>
        /// <param name="conditionString">����</param>
        /// <returns></returns>
        public MySearchUtil And(string conditionString)
        {
            if (string.IsNullOrWhiteSpace(conditionString))
            {
                return this;
            }

            if (!string.IsNullOrWhiteSpace(_conditonClauses))
            {
                _conditonClauses += " AND ";
            }
            _conditonClauses += conditionString;
            return this;
        }

        /// <summary>
        /// Or�Ӿ�
        /// </summary>
        /// <param name="conditionString"></param>
        /// <returns></returns>
        private MySearchUtil Or(string conditionString)
        {
            if (string.IsNullOrWhiteSpace(conditionString))
            {
                return this;
            }

            if (!string.IsNullOrWhiteSpace(_conditonClauses))
            {
                _conditonClauses = "(" + _conditonClauses + ") OR ";
            }
            _conditonClauses += conditionString;
            return this;
        }

        /// <summary>
        /// AndOr�Ӿ�
        /// </summary>
        /// <param name="mcc"></param>
        /// <returns></returns>
        public MySearchUtil AndOr(ConditionClausList mcc)
        {
            if (mcc.Count == 0)
            {
                return this;
            }

            var strs = ResolveConditionClauses(mcc);
            if (!string.IsNullOrWhiteSpace(_conditonClauses))
            {
                _conditonClauses += " AND ";
            }
            _conditonClauses += "(" + string.Join(" OR ", strs) + ")";
            return this;
        }
        #endregion

        #region AND

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="column">����</param>
        /// <param name="value">ֵ</param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public MySearchUtil AndEqual(string column, object value, string tableName = "")
        {
            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            _params.Add(paramName, value);
            return And(string.Format(GetColumnName(column, tableName) + "=" + _sqlParameterPrefix + paramName));
        }

        /// <summary>
        /// ������
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public MySearchUtil AndNotEqual(string column, object value, string tableName = "")
        {
            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            _params.Add(paramName, value);
            return And(string.Format(GetColumnName(column, tableName) + "<>" + _sqlParameterPrefix + paramName));
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public MySearchUtil AndGreaterThan(string column, object value, string tableName = "")
        {
            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            _params.Add(paramName, value);
            return And(string.Format(GetColumnName(column, tableName) + ">" + _sqlParameterPrefix + paramName));
        }

        /// <summary>
        /// ���ڵ���
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public MySearchUtil AndGreaterThanEqual(string column, object value, string tableName = "")
        {
            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            _params.Add(paramName, value);
            return And(string.Format(GetColumnName(column, tableName) + ">=" + _sqlParameterPrefix + paramName));
        }

        /// <summary>
        /// С��
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public MySearchUtil AndLessThan(string column, object value, string tableName = "")
        {
            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            _params.Add(paramName, value);
            return And(string.Format(GetColumnName(column, tableName) + "<" + _sqlParameterPrefix + paramName));
        }

        /// <summary>
        /// С�ڵ���
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public MySearchUtil AndLessThanEqual(string column, object value, string tableName = "")
        {
            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            _params.Add(paramName, value);
            return And(string.Format(GetColumnName(column, tableName) + "<=" + _sqlParameterPrefix + paramName));
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public MySearchUtil AndContains(string column, string value, string tableName = "")
        {
            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            _params.Add(paramName, "%" + value + "%");
            return And(string.Format(GetColumnName(column, tableName) + " LIKE " + _sqlParameterPrefix + paramName));
        }

        public MySearchUtil AndContains(string[] columns, string value, string tableName = "")
        {
            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            _params.Add(paramName, "%" + value + "%");

            if (columns.Any())
            {
                if (!string.IsNullOrWhiteSpace(_conditonClauses))
                {
                    _conditonClauses += " AND ";
                }
                _conditonClauses += "(";
                List<string> claus = new List<string>();
                foreach (var column in columns)
                {
                    claus.Add(GetColumnName(column, tableName) + " LIKE " + _sqlParameterPrefix + paramName);
                }

                _conditonClauses += string.Join(" Or ", claus) + ")";
            }

            return this;
        }

        /// <summary>
        /// �����
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public MySearchUtil AndStartWith(string column, string value, string tableName = "")
        {
            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            _params.Add(paramName, value + "%");
            return And(string.Format(GetColumnName(column, tableName) + " LIKE " + _sqlParameterPrefix + paramName));
        }

        /// <summary>
        /// �Ұ���
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public MySearchUtil AndEndWith(string column, string value, string tableName = "")
        {
            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            _params.Add(paramName, "%" + value);
            return And(string.Format(GetColumnName(column, tableName) + " LIKE " + _sqlParameterPrefix + paramName));
        }

        /// <summary>
        /// In�Ӿ�-����
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public MySearchUtil AndIn(string column, int[] value, string tableName = "")
        {
            if (!value.Any())
            {
                return this;
            }

            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            _params.Add(paramName, value);
            return And(string.Format(GetColumnName(column, tableName) + " IN " + _sqlParameterPrefix + paramName));
        }

        /// <summary>
        /// In�Ӿ�-����
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public MySearchUtil AndIn(string column, float[] value, string tableName = "")
        {
            if (!value.Any())
            {
                return this;
            }

            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            _params.Add(paramName, value);
            return And(string.Format(GetColumnName(column, tableName) + " IN " + _sqlParameterPrefix + paramName));
        }

        /// <summary>
        /// In�Ӿ�-����
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public MySearchUtil AndIn(string column, decimal[] value, string tableName = "")
        {
            if (!value.Any())
            {
                return this;
            }

            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            _params.Add(paramName, value);
            return And(string.Format(GetColumnName(column, tableName) + " IN " + _sqlParameterPrefix + paramName));
        }

        /// <summary>
        /// In�Ӿ�-�ı�
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public MySearchUtil AndIn(string column, string[] value, string tableName = "")
        {
            if (!value.Any())
            {
                return this;
            }

            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            _params.Add(paramName, value);
            return And(string.Format(GetColumnName(column, tableName) + " IN " + _sqlParameterPrefix + paramName));
        }

        /// <summary>
        /// In�Ӿ�-����
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public MySearchUtil AndIn(string column, DateTime[] value, string tableName = "")
        {
            if (!value.Any())
            {
                return this;
            }

            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            _params.Add(paramName, value);
            return And(string.Format(GetColumnName(column, tableName) + " IN " + _sqlParameterPrefix + paramName));
        }

        /// <summary>
        /// ��Ϊ��
        /// </summary>
        /// <param name="column"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public MySearchUtil AndNotNull(string column, string tableName = "")
        {
            return And(GetColumnName(column, tableName) + " IS NOT NULL");
        }

        /// <summary>
        /// ��Ϊ���Ҳ�Ϊ���ַ���
        /// </summary>
        /// <param name="column"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public MySearchUtil AndNotNullOrEmpty(string column, string tableName = "")
        {
            return And(GetColumnName(column, tableName) + " IS NOT NULL AND " + column + " <> ''");
        }

        /// <summary>
        /// Ϊ��
        /// </summary>
        /// <param name="column"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public MySearchUtil AndNull(string column, string tableName = "")
        {
            return And(GetColumnName(column, tableName) + " IS NULL");
        }

        /// <summary>
        /// Ϊ�ջ���ַ���
        /// </summary>
        /// <param name="column"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public MySearchUtil AndNullOrEmpty(string column, string tableName = "")
        {
            return And(GetColumnName(column, tableName) + " IS NULL AND " + GetColumnName(column, tableName) + " = ''");
        }

        /// <summary>
        /// û�в�����And�Ӿ�
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public MySearchUtil AndNoParam(string condition)
        {
            return And(condition);
        }
        #endregion

        #region OR
        public MySearchUtil OrEqual(string column, object value, string tableName = "")
        {
            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            _params.Add(paramName, value);
            return Or(string.Format(GetColumnName(column, tableName) + "=" + _sqlParameterPrefix + paramName));
        }

        public MySearchUtil OrGreaterThan(string column, object value, string tableName = "")
        {
            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            _params.Add(paramName, value);
            return Or(string.Format(GetColumnName(column, tableName) + ">" + _sqlParameterPrefix + paramName));
        }

        public MySearchUtil OrGreaterThanEqual(string column, object value, string tableName = "")
        {
            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            _params.Add(paramName, value);
            return Or(string.Format(GetColumnName(column, tableName) + ">=" + _sqlParameterPrefix + paramName));
        }

        public MySearchUtil OrLessThan(string column, object value, string tableName = "")
        {
            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            _params.Add(paramName, value);
            return Or(string.Format(GetColumnName(column, tableName) + "<" + _sqlParameterPrefix + paramName));
        }

        public MySearchUtil OrLessThanEqual(string column, object value, string tableName = "")
        {
            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            _params.Add(paramName, value);
            return Or(string.Format(GetColumnName(column, tableName) + "<=" + _sqlParameterPrefix + paramName));
        }

        public MySearchUtil OrContains(string column, string value, string tableName = "")
        {
            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            _params.Add(paramName, "%" + value + "%");
            return Or(string.Format(GetColumnName(column, tableName) + " LIKE " + _sqlParameterPrefix + paramName));
        }

        public MySearchUtil OrStartWith(string column, string value, string tableName = "")
        {
            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            _params.Add(paramName, value + "%");
            return Or(string.Format(GetColumnName(column, tableName) + " LIKE " + _sqlParameterPrefix + paramName));
        }

        public MySearchUtil OrEndWith(string column, string value, string tableName = "")
        {
            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            _params.Add(paramName, "%" + value);
            return Or(string.Format(GetColumnName(column, tableName) + " LIKE " + _sqlParameterPrefix + paramName));
        }

        public MySearchUtil OrIn(string column, object[] value, string tableName = "")
        {
            if (value.Count() == 0)
            {
                return this;
            }

            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            _params.Add(paramName, value);
            return Or(string.Format(GetColumnName(column, tableName) + " IN " + _sqlParameterPrefix + paramName));
        }

        public MySearchUtil OrNoParam(string condition)
        {
            return Or(condition);
        }
        #endregion

        #region ���������Ӿ�
        private string ResolveConditonClaus(ConditionClaus claus)
        {
            string op = ResolveOp(claus.Op);
            var paramName = _sqlParameterNamePrefix + _idx++.ToString();
            string condition = "";
            if (!string.IsNullOrWhiteSpace(op))
            {
                condition = string.Format("{0} {1} {2}", claus.Column, op, _sqlParameterPrefix + paramName);
                _params.Add(paramName, claus.Value);
            }
            else if (claus.Op == SqlOperator.Contains)
            {
                condition = claus.Column + " LIKE " + _sqlParameterPrefix + paramName;
                _params.Add(paramName, "%" + claus.Value + "%");
            }
            else if (claus.Op == SqlOperator.StartWith)
            {
                condition = claus.Column + " LIKE " + _sqlParameterPrefix + paramName;
                _params.Add(paramName, claus.Value + "%");
            }
            else if (claus.Op == SqlOperator.EndWith)
            {
                condition = claus.Column + " LIKE " + _sqlParameterPrefix + paramName;
                _params.Add(paramName, "%" + claus.Value);
            }
            return condition;
        }

        private List<string> ResolveConditionClauses(ConditionClausList mcc)
        {
            List<string> clauses = new List<string>();
            foreach (var m in mcc)
            {
                clauses.Add(ResolveConditonClaus(m));
            }
            return clauses;
        }

        private string ResolveOp(SqlOperator op)
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

        #region ����
        public MySearchUtil OrderBy(string column, string tableName = "")
        {
            _orderby.Add(GetColumnName(column, tableName));
            return this;
        }

        public MySearchUtil OrderByDesc(string column, string tableName = "")
        {
            _orderby.Add(GetColumnName(column, tableName) + " Desc");
            return this;
        }

        public MySearchUtil OrderBy(SortClaus sortClaus)
        {
            _orderby.Add(sortClaus.Column);
            return this;
        }

        public MySearchUtil OrderBy(SortClausList sortClauses)
        {
            foreach (var claus in sortClauses)
            {
                var ob = claus.Column;
                if (claus.Direction == SqlOrderByDirection.Desc)
                {
                    ob += " Desc";
                }
                _orderby.Add(ob);
            }
            return this;
        }
        #endregion

        #region ��ѯ����
        public MySearchUtil Select(string cols)
        {
            if (!string.IsNullOrWhiteSpace(cols))
            {
                _cols.Add(cols);
            }
            return this;
        }

        public MySearchUtil Select(string table, string cols)
        {
            SelectClaus sc = new SelectClaus(table, cols);
            return Select(sc);
        }

        public MySearchUtil Select(SelectClaus select)
        {
            _cols.Add(select.Column);
            return this;
        }

        public MySearchUtil Select(SelectClausList selectClausList)
        {
            foreach (var sc in selectClausList)
            {
                _cols.Add(sc.Column);
            }
            return this;
        }
        #endregion

        private string GetColumnName(string column, string tableName = "")
        {
            return string.IsNullOrWhiteSpace(tableName) ? column :  tableName + "." + column;
        }
    }

    public class ConditionClausList : List<ConditionClaus>
    {
        public static ConditionClausList New()
        {
            return new ConditionClausList();
        }

        public ConditionClausList Add(string column, SqlOperator op, object value)
        {
            Add(new ConditionClaus(column, op, value));
            return this;
        }
    }

    public class ConditionClaus
    {
        public string Column { get; set; }

        public object Value { get; set; }

        public SqlOperator Op;

        public ConditionClaus(string column, SqlOperator op, object val)
        {
            Column = column;
            Op = op;
            Value = val;
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
        StartWith,
        EndWith,
        In
    }

    public class SortClaus
    {
        public string Column { get; set; }

        public SqlOrderByDirection Direction { get; set; }

        public SortClaus(string column, SqlOrderByDirection direction)
        {
            Column = column;
            Direction = direction;
        }

        public SortClaus(string column) : this(column, SqlOrderByDirection.Asc)
        { }
    }

    public class SortClausList : List<SortClaus>
    {
        public static SortClausList New()
        {
            return new SortClausList();
        }

        public SortClausList Add(string column, SqlOrderByDirection direction)
        {
            Add(new SortClaus(column, direction));
            return this;
        }

        public SortClausList Add(string column)
        {
            return Add(column, SqlOrderByDirection.Asc);
        }
    }

    public class SelectClaus
    {
        public string Table { get; set; }

        public string Column { get; set; }

        public SelectClaus(string table, string column)
        {
            Table = table;
            var columns = column.Split(',');
            if (columns.Length == 1)
            {
                if (string.IsNullOrWhiteSpace(table))
                {
                    Column = column;
                }
                else
                {
                    Column = "[" + table + "]." + column;
                }
            }
            else
            {
                foreach (var s in columns)
                {
                    if (string.IsNullOrWhiteSpace(table))
                    {
                        Column += ",";
                    }
                    else
                    {
                        Column += ",[" + table + "]." + s;
                    }
                }

                if (!string.IsNullOrWhiteSpace(Column))
                {
                    Column = Column.Remove(0, 1);
                }
            }
        }

        public SelectClaus(string table, string[] columns)
        {
            Table = table;
            foreach (var s in columns)
            {
                Column += "[" + table + "]." + s;
            }
        }

        public SelectClaus(string table, List<string> columns) : this(table, columns.ToArray())
        { }

        public SelectClaus(string column) : this("", column)
        { }

        public SelectClaus(string[] columns) : this("", columns)
        { }

        public SelectClaus(List<string> columns) : this("", columns)
        { }
    }

    public class SelectClausList : List<SelectClaus>
    {
        public static SelectClausList New()
        {
            return new SelectClausList();
        }

        public SelectClausList Add(string table, string columns)
        {
            Add(new SelectClaus(table, columns));
            return this;
        }

        public SelectClausList Add(string columns)
        {
            Add(new SelectClaus(columns));
            return this;
        }

        public SelectClausList Add(string[] columns)
        {
            Add(new SelectClaus(columns));
            return this;
        }

        public SelectClausList Add(List<string> columns)
        {
            Add(new SelectClaus(columns));
            return this;
        }

        public SelectClausList Add(string table, string[] columns)
        {
            Add(new SelectClaus(table, columns));
            return this;
        }

        public SelectClausList Add(string table, List<string> columns)
        {
            Add(new SelectClaus(table, columns));
            return this;
        }
    }

    public enum SqlOrderByDirection
    {
        Asc,
        Desc
    }
}
