using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using HZC.Database.Abstract.Queries;

namespace HZC.Database.MyDapper
{
    public abstract class BaseSearchInput<T> : IConditionBuilder<T, MyCondition>, ISortBuilder<T, MyOrderBy>
    {
        public abstract void BuildWhere(MyCondition condition);

        public abstract void BuildOrderBy(MyOrderBy orderBy);

        public MyCondition InvokeWhere()
        {
            var condition = new MyCondition();
            BuildWhere(condition);
            return condition;
        }

        public MyOrderBy InvokeOrderBy()
        {
            var orderBy = new MyOrderBy();
            BuildOrderBy(orderBy);
            return orderBy;
        }  
    }
}
