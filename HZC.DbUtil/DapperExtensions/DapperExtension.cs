using Dapper;
using HZC.Core;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HZC.DbUtil
{
    public static class DapperExtension
    {
        #region 创建
        /// <summary>
        /// 新建实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static Task<int> InsertAsync<T>(this SqlConnection conn, T entity, SqlTransaction trans = null) 
            where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            var sql = info.GetInsertSqlWithIdentity();
            return conn.ExecuteScalarAsync<int>(sql, entity, trans);
        }

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static T Insert<T>(this SqlConnection conn, T entity, SqlTransaction trans = null) 
            where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            var sql = info.GetInsertSqlWithIdentity();
            entity.Id = conn.ExecuteScalar<int>(sql, entity, trans);
            return entity;
        }

        /// <summary>
        /// 如果不存在，新建一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="existsSearch">是否存在的查询</param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static Task<int> InsertIfNotExistsAsync<T>(this SqlConnection conn, 
            T entity, ISearchUtil existsSearch, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            var sql = info.GetInsertIfNotExistsWithIdentitySql(existsSearch);
            var param = existsSearch.ToDynamicParameters();
            param.AddDynamicParams(entity);
            return conn.ExecuteScalarAsync<int>(sql, param, trans);
        }

        /// <summary>
        /// 如果不存在，新建一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="existsSearch"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static T InsertIfNotExists<T>(this SqlConnection conn, 
            T entity, ISearchUtil existsSearch, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            var sql = info.GetInsertIfNotExistsWithIdentitySql(existsSearch);
            var param = existsSearch.ToDynamicParameters();
            param.AddDynamicParams(entity);
            entity.Id = conn.ExecuteScalar<int>(sql, param, trans);
            return entity;
        }

        /// <summary>
        /// 批量创建实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entityList"></param>
        /// <param name="trans"></param>
        /// <returns>插入的数量</returns>
        public static Task<int> BatchInsertAsync<T>(this SqlConnection conn, 
            IEnumerable<T> entityList, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            var sql = info.GetInsertSql();
            return conn.ExecuteAsync(sql, entityList, trans);
        }

        /// <summary>
        /// 批量创建实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entityList"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int BatchInsert<T>(this SqlConnection conn, 
            List<T> entityList, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            var sql = info.GetInsertSql();
            return conn.Execute(sql, entityList, trans);
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static Task<int> UpdateAsync<T>(this SqlConnection conn, T entity, SqlTransaction trans = null) 
            where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));

            var sql = info.GetDefaultUpdateSql();
            return conn.ExecuteAsync(sql, entity, trans);
        }

        /// <summary>
        /// 修改实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int Update<T>(this SqlConnection conn, T entity, SqlTransaction trans = null) 
            where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));

            var sql = info.GetDefaultUpdateSql();
            return conn.Execute(sql, entity, trans);
        }

        /// <summary>
        /// 如果不存在，修改。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="existsUtil">必须使用 MySearchUtil.NewExists()获取</param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static Task<int> UpdateIfNotExistsAsync<T>(this SqlConnection conn, 
            T entity, ISearchUtil existsUtil, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            var sql = info.GetDefaultUpdateIfNotExistsSql(existsUtil);
            var param = existsUtil.ToDynamicParameters();
            param.AddDynamicParams(entity);
            return conn.ExecuteAsync(sql, param, trans);
        }

        /// <summary>
        /// 如果不存在，修改。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="existsUtil"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int UpdateIfNotExists<T>(this SqlConnection conn, 
            T entity, ISearchUtil existsUtil, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            var sql = info.GetDefaultUpdateIfNotExistsSql(existsUtil);
            var param = existsUtil.ToDynamicParameters();
            param.AddDynamicParams(entity);
            return conn.Execute(sql, param, trans);
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entityList"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static Task<int> BatchUpdateAsync<T>(this SqlConnection conn, 
            List<T> entityList, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));

            var sql = info.GetDefaultUpdateSql();
            return conn.ExecuteAsync(sql, entityList, trans);
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entityList"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int BatchUpdate<T>(this SqlConnection conn, 
            List<T> entityList, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));

            var sql = info.GetDefaultUpdateSql();
            return conn.Execute(sql, entityList, trans);
        }

        /// <summary>
        /// 批量修改指定属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="properties"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static Task<int> UpdateIncludeAsync<T>(this SqlConnection conn, 
            T entity, IEnumerable<string> properties, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));

            var sql = SqlServerSqlBuilder.GetUpdateIncludeSql(info, properties);
            return conn.ExecuteAsync(sql, entity, trans);
        }

        /// <summary>
        /// 批量修改指定属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="properties"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int UpdateInclude<T>(this SqlConnection conn,
            T entity, IEnumerable<string> properties, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));

            var sql = SqlServerSqlBuilder.GetUpdateIncludeSql(info, properties);
            return conn.Execute(sql, entity, trans);
        }

        /// <summary>
        /// 批量修改指定属性以外的属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="properties"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static Task<int> UpdateExcludeAsync<T>(this SqlConnection conn,
            T entity, IEnumerable<string> properties, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));

            var sql = SqlServerSqlBuilder.GetUpdateExcludeSql(info, properties);
            return conn.ExecuteAsync(sql, entity, trans);
        }

        /// <summary>
        /// 批量修改指定属性以外的属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="properties"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int UpdateExclude<T>(this SqlConnection conn,
            T entity, IEnumerable<string> properties, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));

            var sql = SqlServerSqlBuilder.GetUpdateExcludeSql(info, properties);
            return conn.Execute(sql, entity, trans);
        }
        #endregion

        #region 修改部分字段
        /// <summary>
        /// 修改指定表的指定字段
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="table"></param>
        /// <param name="kvs"></param>
        /// <param name="util"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static Task<int> SetAsync(this SqlConnection conn, 
            string table, SOKeyValuePaires kvs, ISearchUtil util, SqlTransaction trans = null)
        {
            var sql = SqlServerSqlBuilder.GetUpdateSetSql(table, kvs, util);
            var param = kvs.ToDynamicParameters();
            param.AddDynamicParams(util.ToDynamicParameters());
            return conn.ExecuteAsync(sql, param, trans);
        }

        /// <summary>
        /// 修改指定表的指定字段
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="table"></param>
        /// <param name="kvs"></param>
        /// <param name="util"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int Set(this SqlConnection conn, 
            string table, SOKeyValuePaires kvs, ISearchUtil util, SqlTransaction trans = null)
        {
            var sql = SqlServerSqlBuilder.GetUpdateSetSql(table, kvs, util);
            var param = kvs.ToDynamicParameters();
            param.AddDynamicParams(util.ToDynamicParameters());
            return conn.Execute(sql, param, trans);
        }

        /// <summary>
        /// 修改指定实体的指定属性为指定值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="kvs"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static Task<int> SetAsync<T>(this SqlConnection conn, 
            T entity, SOKeyValuePaires kvs, SqlTransaction trans = null) where T:BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            var sql = SqlServerSqlBuilder.GetUpdateSetSql(info, kvs);
            var param = kvs.ToDynamicParameters();
            param.Add("Id", entity.Id);
            return conn.ExecuteAsync(sql, param, trans);
        }

        /// <summary>
        /// 修改指定实体的指定属性为指定值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="kvs"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int Set<T>(this SqlConnection conn,
            T entity, SOKeyValuePaires kvs, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            var sql = SqlServerSqlBuilder.GetUpdateSetSql(info, kvs);
            var param = kvs.ToDynamicParameters();
            param.Add("Id", entity.Id);
            return conn.Execute(sql, param, trans);
        }

        /// <summary>
        /// 修改符合条件的所有实体的指定属性为指定的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="kvs"></param>
        /// <param name="search"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static Task<int> BatchSetAsync<T>(this SqlConnection conn,
            SOKeyValuePaires kvs, MySearchUtil search, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            var sql = SqlServerSqlBuilder.GetUpdateSetSql(info, kvs);
            var param = kvs.ToDynamicParameters();
            param.AddDynamicParams(search.ToDynamicParameters());
            return conn.ExecuteAsync(sql, param, trans);
        }

        /// <summary>
        /// 修改符合条件的所有实体的指定属性为指定的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn"></param>
        /// <param name="kvs"></param>
        /// <param name="search"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int BatchSet<T>(this SqlConnection conn,
            SOKeyValuePaires kvs, MySearchUtil search, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            var sql = SqlServerSqlBuilder.GetUpdateSetSql(info, kvs);
            var param = kvs.ToDynamicParameters();
            param.AddDynamicParams(search.ToDynamicParameters());
            return conn.Execute(sql, param, trans);
        }
        #endregion

        #region 删除
        public static Task<int> DeleteAsync<T>(this SqlConnection conn, int id, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            var sql = info.GetDefaultDeleteSql();
            return conn.ExecuteAsync(sql, new { Id = id }, trans);
        }

        public static int Delete<T>(this SqlConnection conn, int id, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            var sql = info.GetDefaultDeleteSql();
            return conn.Execute(sql, new { Id = id }, trans);
        }

        public static Task<int> DeleteAsync<T>(this SqlConnection conn, IEnumerable<int> ids, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            string sql;
            if(info.IsSoftDelete)
            {
                sql = $"UPDATE [{info.DataTableName}] SET IsDel=1 WHERE Id IN @Ids";
            }
            else
            {
                sql = $"DELETE [{info.DataTableName}] WHERE Id IN @Ids";
            }
            return conn.ExecuteAsync(sql, new { Ids = ids }, trans);
        }

        public static int Delete<T>(this SqlConnection conn, IEnumerable<int> ids, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            string sql;
            if (info.IsSoftDelete)
            {
                sql = $"UPDATE [{info.DataTableName}] SET IsDel=1 WHERE Id IN @Ids";
            }
            else
            {
                sql = $"DELETE [{info.DataTableName}] WHERE Id IN @Ids";
            }
            return conn.Execute(sql, new { Ids = ids }, trans);
        }

        public static Task<int> DeleteAsync(this SqlConnection conn, string table, int id)
        {
            var sql = $"DELETE [{table}] WHERE Id=@Id";
            return conn.ExecuteAsync(sql, new { Id = id });
        }

        public static int Delete(this SqlConnection conn, string table, int id)
        {
            var sql = $"DELETE [{table}] WHERE Id=@Id";
            return conn.Execute(sql, new { Id = id });
        }

        public static Task<int> DeleteAsync(this SqlConnection conn, string table, MySearchUtil search)
        {
            search = search ?? MySearchUtil.New();
            var sql = $"DELETE [{table}] WHERE {search.ToWhere()}";
            return conn.ExecuteAsync(sql, search.ToDynamicParameters());
        }

        public static int Delete(this SqlConnection conn, string table, MySearchUtil search)
        {
            search = search ?? MySearchUtil.New();
            var sql = $"DELETE [{table}] WHERE {search.ToWhere()}";
            return conn.Execute(sql, search.ToDynamicParameters());
        }

        public static Task<int> DeleteBySearchAsync<T>(this SqlConnection conn, ISearchUtil search) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            var sql = info.GetDeleteSql(search);
            return conn.ExecuteAsync(sql, search.ToDynamicParameters());
        }

        public static int DeleteBySearch<T>(this SqlConnection conn, ISearchUtil search) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            var sql = info.GetDeleteSql(search);
            return conn.Execute(sql, search.ToDynamicParameters());
        }

        public static Task<int> RemoveAsync(this SqlConnection conn, string table, MySearchUtil search)
        {
            search = search ?? MySearchUtil.New();
            var sql = $"UPDATE [{table}] SET IsDel=1 WHERE {search.ToWhere()}";
            return conn.ExecuteAsync(sql, search.ToDynamicParameters());
        }

        public static int Remove(this SqlConnection conn, string table, MySearchUtil search)
        {
            search = search ?? MySearchUtil.New();
            var sql = $"UPDATE [{table}] SET IsDel=1 WHERE {search.ToWhere()}";
            return conn.Execute(sql, search.ToDynamicParameters());
        }
        #endregion

        #region 加载实体
        public static Task<T> LoadAsync<T>(this SqlConnection conn, int id, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            var sql = $"SELECT {info.GetFullSelectColumns()} FROM [{info.DataTableName}] WHERE Id=@Id";
            if(info.IsSoftDelete)
            {
                sql += " AND IsDel=0";
            }
            return conn.QueryFirstOrDefaultAsync<T>(sql, new { Id = id }, trans);
        }

        public static T Load<T>(this SqlConnection conn, int id, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            var sql = $"SELECT {info.GetFullSelectColumns()} FROM [{info.DataTableName}] WHERE Id=@Id";
            if (info.IsSoftDelete)
            {
                sql += " AND IsDel=0";
            }
            return conn.QueryFirstOrDefault<T>(sql, new { Id = id }, trans);
        }

        public static Task<T> LoadBySearchUtilAsync<T>(
            this SqlConnection conn,
            MySearchUtil search,
            SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            search = search ?? MySearchUtil.New();
            if(info.IsSoftDelete)
            {
                search.AndEqual("IsDel", false);
            }
            var sql = $"SELECT TOP 1 {info.GetFullSelectColumns()} FROM [{info.DataTableName}] WHERE {search.ToWhere()} ORDER BY {search.ToOrderBy()}";
            return conn.QueryFirstOrDefaultAsync<T>(sql, search.ToDynamicParameters(), trans);
        }

        public static T LoadBySearchUtil<T>(
            this SqlConnection conn,
            MySearchUtil search,
            SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            search = search ?? MySearchUtil.New();
            if (info.IsSoftDelete)
            {
                search.AndEqual("IsDel", false);
            }
            var sql = $"SELECT TOP 1 {info.GetFullSelectColumns()} FROM [{info.DataTableName}] WHERE {search.ToWhere()} ORDER BY {search.ToOrderBy()}";
            return conn.QueryFirstOrDefault<T>(sql, search.ToDynamicParameters(), trans);
        }

        public static Task<T> LoadAsync<T>(this SqlConnection conn, string table, 
            MySearchUtil search, string fields = "*", SqlTransaction trans = null)
        {
            var sql = $"SELECT TOP 1 {fields} FROM [{table}] WHERE {search.ToWhere()} ORDER BY {search.ToOrderBy()}";
            return conn.QueryFirstOrDefaultAsync<T>(sql, search.ToDynamicParameters(), trans);
        }

        public static T Load<T>(this SqlConnection conn, string table,
            MySearchUtil search, string fields = "*", SqlTransaction trans = null)
        {
            var sql = $"SELECT TOP 1 {fields} FROM [{table}] WHERE {search.ToWhere()} ORDER BY {search.ToOrderBy()}";
            return conn.QueryFirstOrDefault<T>(sql, search.ToDynamicParameters(), trans);
        }
        #endregion

        #region 加载列表
        public static Task<IEnumerable<T>> FetchAsync<T>(
            this SqlConnection conn, MySearchUtil search, SqlTransaction trans = null) where T : BaseEntity
        {
            if (search == null) search = MySearchUtil.New();
            var info = MyContainer.Get(typeof(T));
            if(info.IsSoftDelete)
            {
                search.AndEqual("IsDel", false);
            }
            var sql = $"SELECT {info.GetFullSelectColumns()} FROM [{info.DataTableName}] WHERE {search.ToWhere()} ORDER BY {search.ToOrderBy()}";
            return conn.QueryAsync<T>(sql, search.ToDynamicParameters(), trans);
        }

        public static IEnumerable<T> Fetch<T>(
            this SqlConnection conn, MySearchUtil search, SqlTransaction trans = null) where T : BaseEntity
        {
            if (search == null) search = MySearchUtil.New();
            var info = MyContainer.Get(typeof(T));
            if (info.IsSoftDelete)
            {
                search.AndEqual("IsDel", false);
            }
            var sql = $"SELECT {info.GetFullSelectColumns()} FROM [{info.DataTableName}] WHERE {search.ToWhere()} ORDER BY {search.ToOrderBy()}";
            return conn.Query<T>(sql, search.ToDynamicParameters(), trans);
        }

        public static Task<IEnumerable<T>> FetchAsync<T>(
            this SqlConnection conn, string table, string fields, MySearchUtil search, SqlTransaction trans = null)
        {
            if (search == null) search = MySearchUtil.New();
            var sql = $"SELECT {fields} FROM [{table}] WHERE {search.ToWhere()} ORDER BY {search.ToOrderBy()}";
            return conn.QueryAsync<T>(sql, search.ToDynamicParameters(), trans);
        }

        public static IEnumerable<T> Fetch<T>(
            this SqlConnection conn, string table, string fields, MySearchUtil search, SqlTransaction trans = null)
        {
            if (search == null) search = MySearchUtil.New();
            var sql = $"SELECT {fields} FROM [{table}] WHERE {search.ToWhere()} ORDER BY {search.ToOrderBy()}";
            return conn.Query<T>(sql, search.ToDynamicParameters(), trans);
        }

        public static Task<IEnumerable<dynamic>> FetchAsync(
            this SqlConnection conn, string table, string fields, MySearchUtil search, SqlTransaction trans = null)
        {
            if (search == null) search = MySearchUtil.New();
            var sql = $"SELECT {fields} FROM [{table}] WHERE {search.ToWhere()} ORDER BY {search.ToOrderBy()}";
            return conn.QueryAsync(sql, search.ToDynamicParameters(), trans);
        }

        public static IEnumerable<dynamic> Fetch(
            this SqlConnection conn, string table, string fields, MySearchUtil search, SqlTransaction trans = null)
        {
            if (search == null) search = MySearchUtil.New();
            var sql = $"SELECT {fields} FROM [{table}] WHERE {search.ToWhere()} ORDER BY {search.ToOrderBy()}";
            return conn.Query(sql, search.ToDynamicParameters(), trans);
        }
        #endregion

        #region 分页列表
        public static async Task<PageList<T>> PageListAsync<T>(
            this SqlConnection conn, string table, string fields, 
            MySearchUtil search, int pageIndex, int pageSize, SqlTransaction trans = null)
        {
            search = search ?? MySearchUtil.New();
            var param = search.ToDynamicParameters(true);
            var sql = SqlServerSqlBuilder.GetPageQueryWithTotalCountSql(
                table, fields, search.ToWhere(), search.ToOrderBy(), pageIndex, pageSize);
            var data = await conn.QueryAsync<T>(sql, param, trans);
            var count = param.Get<int>("RecordCount");
            return new PageList<T>
            {
                RecordCount = count,
                Body = data.ToList(),
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public static PageList<T> PageList<T>(
            this SqlConnection conn, string table, string fields,
            MySearchUtil search, int pageIndex, int pageSize, SqlTransaction trans = null)
        {
            search = search ?? MySearchUtil.New();
            var param = search.ToDynamicParameters(true);
            var sql = SqlServerSqlBuilder.GetPageQueryWithTotalCountSql(
                table, fields, search.ToWhere(), search.ToOrderBy(), pageIndex, pageSize);
            var data = conn.Query<T>(sql, param, trans);
            var count = param.Get<int>("RecordCount");
            return new PageList<T>
            {
                RecordCount = count,
                Body = data.ToList(),
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public static async Task<PageList<T>> PageListAsync<T>(this SqlConnection conn, 
            MySearchUtil search, int pageIndex, int pageSize, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            search = search ?? MySearchUtil.New();
            if(info.IsSoftDelete)
            {
                search.AndEqual("IsDel", false);
            }
            return await PageListAsync<T>(conn, info.DataTableName, info.GetFullSelectColumns(), search, pageIndex, pageSize, trans);
        }

        public static PageList<T> PageList<T>(this SqlConnection conn,
            MySearchUtil search, int pageIndex, int pageSize, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            search = search ?? MySearchUtil.New();
            if (info.IsSoftDelete)
            {
                search.AndEqual("IsDel", false);
            }
            return PageList<T>(conn, info.DataTableName, info.GetFullSelectColumns(), search, pageIndex, pageSize, trans);
        }
        #endregion

        #region 数量
        public static Task<int> CountAsync<T>(this SqlConnection conn, MySearchUtil search, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            search = search ?? MySearchUtil.New();
            if(info.IsSoftDelete)
            {
                search.AndEqual("IsDel", false);
            }
            var sql = $"SELECT COUNT(0) FROM [{info.DataTableName}] WHERE {search.ToWhere()}";
            return conn.ExecuteScalarAsync<int>(sql, search.ToDynamicParameters(), trans);
        }

        public static int Count<T>(this SqlConnection conn, MySearchUtil search, SqlTransaction trans = null) where T : BaseEntity
        {
            var info = MyContainer.Get(typeof(T));
            search = search ?? MySearchUtil.New();
            if (info.IsSoftDelete)
            {
                search.AndEqual("IsDel", false);
            }
            var sql = $"SELECT COUNT(0) FROM [{info.DataTableName}] WHERE {search.ToWhere()}";
            return conn.ExecuteScalar<int>(sql, search.ToDynamicParameters(), trans);
        }

        public static Task<int> CountAsync(this SqlConnection conn, string table, MySearchUtil search, SqlTransaction trans = null)
        {
            search = search ?? MySearchUtil.New();
            var sql = $"SELECT COUNT(0) FROM [{table}] WHERE {search.ToWhere()}";
            return conn.ExecuteScalarAsync<int>(sql, search.ToDynamicParameters(), trans);
        }

        public static int Count(this SqlConnection conn, string table, MySearchUtil search, SqlTransaction trans = null)
        {
            search = search ?? MySearchUtil.New();
            var sql = $"SELECT COUNT(0) FROM [{table}] WHERE {search.ToWhere()}";
            return conn.ExecuteScalar<int>(sql, search.ToDynamicParameters(), trans);
        }
        #endregion
    }
}
