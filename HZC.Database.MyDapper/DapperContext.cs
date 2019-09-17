using System;
using System.Collections.Generic;
using System.Text;
using HZC.Database.Abstract.Entities;
using HZC.Framework.Authorizations;
using HZC.Framework.Datas;
using ISoftDelete = HZC.Database.Abstract.Entities.ISoftDelete;
using IUpdateAudit = HZC.Database.Abstract.Entities.IUpdateAudit;

namespace HZC.Database.MyDapper
{
    public class DapperContext<TPrimaryKey>
    {
        private string _connectionString;

        private readonly ISessionUserManager<ISessionUser<TPrimaryKey>, TPrimaryKey> _sessionUserManager;

        public DapperContext(string connectionString, ISessionUserManager<ISessionUser<TPrimaryKey>, TPrimaryKey> sessionUserManager)
        {
            _connectionString = connectionString;
            _sessionUserManager = sessionUserManager;
        }

        public int Create<TEntity>(TEntity entity)
        {
            if (typeof(ICreateAudit).IsAssignableFrom(typeof(TEntity)))
            {
                ((ICreateAudit<TPrimaryKey>) entity).CreateBy = _sessionUserManager.GetCurrentUser().Id;
                ((ICreateAudit<TPrimaryKey>) entity).CreateAt = DateTime.Now;
            }

            if (typeof(IUpdateAudit).IsAssignableFrom(typeof(TEntity)))
            {
                ((IUpdateAudit<TPrimaryKey>)entity).UpdateBy = _sessionUserManager.GetCurrentUser().Id;
                ((IUpdateAudit<TPrimaryKey>) entity).UpdateAt = DateTime.Now;
            }

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                ((ISoftDelete) entity).IsDel = false;
            }

            return 0;
        }
    }
}
