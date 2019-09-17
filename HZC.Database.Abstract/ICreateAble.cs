using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using HZC.Database.Abstract.Entities;

namespace HZC.Database.Abstract
{
    public interface ICreateAble<in T, out TPrimaryKey> where T : IEntity<TPrimaryKey>
    {
        /// <summary>
        /// 保存到数据库，返回新的ID
        /// </summary>
        /// <returns></returns>
        TPrimaryKey SaveAndGetNewId();

        /// <summary>
        /// 保存到数据库，返回成功的条数。
        /// </summary>
        /// <returns></returns>
        int Save();

        /// <summary>
        /// 批量添加实体
        /// </summary>
        /// <param name="entityList"></param>
        /// <returns></returns>
        int Add(IEnumerable<T> entityList);

        /// <summary>
        /// 大批量插入实体
        /// </summary>
        /// <param name="entityList"></param>
        /// <returns></returns>
        int Batch(IEnumerable<T> entityList);
    }
}
