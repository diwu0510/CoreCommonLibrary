using Dapper;
using HZC.Framework;
using HZC.Framework.Datas;
using HZC.MyDapper.Conditions;
using HZC.MyDapper.SqlBuilders;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HZC.MyDapper.Extensions
{
    public static class DapperProExtensions
    {
        #region 创建

        /// <summary>
        /// 新建实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="func"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static Task<TPrimaryKey> InsertAsync<T, TPrimaryKey>(this SqlConnection conn,
            T entity,
            Func<TPrimaryKey> func = null,
            SqlTransaction trans = null)
            where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));

            var userId = func != null ? func.Invoke() : default(TPrimaryKey);

            if (info.IsCreateAudited)
            {
                ((ICreateAudit<TPrimaryKey>) entity).CreateBy = userId;
                ((ICreateAudit<TPrimaryKey>) entity).CreateAt = DateTime.Now;
            }

            if (info.IsModificationAudited)
            {
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
            }

            var sql = info.GetInsertSqlWithIdentity();
            return conn.ExecuteScalarAsync<TPrimaryKey>(sql, entity, trans);
        }

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="func"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static T Insert<T, TPrimaryKey>(this SqlConnection conn,
            T entity,
            Func<TPrimaryKey> func = null,
            SqlTransaction trans = null)
            where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));

            var userId = func != null ? func.Invoke() : default(TPrimaryKey);
   
            if (info.IsCreateAudited)
            {
                ((ICreateAudit<TPrimaryKey>)entity).CreateBy = userId;
                ((ICreateAudit<TPrimaryKey>)entity).CreateAt = DateTime.Now;
            }

            if (info.IsModificationAudited)
            {
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
            }

            var sql = info.GetInsertSqlWithIdentity();
            entity.Id = conn.ExecuteScalar<TPrimaryKey>(sql, entity, trans);
            return entity;
        }

        /// <summary>
        /// 如果不存在，新建一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="existsSearch">是否存在的查询</param>
        /// <param name="func"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static Task<TPrimaryKey> InsertIfNotExistsAsync<T, TPrimaryKey>(this SqlConnection conn,
            T entity,
            ConditionBuilder existsSearch,
            Func<TPrimaryKey> func = null,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));

            var userId = func != null ? func.Invoke() : default(TPrimaryKey);

            if (info.IsCreateAudited)
            {
                ((ICreateAudit<TPrimaryKey>)entity).CreateBy = userId;
                ((ICreateAudit<TPrimaryKey>)entity).CreateAt = DateTime.Now;
            }

            if (info.IsModificationAudited)
            {
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
            }

            var sql = info.GetInsertIfNotExistsWithIdentitySql(existsSearch);
            var param = existsSearch.ToParameters();
            param.AddDynamicParams(entity);
            return conn.ExecuteScalarAsync<TPrimaryKey>(sql, param, trans);
        }

        /// <summary>
        /// 如果不存在，新建一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="existsSearch"></param>
        /// <param name="func"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static T InsertIfNotExists<T, TPrimaryKey>(this SqlConnection conn,
            T entity,
            ConditionBuilder existsSearch,
            Func<TPrimaryKey> func = null,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));

            var userId = func != null ? func.Invoke() : default(TPrimaryKey);

            if (info.IsCreateAudited)
            {
                ((ICreateAudit<TPrimaryKey>)entity).CreateBy = userId;
                ((ICreateAudit<TPrimaryKey>)entity).CreateAt = DateTime.Now;
            }

            if (info.IsModificationAudited)
            {
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
            }

            var sql = info.GetInsertIfNotExistsWithIdentitySql(existsSearch);
            var param = existsSearch.ToParameters();
            param.AddDynamicParams(entity);
            entity.Id = conn.ExecuteScalar<TPrimaryKey>(sql, param, trans);
            return entity;
        }

        /// <summary>
        /// 批量创建实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entityList"></param>
        /// <param name="func"></param>
        /// <param name="trans"></param>
        /// <returns>插入的数量</returns>
        public static Task<int> BatchInsertAsync<T, TPrimaryKey>(this SqlConnection conn,
            IEnumerable<T> entityList,
            Func<TPrimaryKey> func = null,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));

            var userId = func != null ? func.Invoke() : default(TPrimaryKey);

            foreach (var entity in entityList)
            {
                if (info.IsCreateAudited)
                {
                    ((ICreateAudit<TPrimaryKey>)entity).CreateBy = userId;
                    ((ICreateAudit<TPrimaryKey>)entity).CreateAt = DateTime.Now;
                }

                if (info.IsModificationAudited)
                {
                    ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                    ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
                }
            }

            var sql = info.GetInsertSql();
            return conn.ExecuteAsync(sql, entityList, trans);
        }

        /// <summary>
        /// 批量创建实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entityList"></param>
        /// <param name="func"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int BatchInsert<T, TPrimaryKey>(this SqlConnection conn,
            List<T> entityList,
            Func<TPrimaryKey> func = null,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));

            var userId = func != null ? func.Invoke() : default(TPrimaryKey);

            foreach (var entity in entityList)
            {
                if (info.IsCreateAudited)
                {
                    ((ICreateAudit<TPrimaryKey>)entity).CreateBy = userId;
                    ((ICreateAudit<TPrimaryKey>)entity).CreateAt = DateTime.Now;
                }

                if (info.IsModificationAudited)
                {
                    ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                    ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
                }
            }

            var sql = info.GetInsertSql();
            return conn.Execute(sql, entityList, trans);
        }
        #endregion

        #region 修改

        /// <summary>
        /// 修改实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="func"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static Task<int> UpdateAsync<T, TPrimaryKey>(this SqlConnection conn,
            T entity,
            Func<TPrimaryKey> func = null,
            SqlTransaction trans = null)
            where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));

            var userId = func != null ? func.Invoke() : default(TPrimaryKey);

            if (info.IsModificationAudited)
            {
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
            }

            var sql = info.GetDefaultUpdateSql();
            return conn.ExecuteAsync(sql, entity, trans);
        }

        /// <summary>
        /// 修改实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="func"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int Update<T, TPrimaryKey>(this SqlConnection conn,
            T entity,
            Func<TPrimaryKey> func = null,
            SqlTransaction trans = null)
            where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));

            var userId = func != null ? func.Invoke() : default(TPrimaryKey);

            if (info.IsModificationAudited)
            {
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
            }

            var sql = info.GetDefaultUpdateSql();
            return conn.Execute(sql, entity, trans);
        }

        /// <summary>
        /// 如果不存在，修改。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="existsUtil">必须使用 ConditionBuilder.NewExists()获取</param>
        /// <param name="func"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static Task<int> UpdateIfNotExistsAsync<T, TPrimaryKey>(this SqlConnection conn,
            T entity,
            ConditionBuilder existsUtil,
            Func<TPrimaryKey> func = null,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));

            var userId = func != null ? func.Invoke() : default(TPrimaryKey);

            if (info.IsModificationAudited)
            {
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
            }

            var sql = info.GetDefaultUpdateIfNotExistsSql(existsUtil);
            var param = existsUtil.ToParameters();
            param.AddDynamicParams(entity);
            return conn.ExecuteAsync(sql, param, trans);
        }

        /// <summary>
        /// 如果不存在，修改。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="existsUtil"></param>
        /// <param name="func"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int UpdateIfNotExists<T, TPrimaryKey>(this SqlConnection conn,
            T entity,
            ConditionBuilder existsUtil,
            Func<TPrimaryKey> func = null,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));

            var userId = func != null ? func.Invoke() : default(TPrimaryKey);

            if (info.IsModificationAudited)
            {
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
            }

            var sql = info.GetDefaultUpdateIfNotExistsSql(existsUtil);
            var param = existsUtil.ToParameters();
            param.AddDynamicParams(entity);
            return conn.Execute(sql, param, trans);
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entityList"></param>
        /// <param name="func"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static Task<int> BatchUpdateAsync<T, TPrimaryKey>(this SqlConnection conn,
            List<T> entityList,
            Func<TPrimaryKey> func = null,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));

            var userId = func != null ? func.Invoke() : default(TPrimaryKey);

            if (info.IsModificationAudited)
            {
                foreach (var entity in entityList)
                {
                    ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                    ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
                }
            }

            var sql = info.GetDefaultUpdateSql();
            return conn.ExecuteAsync(sql, entityList, trans);
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entityList"></param>
        /// <param name="func"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int BatchUpdate<T, TPrimaryKey>(this SqlConnection conn,
            List<T> entityList,
            Func<TPrimaryKey> func = null,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));

            var userId = func != null ? func.Invoke() : default(TPrimaryKey);

            if (info.IsModificationAudited)
            {
                foreach (var entity in entityList)
                {
                    ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                    ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
                }
            }

            var sql = info.GetDefaultUpdateSql();
            return conn.Execute(sql, entityList, trans);
        }

        /// <summary>
        /// 批量修改指定属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="properties"></param>
        /// <param name="func"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static Task<int> UpdateIncludeAsync<T, TPrimaryKey>(this SqlConnection conn,
            T entity,
            IEnumerable<string> properties,
            Func<TPrimaryKey> func = null,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));

            var userId = func != null ? func.Invoke() : default(TPrimaryKey);

            if (info.IsModificationAudited)
            {
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
            }

            var sql = SqlServerBuilder.GetUpdateIncludeSql(info, properties);
            return conn.ExecuteAsync(sql, entity, trans);
        }

        /// <summary>
        /// 批量修改指定属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="properties"></param>
        /// <param name="func"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int UpdateInclude<T, TPrimaryKey>(this SqlConnection conn,
            T entity,
            IEnumerable<string> properties,
            Func<TPrimaryKey> func = null,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));

            var userId = func != null ? func.Invoke() : default(TPrimaryKey);

            if (info.IsModificationAudited)
            {
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
            }

            var sql = SqlServerBuilder.GetUpdateIncludeSql(info, properties);
            return conn.Execute(sql, entity, trans);
        }

        /// <summary>
        /// 批量修改指定属性以外的属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="properties"></param>
        /// <param name="func"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static Task<int> UpdateExcludeAsync<T, TPrimaryKey>(this SqlConnection conn,
            T entity,
            IEnumerable<string> properties,
            Func<TPrimaryKey> func = null,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));

            var userId = func != null ? func.Invoke() : default(TPrimaryKey);

            if (info.IsModificationAudited)
            {
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
            }

            var sql = SqlServerBuilder.GetUpdateExcludeSql(info, properties);
            return conn.ExecuteAsync(sql, entity, trans);
        }

        /// <summary>
        /// 批量修改指定属性以外的属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="properties"></param>
        /// <param name="func"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int UpdateExclude<T, TPrimaryKey>(this SqlConnection conn,
            T entity,
            IEnumerable<string> properties,
            Func<TPrimaryKey> func = null,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));

            var userId = func != null ? func.Invoke() : default(TPrimaryKey);

            if (info.IsModificationAudited)
            {
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
            }

            var sql = SqlServerBuilder.GetUpdateExcludeSql(info, properties);
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
            string table, SoKeyValuePairs kvs, ConditionBuilder util, SqlTransaction trans = null)
        {
            var sql = SqlServerBuilder.GetUpdateSetSql(table, kvs, util);
            var param = kvs.ToParameters();
            param.AddDynamicParams(util.ToParameters());
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
            string table, SoKeyValuePairs kvs, ConditionBuilder util, SqlTransaction trans = null)
        {
            var sql = SqlServerBuilder.GetUpdateSetSql(table, kvs, util);
            var param = kvs.ToParameters();
            param.AddDynamicParams(util.ToParameters());
            return conn.Execute(sql, param, trans);
        }

        /// <summary>
        /// 修改指定实体的指定属性为指定值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="kvs"></param>
        /// <param name="func"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static Task<int> SetAsync<T, TPrimaryKey>(this SqlConnection conn,
            T entity, 
            SoKeyValuePairs kvs,
            Func<TPrimaryKey> func = null,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));

            var userId = func != null ? func.Invoke() : default(TPrimaryKey);

            if (info.IsModificationAudited)
            {
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
            }

            var sql = SqlServerBuilder.GetUpdateSetSql(info, kvs);
            var param = kvs.ToParameters();
            param.Add("Id", entity.Id);
            return conn.ExecuteAsync(sql, param, trans);
        }

        /// <summary>
        /// 修改指定实体的指定属性为指定值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="conn"></param>
        /// <param name="entity"></param>
        /// <param name="kvs"></param>
        /// <param name="func"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int Set<T, TPrimaryKey>(this SqlConnection conn,
            T entity, 
            SoKeyValuePairs kvs,
            Func<TPrimaryKey> func = null,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));

            var userId = func != null ? func.Invoke() : default(TPrimaryKey);

            if (info.IsModificationAudited)
            {
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = userId;
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateAt = DateTime.Now;
            }

            var sql = SqlServerBuilder.GetUpdateSetSql(info, kvs);
            var param = kvs.ToParameters();
            param.Add("Id", entity.Id);
            return conn.Execute(sql, param, trans);
        }

        /// <summary>
        /// 修改符合条件的所有实体的指定属性为指定的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="conn"></param>
        /// <param name="kvs"></param>
        /// <param name="search"></param>
        /// <param name="func"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static Task<int> BatchSetAsync<T, TPrimaryKey>(this SqlConnection conn,
            SoKeyValuePairs kvs, 
            ConditionBuilder search,
            Func<TPrimaryKey> func = null,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));

            var userId = func != null ? func.Invoke() : default(TPrimaryKey);

            if (info.IsModificationAudited)
            {
                kvs.Add("UpdateBy", userId);
                kvs.Add("UpdateAt", DateTime.Now);
            }

            var sql = SqlServerBuilder.GetUpdateSetSql(info, kvs);
            var param = kvs.ToParameters();
            param.AddDynamicParams(search.ToParameters());
            return conn.ExecuteAsync(sql, param, trans);
        }

        /// <summary>
        /// 修改符合条件的所有实体的指定属性为指定的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="conn"></param>
        /// <param name="kvs"></param>
        /// <param name="search"></param>
        /// <param name="func"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int BatchSet<T, TPrimaryKey>(this SqlConnection conn,
            SoKeyValuePairs kvs,
            ConditionBuilder search,
            Func<TPrimaryKey> func = null,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));

            var userId = func != null ? func.Invoke() : default(TPrimaryKey);

            if (info.IsModificationAudited)
            {
                kvs.Add("UpdateBy", userId);
                kvs.Add("UpdateAt", DateTime.Now);
            }

            var sql = SqlServerBuilder.GetUpdateSetSql(info.DataTableName, kvs, search);

            var param = kvs.ToParameters();
            param.AddDynamicParams(search.ToParameters());
            return conn.Execute(sql, param, trans);
        }
        #endregion

        #region 删除
        public static Task<int> DeleteAsync<T, TPrimaryKey>(this SqlConnection conn, TPrimaryKey id, SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));
            var sql = info.GetDefaultDeleteSql();
            return conn.ExecuteAsync(sql, new { Id = id }, trans);
        }

        public static int Delete<T, TPrimaryKey>(this SqlConnection conn, TPrimaryKey id, SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));
            var sql = info.GetDefaultDeleteSql();
            return conn.Execute(sql, new { Id = id }, trans);
        }

        public static Task<int> DeleteAsync<T, TPrimaryKey>(this SqlConnection conn, IEnumerable<TPrimaryKey> ids, SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));
            var sql = info.IsSoftDelete
                ? $"UPDATE [{info.DataTableName}] SET IsDel=1 WHERE Id IN @Ids"
                : $"DELETE [{info.DataTableName}] WHERE Id IN @Ids";
            return conn.ExecuteAsync(sql, new { Ids = ids }, trans);
        }

        public static int Delete<T, TPrimaryKey>(this SqlConnection conn, IEnumerable<TPrimaryKey> ids, SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));
            var sql = info.IsSoftDelete
                ? $"UPDATE [{info.DataTableName}] SET IsDel=1 WHERE Id IN @Ids"
                : $"DELETE [{info.DataTableName}] WHERE Id IN @Ids";
            return conn.Execute(sql, new { Ids = ids }, trans);
        }

        public static Task<int> DeleteAsync<TPrimaryKey>(this SqlConnection conn, string table, TPrimaryKey id)
        {
            var sql = $"DELETE [{table}] WHERE Id=@Id";
            return conn.ExecuteAsync(sql, new { Id = id });
        }

        public static int Delete<TPrimaryKey>(this SqlConnection conn, string table, TPrimaryKey id)
        {
            var sql = $"DELETE [{table}] WHERE Id=@Id";
            return conn.Execute(sql, new { Id = id });
        }

        public static Task<int> DeleteAsync(this SqlConnection conn, string table, ConditionBuilder search)
        {
            search = search ?? ConditionBuilder.New();
            var sql = $"DELETE [{table}] WHERE {search.ToCondition()}";
            return conn.ExecuteAsync(sql, search.ToParameters());
        }

        public static int Delete(this SqlConnection conn, string table, ConditionBuilder search)
        {
            search = search ?? ConditionBuilder.New();
            var sql = $"DELETE [{table}] WHERE {search.ToCondition()}";
            return conn.Execute(sql, search.ToParameters());
        }

        public static Task<int> DeleteBySearchAsync<T, TPrimaryKey>(this SqlConnection conn,
            ConditionBuilder search) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));
            var sql = info.GetDeleteSql(search);
            return conn.ExecuteAsync(sql, search.ToParameters());
        }

        public static int DeleteBySearch<T, TPrimaryKey>(this SqlConnection conn, ConditionBuilder search) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));
            var sql = info.GetDeleteSql(search);
            return conn.Execute(sql, search.ToParameters());
        }

        public static Task<int> RemoveAsync(this SqlConnection conn, string table, ConditionBuilder search)
        {
            search = search ?? ConditionBuilder.New();
            var sql = $"UPDATE [{table}] SET IsDel=1 WHERE {search.ToCondition()}";
            return conn.ExecuteAsync(sql, search.ToParameters());
        }

        public static int Remove(this SqlConnection conn, string table, ConditionBuilder search)
        {
            search = search ?? ConditionBuilder.New();
            var sql = $"UPDATE [{table}] SET IsDel=1 WHERE {search.ToCondition()}";
            return conn.Execute(sql, search.ToParameters());
        }
        #endregion

        #region 加载实体
        public static Task<T> LoadAsync<T, TPrimaryKey>(this SqlConnection conn,
            TPrimaryKey id,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));
            var sql = $"SELECT {info.GetFullSelectColumns()} FROM [{info.DataTableName}] WHERE Id=@Id";
            if (info.IsSoftDelete)
            {
                sql += " AND IsDel=0";
            }
            return conn.QueryFirstOrDefaultAsync<T>(sql, new { Id = id }, trans);
        }

        public static T Load<T, TPrimaryKey>(this SqlConnection conn,
            TPrimaryKey id,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));
            var sql = $"SELECT {info.GetFullSelectColumns()} FROM [{info.DataTableName}] WHERE Id=@Id";
            if (info.IsSoftDelete)
            {
                sql += " AND IsDel=0";
            }
            return conn.QueryFirstOrDefault<T>(sql, new { Id = id }, trans);
        }

        public static Task<T> LoadBySearchUtilAsync<T, TPrimaryKey>(
            this SqlConnection conn,
            ConditionBuilder search,
            SortBuilder sort,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));
            search = search ?? ConditionBuilder.New();
            if (info.IsSoftDelete)
            {
                search.AndEqual("IsDel", false);
            }
            var sql = $"SELECT TOP 1 {info.GetFullSelectColumns()} FROM [{info.DataTableName}] WHERE {search.ToCondition()} ORDER BY {sort.ToOrderBy()}";
            return conn.QueryFirstOrDefaultAsync<T>(sql, search.ToParameters(), trans);
        }

        public static T LoadBySearchUtil<T, TPrimaryKey>(
            this SqlConnection conn,
            ConditionBuilder search,
            SortBuilder sort,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));
            search = search ?? ConditionBuilder.New();
            if (info.IsSoftDelete)
            {
                search.AndEqual("IsDel", false);
            }
            var sql = $"SELECT TOP 1 {info.GetFullSelectColumns()} FROM [{info.DataTableName}] WHERE {search.ToCondition()} ORDER BY {sort.ToOrderBy()}";
            return conn.QueryFirstOrDefault<T>(sql, search.ToParameters(), trans);
        }

        public static Task<T> LoadAsync<T>(this SqlConnection conn, string table,
            ConditionBuilder search, SortBuilder sort, string fields = "*", SqlTransaction trans = null)
        {
            var sql = $"SELECT TOP 1 {fields} FROM [{table}] WHERE {search.ToCondition()} ORDER BY {sort.ToOrderBy()}";
            return conn.QueryFirstOrDefaultAsync<T>(sql, search.ToParameters(), trans);
        }

        public static T Load<T>(this SqlConnection conn, string table,
            ConditionBuilder search, SortBuilder sort, string fields = "*", SqlTransaction trans = null)
        {
            var sql = $"SELECT TOP 1 {fields} FROM [{table}] WHERE {search.ToCondition()} ORDER BY {sort.ToOrderBy()}";
            return conn.QueryFirstOrDefault<T>(sql, search.ToParameters(), trans);
        }
        #endregion

        #region 加载列表
        public static Task<IEnumerable<T>> FetchAsync<T, TPrimaryKey>(
            this SqlConnection conn,
            ConditionBuilder search,
            SortBuilder sort,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            if (search == null) search = ConditionBuilder.New();
            var info = MyContainer.Get(typeof(T));
            if (info.IsSoftDelete)
            {
                search.AndEqual("IsDel", false);
            }
            var sql = $"SELECT {info.GetFullSelectColumns()} FROM [{info.DataTableName}] WHERE {search.ToCondition()} ORDER BY {sort.ToOrderBy()}";
            return conn.QueryAsync<T>(sql, search.ToParameters(), trans);
        }

        public static IEnumerable<T> Fetch<T, TPrimaryKey>(
            this SqlConnection conn,
            ConditionBuilder search,
            SortBuilder sort,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            if (search == null) search = ConditionBuilder.New();
            var info = MyContainer.Get(typeof(T));
            if (info.IsSoftDelete)
            {
                search.AndEqual("IsDel", false);
            }
            var sql = $"SELECT {info.GetFullSelectColumns()} FROM [{info.DataTableName}] WHERE {search.ToCondition()} ORDER BY {sort.ToOrderBy()}";
            return conn.Query<T>(sql, search.ToParameters(), trans);
        }

        public static Task<IEnumerable<T>> FetchAsync<T>(
            this SqlConnection conn, string table, string fields, ConditionBuilder search, SortBuilder sort, SqlTransaction trans = null)
        {
            if (search == null) search = ConditionBuilder.New();
            var info = MyContainer.Get(typeof(T));
            if (info.IsSoftDelete)
            {
                search.AndEqual("IsDel", false);
            }
            var sql = $"SELECT {fields} FROM [{table}] WHERE {search.ToCondition()} ORDER BY {sort.ToOrderBy()}";
            return conn.QueryAsync<T>(sql, search.ToParameters(), trans);
        }

        public static IEnumerable<T> Fetch<T>(
            this SqlConnection conn, string table, string fields, ConditionBuilder search, SortBuilder sort, SqlTransaction trans = null)
        {
            if (search == null) search = ConditionBuilder.New();
            var info = MyContainer.Get(typeof(T));
            if (info.IsSoftDelete)
            {
                search.AndEqual("IsDel", false);
            }
            var sql = $"SELECT {fields} FROM [{table}] WHERE {search.ToCondition()} ORDER BY {sort.ToOrderBy()}";
            return conn.Query<T>(sql, search.ToParameters(), trans);
        }

        public static Task<IEnumerable<dynamic>> FetchAsync(
            this SqlConnection conn, string table, string fields, ConditionBuilder search, SortBuilder sort, SqlTransaction trans = null)
        {
            if (search == null) search = ConditionBuilder.New();
            var sql = $"SELECT {fields} FROM [{table}] WHERE {search.ToCondition()} ORDER BY {sort.ToOrderBy()}";
            return conn.QueryAsync(sql, search.ToParameters(), trans);
        }

        public static IEnumerable<dynamic> Fetch(
            this SqlConnection conn, string table, string fields, ConditionBuilder search, SortBuilder sort, SqlTransaction trans = null)
        {
            if (search == null) search = ConditionBuilder.New();
            var sql = $"SELECT {fields} FROM [{table}] WHERE {search.ToCondition()} ORDER BY {sort.ToOrderBy()}";
            return conn.Query(sql, search.ToParameters(), trans);
        }
        #endregion

        #region 分页列表
        public static async Task<PageList<T>> PageListAsync<T>(
            this SqlConnection conn, string table, string fields,
            ConditionBuilder search, SortBuilder sort,
            int pageIndex, int pageSize, SqlTransaction trans = null)
        {
            search = search ?? ConditionBuilder.New();
            var param = search.ToParameters();
            var sql = SqlServerBuilder.GetPageQueryWithTotalCountSql(
                table, fields, search.ToCondition(), sort.ToOrderBy(), pageIndex, pageSize);
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
            ConditionBuilder search, SortBuilder sort, int pageIndex, int pageSize, SqlTransaction trans = null)
        {
            search = search ?? ConditionBuilder.New();
            var info = MyContainer.Get(typeof(T));
            if (info.IsSoftDelete)
            {
                search.AndEqual("IsDel", false);
            }
            var param = search.ToParameters();
            var sql = SqlServerBuilder.GetPageQueryWithTotalCountSql(
                table, fields, search.ToCondition(), sort.ToOrderBy(), pageIndex, pageSize);
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

        public static async Task<PageList<T>> PageListAsync<T, TPrimaryKey>(this SqlConnection conn,
            ConditionBuilder search,
            SortBuilder sort,
            int pageIndex,
            int pageSize,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));
            search = search ?? ConditionBuilder.New();
            if (info.IsSoftDelete)
            {
                search.AndEqual("IsDel", false);
            }
            return await PageListAsync<T>(conn, info.DataTableName, info.GetFullSelectColumns(), search, sort, pageIndex, pageSize, trans);
        }

        public static PageList<T> PageList<T, TPrimaryKey>(this SqlConnection conn,
            ConditionBuilder search,
            SortBuilder sort,
            int pageIndex,
            int pageSize,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));
            search = search ?? ConditionBuilder.New();
            if (info.IsSoftDelete)
            {
                search.AndEqual("IsDel", false);
            }
            return PageList<T>(conn, info.DataTableName, info.GetFullSelectColumns(), search, sort, pageIndex, pageSize, trans);
        }
        #endregion

        #region 数量
        public static Task<int> CountAsync<T, TPrimaryKey>(this SqlConnection conn,
            ConditionBuilder search,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));
            search = search ?? ConditionBuilder.New();
            if (info.IsSoftDelete)
            {
                search.AndEqual("IsDel", false);
            }
            var sql = $"SELECT COUNT(0) FROM [{info.DataTableName}] WHERE {search.ToCondition()}";
            return conn.ExecuteScalarAsync<int>(sql, search.ToParameters(), trans);
        }

        public static int Count<T, TPrimaryKey>(this SqlConnection conn,
            ConditionBuilder search,
            SqlTransaction trans = null) where T : IEntity<TPrimaryKey>
        {
            var info = MyContainer.Get(typeof(T));
            search = search ?? ConditionBuilder.New();
            if (info.IsSoftDelete)
            {
                search.AndEqual("IsDel", false);
            }
            var sql = $"SELECT COUNT(0) FROM [{info.DataTableName}] WHERE {search.ToCondition()}";
            return conn.ExecuteScalar<int>(sql, search.ToParameters(), trans);
        }

        public static Task<int> CountAsync(this SqlConnection conn,
            string table,
            ConditionBuilder search,
            SqlTransaction trans = null)
        {
            search = search ?? ConditionBuilder.New();
            var sql = $"SELECT COUNT(0) FROM [{table}] WHERE {search.ToCondition()}";
            return conn.ExecuteScalarAsync<int>(sql, search.ToParameters(), trans);
        }

        public static int Count(this SqlConnection conn,
            string table,
            ConditionBuilder search,
            SqlTransaction trans = null)
        {
            search = search ?? ConditionBuilder.New();
            var sql = $"SELECT COUNT(0) FROM [{table}] WHERE {search.ToCondition()}";
            return conn.ExecuteScalar<int>(sql, search.ToParameters(), trans);
        }
        #endregion
    }
}
