using System;
using System.Collections.Generic;
using System.Text;
using HZC.Database.Abstract.Entities;
using HZC.Database.Abstract.Queries;

namespace HZC.Database.Abstract
{
    public interface IRepository<T, TPrimaryKey> where T : IEntity<TPrimaryKey>
    {
        #region 增

        int Insert(T entity);

        int InsertAndReturnNewId(T entity);

        int Insert(IEnumerable<T> entityList);

        int BatchInsert(IEnumerable<T> entityList);

        #endregion

        #region 改

        int Update(T entity);

        int Update(IEnumerable<T> entityList);

        int UpdateInclude(T entity, IEnumerable<string> fields);

        int UpdateInclude(IEnumerable<T> entityList, IEnumerable<string> fields);

        int UpdateExclude(T entity, IEnumerable<string> fields);

        int UpdateExclude(IEnumerable<T> entityList, IEnumerable<string> fields);

        #endregion

        #region 编辑字段

        int Set(IConditionBuilder<IFilterResult> filter, Dictionary<string, object> kvs);

        #endregion

        #region 删除

        int Delete(TPrimaryKey id);

        int Delete(IConditionBuilder<IFilterResult> filter);

        #endregion

        #region 查

        

        #endregion
    }
}
