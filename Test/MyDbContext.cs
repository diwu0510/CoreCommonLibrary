using System;
using System.Collections.Generic;
using System.Text;
using HZC.Framework.Authorizations;
using HZC.MyDapper;

namespace Test
{
    public class MyDbContext : BaseDapperContext<int>
    {
        public MyDbContext(string connectionString, ISessionUserManager<ISessionUser<int>, int> userManager) : base(connectionString, userManager)
        {
        }
    }
}
