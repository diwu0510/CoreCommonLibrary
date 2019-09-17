using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HZC.Framework.Authorizations;

namespace MvcTest.Services
{
    public class AppSessionUserManager : ISessionUserManager<AppSessionUser, int>
    {
        public ISessionUser<int> GetCurrentUser()
        {
            return new AppSessionUser
            {
                Id = 1,
                Name = "admin"
            };
        }
    }
}
