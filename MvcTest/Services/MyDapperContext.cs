using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HZC.Framework.Authorizations;
using HZC.MyDapper;

namespace MvcTest.Services
{
    public class MyDapperContext : BaseDapperContext<int>
    {
        public MyDapperContext(DapperContextOption<int> options) : base(options)
        {
        }
    }
}
