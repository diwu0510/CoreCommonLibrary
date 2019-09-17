using System;

namespace HZC.DbUtil.Common
{
    public class MyDbUtilConfiguration
    {
        private static string _defaultConnectionString;

        private static bool _useSqlServer2008 = true;

        public static void Init(string connectionString, bool useSqlServer2008 = true)
        {
            _defaultConnectionString = connectionString;
            _useSqlServer2008 = useSqlServer2008;
        }

        public static string GetDefaultConnectionString()
        {
            if(string.IsNullOrWhiteSpace(_defaultConnectionString))
            {
                throw new Exception("DbConfiguration尚未初始化");
            }

            return _defaultConnectionString;
        }

        public static bool UseSqlServer2008()
        {
            return _useSqlServer2008;
        }
    }
}
