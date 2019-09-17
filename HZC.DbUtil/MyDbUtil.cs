using Dapper;
using HZC.Core;
using HZC.DbUtil.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HZC.DbUtil
{
    public class MyDbUtil
    {
        private readonly string _connectionString;

        public MyDbUtil()
        {
            _connectionString = MyDbUtilConfiguration.GetDefaultConnectionString();
        }

        public MyDbUtil(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        #region 增
        public async Task<int> InsertAsync<T>(T entity) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return await conn.InsertAsync(entity);
            }
        }

        public T Insert<T>(T entity) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return conn.Insert(entity);
            }
        }

        public async Task<int> InsertAsync<T>(List<T> entityList) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return await conn.BatchInsertAsync(entityList);
            }
        }

        public int Insert<T>(List<T> entityList) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return conn.BatchInsert(entityList);
            }
        }

        public async Task<int> InsertIfNotExistsAsync<T>(T entity, MySearchUtil existsSearch) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return await conn.InsertIfNotExistsAsync(entity, existsSearch);
            }
        }

        public T InsertIfNotExists<T>(T entity, MySearchUtil existsSearch) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return conn.InsertIfNotExists(entity, existsSearch);
            }
        }
        #endregion

        #region 改
        public async Task<int> UpdateAsync<T>(T entity) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return await conn.UpdateAsync(entity);
            }
        }

        public int Update<T>(T entity) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return conn.Update(entity);
            }
        }

        public async Task<int> UpdateAsync<T>(List<T> entityList) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return await conn.BatchUpdateAsync(entityList);
            }
        }

        public int Update<T>(List<T> entityList) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return conn.BatchUpdate(entityList);
            }
        }

        public async Task<int> UpdateIfNotExistsAsync<T>(T entity, MySearchUtil existsSearch) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return await conn.UpdateIfNotExistsAsync(entity, existsSearch);
            }
        }

        public int UpdateIfNotExists<T>(T entity, MySearchUtil existsSearch) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return conn.UpdateIfNotExists(entity, existsSearch);
            }
        }

        public async Task<int> UpdateIncludeAsync<T>(T entity, IEnumerable<string> properties) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return await conn.UpdateIncludeAsync(entity, properties);
            }
        }

        public int UpdateInclude<T>(T entity, IEnumerable<string> properties) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return conn.UpdateInclude(entity, properties);
            }
        }

        public async Task<int> UpdateExcludeAsync<T>(T entity, IEnumerable<string> properties) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return await conn.UpdateExcludeAsync(entity, properties);
            }
        }

        public int UpdateExclude<T>(T entity, IEnumerable<string> properties) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return conn.UpdateExclude(entity, properties);
            }
        }
        #endregion

        #region 修改部分字段
        public async Task<int> SetAsync<T>(T entity, SOKeyValuePaires kvs) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return await conn.SetAsync(entity, kvs);
            }
        }

        public int Set<T>(T entity, SOKeyValuePaires kvs) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return conn.Set(entity, kvs);
            }
        }

        public async Task<int> SetAsync<T>(MySearchUtil search, SOKeyValuePaires kvs) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return await conn.BatchSetAsync<T>(kvs, search);
            }
        }

        public int Set<T>(SOKeyValuePaires kvs, MySearchUtil search) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return conn.BatchSet<T>(kvs, search);
            }
        }

        public async Task<int> SetAsync(string table, SOKeyValuePaires kvs, MySearchUtil search)
        {
            using (var conn = GetConnection())
            {
                return await conn.SetAsync(table, kvs, search);
            }
        }

        public int Set(string table, SOKeyValuePaires kvs, MySearchUtil search)
        {
            using (var conn = GetConnection())
            {
                return conn.Set(table, kvs, search);
            }
        }
        #endregion

        #region 加载实体
        public async Task<T> LoadAsync<T>(int id) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return await conn.LoadAsync<T>(id);
            }
        }

        public T Load<T>(int id) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return conn.Load<T>(id);
            }
        }

        public async Task<T> LoadAsync<T>(MySearchUtil search) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return await conn.LoadBySearchUtilAsync<T>(search);
            }
        }

        public T Load<T>(MySearchUtil search) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return conn.LoadBySearchUtil<T>(search);
            }
        }

        public async Task<T> LoadAsync<T>(string table, MySearchUtil search, string fields = "*")
        {
            using (var conn = GetConnection())
            {
                return await conn.LoadAsync<T>(table, search, fields);
            }
        }

        public T Load<T>(string table, MySearchUtil search, string fields = "*")
        {
            using (var conn = GetConnection())
            {
                return conn.Load<T>(table, search, fields);
            }
        }
        #endregion

        #region 列表
        public async Task<List<T>> FetchAsync<T>(MySearchUtil search) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                var data = await conn.FetchAsync<T>(search);
                return data.ToList();
            }
        }

        public List<T> Fetch<T>(MySearchUtil search) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                var data = conn.Fetch<T>(search);
                return data.ToList();
            }
        }

        public async Task<List<T>> FetchAsync<T>(string table, string fields, MySearchUtil search)
        {
            using (var conn = GetConnection())
            {
                var data = await conn.FetchAsync<T>(table, fields, search);
                return data.ToList();
            }
        }

        public List<T> Fetch<T>(string table, string fields, MySearchUtil search)
        {
            using (var conn = GetConnection())
            {
                var data = conn.Fetch<T>(table, fields, search);
                return data.ToList();
            }
        }
        #endregion

        #region 分页
        public async Task<PageList<T>> PageListAsync<T>(MySearchUtil search, int pageIndex, int pageSize) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return await conn.PageListAsync<T>(search, pageIndex, pageSize);
            }
        }

        public PageList<T> PageList<T>(MySearchUtil search, int pageIndex, int pageSize) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return conn.PageList<T>(search, pageIndex, pageSize);
            }
        }

        public async Task<PageList<T>> PageListAsync<T>(string table, string fields, MySearchUtil search, int pageIndex, int pageSize)
        {
            using (var conn = GetConnection())
            {
                return await conn.PageListAsync<T>(table, fields, search, pageIndex, pageSize);
            }
        }

        public PageList<T> PageList<T>(string table, string fields, MySearchUtil search, int pageIndex, int pageSize)
        {
            using (var conn = GetConnection())
            {
                return conn.PageList<T>(table, fields, search, pageIndex, pageSize);
            }
        }
        #endregion

        #region 删除
        public async Task<int> DeleteAsync<T>(int id) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return await conn.DeleteAsync<T>(id);
            }
        }

        public int Delete<T>(int id) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return conn.Delete<T>(id);
            }
        }

        public async Task<int> DeleteAsync<T>(IEnumerable<int> ids) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return await conn.DeleteAsync<T>(ids);
            }
        }

        public int Delete<T>(IEnumerable<int> ids) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return conn.Delete<T>(ids);
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

        public async Task<int> DeleteAsync(string table, MySearchUtil search)
        {
            using (var conn = GetConnection())
            {
                return await conn.DeleteAsync(table, search);
            }
        }

        public int Delete(string table, MySearchUtil search)
        {
            using (var conn = GetConnection())
            {
                return conn.Delete(table, search);
            }
        }

        public async Task<int> RemoveAsync(string table, MySearchUtil search)
        {
            using (var conn = GetConnection())
            {
                return await conn.RemoveAsync(table, search);
            }
        }

        public int Remove(string table, MySearchUtil search)
        {
            using (var conn = GetConnection())
            {
                return conn.Remove(table, search);
            }
        }

        public async Task<int> DeleteAsync<T>(MySearchUtil search) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return await conn.DeleteBySearchAsync<T>(search);
            }
        }

        public int Delete<T>(MySearchUtil search) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return conn.DeleteBySearch<T>(search);
            }
        }
        #endregion

        #region 数量
        public async Task<int> CountAsync<T>(MySearchUtil search) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return await conn.CountAsync<T>(search);
            }
        }

        public int Count<T>(MySearchUtil search) where T : BaseEntity
        {
            using (var conn = GetConnection())
            {
                return conn.Count<T>(search);
            }
        }

        public async Task<int> CountAsync(string table, MySearchUtil search)
        {
            using (var conn = GetConnection())
            {
                return await conn.CountAsync(table, search);
            }
        }

        public int Count(string table, MySearchUtil search)
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
        public bool ExecuteTran(SOKeyValuePaires kvsKeyValuePairs)
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
