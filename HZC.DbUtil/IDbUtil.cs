using System;
using System.Collections.Generic;
using System.Text;

namespace HZC.DbUtil
{
    public interface IDbUtil
    {
        #region 创建

        int Insert<T>(T entity) where T : BaseEntity;

        int InsertIfNotExists<T>(T entity, ISearchUtil search) where T : BaseEntity;

        int BatchInsert<T>(IEnumerable<T> entityList) where T : BaseEntity;

        #endregion

        #region 修改

        int Update<T>(T entity) where T : BaseEntity;

        int UpdateIfNotExists<T>(T entity, ISearchUtil search) where T : BaseEntity;

        int BatchUpdate<T>(IEnumerable<T> entityList, ISearchUtil search) where T : BaseEntity;

        #endregion

        #region 修改部分字段

        int SetInclude<T>(T entity, IEnumerable<string> propertyList) where T : BaseEntity;

        int SetExclude<T>(T entity, IEnumerable<string> propertyList) where T : BaseEntity;

        int BatchSet<T>(SOKeyValuePaires propertyValueMaps, ISearchUtil search);

        int BatchSet(string table, SOKeyValuePaires propertyValueMaps, ISearchUtil search);

        #endregion
    }
}
