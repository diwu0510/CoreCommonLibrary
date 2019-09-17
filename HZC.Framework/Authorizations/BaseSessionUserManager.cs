using System;
using System.Collections.Generic;
using System.Text;

namespace HZC.Framework.Authorizations
{
    public abstract class BaseSessionUserManager<TPrimaryKey>
    {
        public ISessionUser<int> GetCurrentUser()
        {
            throw new NotImplementedException();
        }
    }
}
