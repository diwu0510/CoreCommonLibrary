﻿using HZC.Framework;
using HZC.MyDapper.Conditions;
using HZC.MyDapper.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HZC.MyDapper.SqlBuilders
{
    public class SqlServerBuilder
    {
        #region 插入
        /// <summary>
        /// 获取插入的SQL语句
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string GetInsertSql(MyEntity entity)
        {
            var columnList = new List<string>();
            var paramList = new List<string>();

            foreach (var property in entity.Properties.Where(p => !p.InsertIgnore))
            {
                columnList.Add($"[{property.DataTableColumn}]");
                paramList.Add($"@{property.PropertyName}");
            }

            var sb = new StringBuilder($"INSERT INTO [{entity.DataTableName}] (");
            sb.Append(string.Join(",", columnList));
            sb.Append(") VALUES (");
            sb.Append(string.Join(",", paramList));
            sb.Append(")");

            return sb.ToString();
        }

        /// <summary>
        /// 获取如果不满足条件则插入的SQL语句
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static string GetInsertIfNotExistsSql(MyEntity entity, ConditionBuilder builder)
        {
            var sql = GetInsertSql(entity);
            return $"IF NOT EXISTS (SELECT Id FROM [{entity.DataTableName}] WHERE {builder.ToCondition()}) {sql}";
        }

        /// <summary>
        /// 获取插入的SQL语句，返回SCOPE_IDENTITTY
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string GetInsertSqlWithIdentity(MyEntity entity)
        {
            var sql = GetInsertSql(entity);
            return $"{sql};SELECT SCOPE_IDENTITY()";
        }

        /// <summary>
        /// 获取如果不满足条件则插入的SQL语句，返回SCOPE_IDENTITY
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static string GetInsertIfNotExistsSqlWithIdentity(MyEntity entity, ConditionBuilder builder)
        {
            var sql = GetInsertSql(entity);
            return $"IF NOT EXISTS (SELECT Id FROM [{entity.DataTableName}] WHERE {builder.ToCondition()}) {sql};SELECT SCOPE_IDENTITY()";
        }
        #endregion

        #region 修改
        public static string GetUpdateSql(MyEntity entity)
        {
            var sb = new StringBuilder($"UPDATE [{entity.DataTableName}] SET ");
            sb.Append(string.Join(",", entity.Properties.Where(p => !p.UpdateIgnore)
                .Select(p => $"[{p.DataTableColumn}]=@{p.PropertyName}")));
            return sb.ToString();
        }

        public static string GetUpdateSql(MyEntity entity, ConditionBuilder builder)
        {
            var where = builder.ToCondition();
            var sb = new StringBuilder($"UPDATE [{entity.DataTableName}] SET ");
            sb.Append(string.Join(",", entity.Properties.Where(p => !p.UpdateIgnore)
                .Select(p => $"[{p.DataTableColumn}]=@{p.PropertyName}")));
            sb.Append($" WHERE {where}");
            return sb.ToString();
        }

        public static string GetUpdateIncludeSql(MyEntity entity, IEnumerable<string> includePropertyStrings, ConditionBuilder builder = null)
        {
            var where = builder?.ToCondition() ?? "Id=@Id";
            var sb = new StringBuilder($"UPDATE [{entity.DataTableName}] SET ");
            sb.Append(string.Join(",", entity.Properties.Where(p => !p.UpdateIgnore && includePropertyStrings.Contains(p.PropertyName))
                .Select(p => $"[{p.DataTableColumn}]=@{p.PropertyName}")));
            sb.Append($" WHERE {where}");
            return sb.ToString();
        }

        public static string GetUpdateExcludeSql(MyEntity entity, IEnumerable<string> excludePropertyStrings, ConditionBuilder builder = null)
        {
            var where = builder?.ToCondition() ?? "Id=@Id";
            var sb = new StringBuilder($"UPDATE [{entity.DataTableName}] SET ");
            sb.Append(string.Join(",", entity.Properties.Where(p => !p.UpdateIgnore && !excludePropertyStrings.Contains(p.PropertyName))
                .Select(p => $"[{p.DataTableColumn}]=@{p.PropertyName}")));
            sb.Append($" WHERE {where}");
            return sb.ToString();
        }

        public static string GetUpdateSetSql(string table, SoKeyValuePairs kvs, ConditionBuilder search = null)
        {
            var where = search?.ToCondition() ?? "1=1";
            var sql = $"UPDATE [{table}] SET {kvs.ToUpdateSetSql()} WHERE {where}";
            return sql;
        }

        public static string GetUpdateSetSql(MyEntity entity, SoKeyValuePairs kvs)
        {
            var sql = $"UPDATE [{entity.DataTableName}] SET {kvs.ToUpdateSetSql()} WHERE Id=@Id";
            return sql;
        }
        #endregion

        #region 加载数据
        public static string GetLoadByIdSql(MyEntity entity, int id)
        {
            var sql = $"SELECT {GetFullQueryColumns(entity)} FROM [{entity.DataTableName}] WHERE Id=@Id";
            if (entity.IsSoftDelete)
            {
                sql += " AND IsDel=0";
            }
            return sql;
        }

        public static string GetLoadBySearchUtilSql(MyEntity entity, ConditionBuilder builder)
        {
            var sql = $"SELECT {GetFullQueryColumns(entity)} FROM [{entity.DataTableName}] WHERE {builder.ToCondition()}";
            if (entity.IsSoftDelete)
            {
                sql += " AND IsDel=0";
            }
            return sql;
        }
        #endregion

        #region 查询数据
        public static string GetQuerySql(MyEntity entity)
        {
            var sql = $"SELECT {GetFullQueryColumns(entity)} FROM [{entity.DataTableName}]";
            if (entity.IsSoftDelete)
            {
                sql += " AND IsDel=0";
            }
            return sql;
        }

        public static string GetQuerySql(MyEntity entity, ConditionBuilder util, SortBuilder sort)
        {
            var sql = $"SELECT {GetFullQueryColumns(entity)} FROM [{entity.DataTableName}] WHERE {util.ToCondition()} ORDER BY {sort.ToOrderBy()}";
            if (entity.IsSoftDelete)
            {
                sql += " AND IsDel=0";
            }
            return sql;
        }
        #endregion

        #region 分页查询
        public static string GetPageQuerySql(MyEntity entity, int pageIndex, int pageSize)
        {
            var table = entity.DataTableName;
            var fields = GetFullQueryColumns(entity);
            var where = entity.IsSoftDelete ? "IsDel=0" : "1=1";
            var orderBy = "Id DESC";

            return GetPageQuerySql(table, fields, where, orderBy, pageIndex, pageSize);
        }

        public static string GetPageQueryWithTotalCountSql(MyEntity entity, int pageIndex, int pageSize)
        {
            var table = entity.DataTableName;
            var fields = GetFullQueryColumns(entity);
            var where = entity.IsSoftDelete ? "IsDel=0" : "1=1";
            var orderBy = "Id DESC";

            return GetPageQueryWithTotalCountSql(table, fields, where, orderBy, pageIndex, pageSize);
        }

        public static string GetPageQuerySql(MyEntity entity, ConditionBuilder search, SortBuilder sort, int pageIndex, int pageSize)
        {
            var table = entity.DataTableName;
            var fields = GetFullQueryColumns(entity);
            var where = search.ToCondition();
            if (entity.IsSoftDelete)
            {
                where += " AND IsDel=0";
            }
            var orderBy = sort.ToOrderBy();
            return GetPageQuerySql(table, fields, where, orderBy, pageIndex, pageSize);
        }

        public static string GetPageQueryWithTotalCountSql(MyEntity entity, ConditionBuilder search, SortBuilder sort, int pageIndex, int pageSize)
        {
            var table = entity.DataTableName;
            var fields = GetFullQueryColumns(entity);
            var where = search.ToCondition();
            if (entity.IsSoftDelete)
            {
                where += " AND IsDel=0";
            }
            var orderBy = sort.ToOrderBy();
            return GetPageQueryWithTotalCountSql(table, fields, where, orderBy, pageIndex, pageSize);
        }
        #endregion

        public static string GetFullQueryColumns(MyEntity entity)
        {
            return string.Join(",", entity.Properties.Where(p => p.IsMap).Select(p => $"[{p.DataTableColumn}] AS {p.PropertyName}"));
        }

        /// <summary>
        /// 获取分页查询的SQL语句，并返回数据总数
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="fields">列名</param>
        /// <param name="where">条件</param>
        /// <param name="orderBy">排序</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        public static string GetPageQueryWithTotalCountSql(string table, string fields, string where, string orderBy, int pageIndex, int pageSize)
        {
            where = string.IsNullOrWhiteSpace(where) ? "1=1" : where;
            orderBy = string.IsNullOrWhiteSpace(orderBy) ? "Id DESC" : orderBy;
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            if (pageIndex == 1)
            {
                var sql =
                    $"SELECT TOP {pageSize} {fields} FROM {table} WHERE {where} ORDER BY {orderBy};SELECT @RecordCount=COUNT(0) FROM {table} WHERE {where}";

                return sql;
            }
            else
            {
                var sb = new StringBuilder();
                sb.Append("FROM ").Append(table);

                if (!string.IsNullOrWhiteSpace(where))
                {
                    sb.Append(" WHERE ").Append(where);
                }

                var sql = $@"  WITH PAGEDDATA AS
					    (
						    SELECT TOP 100 PERCENT {fields}, ROW_NUMBER() OVER (ORDER BY {orderBy}) AS FLUENTDATA_ROWNUMBER
						    {sb}
					    )
					    SELECT {fields}
					    FROM PAGEDDATA
					    WHERE FLUENTDATA_ROWNUMBER BETWEEN {(pageIndex - 1) * pageSize + 1} AND {pageIndex * pageSize};
                        SELECT @RecordCount=COUNT(0) FROM {table} WHERE {where}";
                return sql;
            }
        }

        /// <summary>
        /// 获取分页查询的SQL语句，不包含总数
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="fields">列名</param>
        /// <param name="where">条件</param>
        /// <param name="orderBy">排序</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        public static string GetPageQuerySql(string table, string fields, string where, string orderBy, int pageIndex, int pageSize)
        {
            where = string.IsNullOrWhiteSpace(where) ? "1=1" : where;
            orderBy = string.IsNullOrWhiteSpace(orderBy) ? "Id DESC" : orderBy;
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            if (pageIndex == 1)
            {
                var sql =
                    $"SELECT TOP {pageSize} {fields} FROM {table} WHERE {where} ORDER BY {orderBy}";

                return sql;
            }
            else
            {
                var sb = new StringBuilder();
                sb.Append("FROM ").Append(table);

                if (!string.IsNullOrWhiteSpace(where))
                {
                    sb.Append(" WHERE ").Append(where);
                }

                var sql = $@"  WITH PAGEDDATA AS
					    (
						    SELECT TOP 100 PERCENT {fields}, ROW_NUMBER() OVER (ORDER BY {orderBy}) AS FLUENTDATA_ROWNUMBER
						    {sb}
					    )
					    SELECT {fields}
					    FROM PAGEDDATA
					    WHERE FLUENTDATA_ROWNUMBER BETWEEN {(pageIndex - 1) * pageSize + 1} AND {pageIndex * pageSize}";
                return sql;
            }
        }
    }
}
