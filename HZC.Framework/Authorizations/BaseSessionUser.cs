using System;
using System.Collections.Generic;
using System.Text;

namespace HZC.Framework.Authorizations
{
    public class BaseSessionUser : ISessionUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
