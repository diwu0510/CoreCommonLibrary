using System;

namespace HZC.MyDapper
{
    public class DapperContextOption<TPrimaryKey>
    {
        public string ConnectionString { get; set; }

        public Func<TPrimaryKey> GetAuditorIdDelegate { get; set; }
    }
}
