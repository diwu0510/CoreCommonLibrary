using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZC.DbUtil
{
    public interface ISearchUtil
    {
        string ToWhere();

        DynamicParameters ToDynamicParameters(bool isPaging = false);

        string ToOrderBy();
    }
}
