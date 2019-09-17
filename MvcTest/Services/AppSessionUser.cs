using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HZC.Framework.Authorizations;

namespace MvcTest.Services
{
    public class AppSessionUser : ISessionUser<int>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
