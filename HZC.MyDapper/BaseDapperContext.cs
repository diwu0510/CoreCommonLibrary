using Dapper;
using HZC.Framework;
using HZC.Framework.Datas;
using HZC.MyDapper.Conditions;
using HZC.MyDapper.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HZC.MyDapper
{
    public abstract class BaseDapperContext<TPrimaryKey>
    {
        private readonly string _connectionString;

        private readonly Func<TPrimaryKey> _getUserIdFunc;

        protected BaseDapperContext(DapperContextOption<TPrimaryKey> options)
        {
            _getUserIdFunc = options.GetAuditorIdDelegate;
            _connectionString = options.ConnectionString;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        #region 增
        public async Task<TPrimaryKey> InsertAsync<T>(T entity) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return await conn.InsertAsync(entity, _getUserIdFunc);
            }
        }

        public T Insert<T>(T entity) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return conn.Insert(entity, _getUserIdFunc);
            }
        }

        public async Task<int> InsertAsync<T>(List<T> entityList) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return await conn.BatchInsertAsync(entityList, _getUserIdFunc);
            }
        }

        public int Insert<T>(List<T> entityList) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return conn.BatchInsert(entityList, _getUserIdFunc);
            }
        }

        public async Task<TPrimaryKey> InsertIfNotExistsAsync<T>(T entity, ConditionBuilder existsSearch) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return await conn.InsertIfNotExistsAsync(entity, existsSearch, _getUserIdFunc);
            }
        }

        public T InsertIfNotExists<T>(T entity, ConditionBuilder existsSearch) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return conn.InsertIfNotExists(entity, existsSearch, _getUserIdFunc);
            }
        }
        #endregion

        #region 改
        public async Task<int> UpdateAsync<T>(T entity) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return await conn.UpdateAsync(entity, _getUserIdFunc);
            }
        }

        public int Update<T>(T entity) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return conn.Update(entity, _getUserIdFunc);
            }
        }

        public async Task<int> UpdateAsync<T>(List<T> entityList) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return await conn.BatchUpdateAsync(entityList, _getUserIdFunc);
            }
        }

        public int Update<T>(List<T> entityList) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return conn.BatchUpdate(entityList, _getUserIdFunc);
            }
        }

        public async Task<int> UpdateIfNotExistsAsync<T>(T entity, ConditionBuilder existsSearch) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return await conn.UpdateIfNotExistsAsync(entity, existsSearch, _getUserIdFunc);
            }
        }

        public int UpdateIfNotExists<T>(T entity, ConditionBuilder existsSearch) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return conn.UpdateIfNotExists(entity, existsSearch, _getUserIdFunc);
            }
        }

        public async Task<int> UpdateIncludeAsync<T>(T entity, IEnumerable<string> properties) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return await conn.UpdateIncludeAsync(entity, properties, _getUserIdFunc);
            }
        }

        public int UpdateInclude<T>(T entity, IEnumerable<string> properties) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return conn.UpdateInclude(entity, properties, _getUserIdFunc);
            }
        }

        public async Task<int> UpdateExcludeAsync<T>(T entity, IEnumerable<string> properties) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return await conn.UpdateExcludeAsync(entity, properties, _getUserIdFunc);
            }
        }

        public int UpdateExclude<T>(T entity, IEnumerable<string> properties) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return conn.UpdateExclude(entity, properties, _getUserIdFunc);
            }
        }
        #endregion

        #region 修改部分字段
        public async Task<int> SetAsync<T>(T entity, SoKeyValuePairs kvs) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return await conn.SetAsync(entity, kvs, _getUserIdFunc);
            }
        }

        public int Set<T>(T entity, SoKeyValuePairs kvs) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return conn.Set(entity, kvs, _getUserIdFunc);
            }
        }

        public async Task<int> SetByConditionAsync<T>(ConditionBuilder search, SoKeyValuePairs kvs) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return await conn.BatchSetAsync<T, TPrimaryKey>(kvs, search, _getUserIdFunc);
            }
        }

        public int SetByCondition<T>(ConditionBuilder search, SoKeyValuePairs kvs) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return conn.BatchSet<T, TPrimaryKey>(kvs, search, _getUserIdFunc);
            }
        }

        public async Task<int> SetByConditionAndTableAsync(string table, SoKeyValuePairs kvs, ConditionBuilder search)
        {
            using (var conn = GetConnection())
            {
                return await conn.SetAsync(table, kvs, search);
            }
        }

        public int SetByConditionAndTable(string table, SoKeyValuePairs kvs, ConditionBuilder search)
        {
            using (var conn = GetConnection())
            {
                return conn.Set(table, kvs, search);
            }
        }
        #endregion

        #region 加载实体
        public async Task<T> LoadAsync<T>(TPrimaryKey id) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return await conn.LoadAsync<T, TPrimaryKey>(id);
            }
        }

        public T Load<T>(TPrimaryKey id) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return conn.Load<T, TPrimaryKey>(id);
            }
        }

        public async Task<T> LoadAsync<T>(ConditionBuilder search, SortBuilder sort) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return await conn.LoadBySearchUtilAsync<T, TPrimaryKey>(search, sort);
            }
        }

        public T Load<T>(ConditionBuilder search, SortBuilder sort) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return conn.LoadBySearchUtil<T, TPrimaryKey>(search, sort);
            }
        }

        public async Task<T> LoadAsync<T>(string table, ConditionBuilder search, SortBuilder sort, string fields = "*")
        {
            using (var conn = GetConnection())
            {
                return await conn.LoadAsync<T>(table, search, sort, fields);
            }
        }

        public T Load<T>(string table, ConditionBuilder search, SortBuilder sort, string fields = "*")
        {
            using (var conn = GetConnection())
            {
                return conn.Load<T>(table, search, sort, fields);
            }
        }
        #endregion

        #region 列表
        public async Task<List<T>> FetchAsync<T>(ConditionBuilder search, SortBuilder sort) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                var data = await conn.FetchAsync<T, TPrimaryKey>(search, sort);
                return data.ToList();
            }
        }

        public List<T> Fetch<T>(ConditionBuilder search, SortBuilder sort) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                var data = conn.Fetch<T, TPrimaryKey>(search, sort);
                return data.ToList();
            }
        }

        public async Task<List<T>> FetchAsync<T>(string table, string fields, ConditionBuilder search, SortBuilder sort)
        {
            using (var conn = GetConnection())
            {
                var data = await conn.FetchAsync<T>(table, fields, search, sort);
                return data.ToList();
            }
        }

        public List<T> Fetch<T>(string table, string fields, ConditionBuilder search, SortBuilder sort)
        {
            using (var conn = GetConnection())
            {
                var data = conn.Fetch<T>(table, fields, search, sort);
                return data.ToList();
            }
        }
        #endregion

        #region 分页
        public async Task<PageList<T>> PageListAsync<T>(ConditionBuilder search, SortBuilder sort, int pageIndex, int pageSize) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return await conn.PageListAsync<T, TPrimaryKey>(search, sort, pageIndex, pageSize);
            }
        }

        public PageList<T> PageList<T>(ConditionBuilder search, SortBuilder sort, int pageIndex, int pageSize) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return conn.PageList<T, TPrimaryKey>(search, sort, pageIndex, pageSize);
            }
        }

        public async Task<PageList<T>> PageListAsync<T>(string table, string fields, ConditionBuilder search, SortBuilder sort, int pageIndex, int pageSize)
        {
            using (var conn = GetConnection())
            {
                return await conn.PageListAsync<T>(table, fields, search, sort, pageIndex, pageSize);
            }
        }

        public PageList<T> PageList<T>(string table, string fields, ConditionBuilder search, SortBuilder sort, int pageIndex, int pageSize)
        {
            using (var conn = GetConnection())
            {
                return conn.PageList<T>(table, fields, search, sort, pageIndex, pageSize);
            }
        }
        #endregion

        #region 删除
        public async Task<int> DeleteAsync<T>(TPrimaryKey id) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return await conn.DeleteAsync<T, TPrimaryKey>(id);
            }
        }

        public int Delete<T>(TPrimaryKey id) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return conn.Delete<T, TPrimaryKey>(id);
            }
        }

        public async Task<int> DeleteAsync<T>(IEnumerable<TPrimaryKey> ids) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return await conn.DeleteAsync<T, TPrimaryKey>(ids);
            }
        }

        public int Delete<T>(IEnumerable<TPrimaryKey> ids) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return conn.Delete<T, TPrimaryKey>(ids);
            }
        }

        public async Task<int> DeleteAsync(string table, int id)
        {
            using (var conn = GetConnection())
            {
                return await conn.DeleteAsync(table, id);
            }
        }

        public int Delete(string table, int id)
        {
            using (var conn = GetConnection())
            {
                return conn.Delete(table, id);
            }
        }

        public async Task<int> DeleteAsync(string table, ConditionBuilder search)
        {
            using (var conn = GetConnection())
            {
                return await conn.DeleteAsync(table, search);
            }
        }

        public int Delete(string table, ConditionBuilder search)
        {
            using (var conn = GetConnection())
            {
                return conn.Delete(table, search);
            }
        }

        public async Task<int> RemoveAsync(string table, ConditionBuilder search)
        {
            using (var conn = GetConnection())
            {
                return await conn.RemoveAsync(table, search);
            }
        }

        public int Remove(string table, ConditionBuilder search)
        {
            using (var conn = GetConnection())
            {
                return conn.Remove(table, search);
            }
        }

        public async Task<int> DeleteAsync<T>(ConditionBuilder search) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return await conn.DeleteBySearchAsync<T, TPrimaryKey>(search);
            }
        }

        public int Delete<T>(ConditionBuilder search) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return conn.DeleteBySearch<T, TPrimaryKey>(search);
            }
        }
        #endregion

        #region 数量
        public async Task<int> CountAsync<T>(ConditionBuilder search) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return await conn.CountAsync<T, TPrimaryKey>(search);
            }
        }

        public int Count<T>(ConditionBuilder search) where T : IEntity<TPrimaryKey>
        {
            using (var conn = GetConnection())
            {
                return conn.Count<T, TPrimaryKey>(search);
            }
        }

        public async Task<int> CountAsync(string table, ConditionBuilder search)
        {
            using (var conn = GetConnection())
            {
                return await conn.CountAsync(table, search);
            }
        }

        public int Count(string table, ConditionBuilder search)
        {
            using (var conn = GetConnection())
            {
                return conn.Count(table, search);
            }
        }
        #endregion

        #region 通用方法
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int Execute(string sql, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Execute(sql, param);
            }
        }

        public async Task<int> ExecuteAsync(string sql, object param = null)
        {
            using (var conn = GetConnection())
            {
                return await conn.ExecuteAsync(sql, param);
            }
        }

        /// <summary>
        /// 执行并获取第一行第一列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string sql, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.ExecuteScalar<T>(sql, param);
            }
        }

        public async Task<T> ExecuteScalarAsync<T>(string sql, object param = null)
        {
            using (var conn = GetConnection())
            {
                return await conn.ExecuteScalarAsync<T>(sql, param);
            }
        }

        public IEnumerable<T> Query<T>(string sql, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Query<T>(sql, param);
            }
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null)
        {
            using (var conn = GetConnection())
            {
                return await conn.QueryAsync<T>(sql, param);
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int ExecuteProc(string procName, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.Execute(procName, param, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> ExecuteProcAsync(string procName, object param = null)
        {
            using (var conn = GetConnection())
            {
                return await conn.ExecuteAsync(procName, param, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// 执行存储过程并获取第一行第一列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T ExecuteProc<T>(string procName, object param = null)
        {
            using (var conn = GetConnection())
            {
                return conn.ExecuteScalar<T>(procName, param, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<T> ExecuteProcAsync<T>(string procName, object param = null)
        {
            using (var conn = GetConnection())
            {
                return await conn.ExecuteScalarAsync<T>(procName, param, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="sqlList"></param>
        /// <returns></returns>
        public bool ExecuteTran(string[] sqlList)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var s in sqlList)
                        {
                            conn.Execute(s, null, tran);
                        }
                        tran.Commit();
                        return true;
                    }
                    catch
                    {
                        tran.Rollback();
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="kvsKeyValuePairs"></param>
        /// <returns></returns>
        public bool ExecuteTran(SoKeyValuePairs kvsKeyValuePairs)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var s in kvsKeyValuePairs)
                        {
                            conn.Execute(s.Key, s.Value, tran);
                        }
                        tran.Commit();
                        return true;
                    }
                    catch
                    {
                        tran.Rollback();
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 多数据集
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>> MultiQuery<T1, T2>(string sql,
            object param = null,
            CommandType commandType = CommandType.Text)
        {
            using (var conn = GetConnection())
            {
                using (var multi = conn.QueryMultiple(sql, param, commandType: commandType))
                {
                    var list1 = multi.Read<T1>();
                    var list2 = multi.Read<T2>();

                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(list1, list2);
                }
            }
        }

        /// <summary>
        /// 多数据集
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> MultiQuery<T1, T2, T3>(string sql,
            object param = null,
            CommandType commandType = CommandType.Text)
        {
            using (var conn = GetConnection())
            {
                using (var multi = conn.QueryMultiple(sql, param, commandType: commandType))
                {
                    var list1 = multi.Read<T1>();
                    var list2 = multi.Read<T2>();
                    var list3 = multi.Read<T3>();

                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>(list1, list2, list3);
                }
            }
        }

        /// <summary>
        /// 多数据集
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> MultiQuery<T1, T2, T3, T4>(
            string sql,
            object param = null,
            CommandType commandType = CommandType.Text)
        {
            using (var conn = GetConnection())
            {
                using (var multi = conn.QueryMultiple(sql, param, commandType: commandType))
                {
                    var list1 = multi.Read<T1>();
                    var list2 = multi.Read<T2>();
                    var list3 = multi.Read<T3>();
                    var list4 = multi.Read<T4>();

                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>(list1, list2, list3, list4);
                }
            }
        }

        /// <summary>
        /// 多数据集
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> MultiQuery<T1, T2, T3, T4, T5>(string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            using (var conn = GetConnection())
            {
                using (var multi = conn.QueryMultiple(sql, param, commandType: commandType))
                {
                    var list1 = multi.Read<T1>();
                    var list2 = multi.Read<T2>();
                    var list3 = multi.Read<T3>();
                    var list4 = multi.Read<T4>();
                    var list5 = multi.Read<T5>();

                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>(list1, list2, list3, list4, list5);
                }
            }
        }
        #endregion

        #region 辅助方法
        #endregion
    }
}
