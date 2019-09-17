using Dapper;
using HZC.Framework;
using System.Linq;

namespace HZC.MyDapper.Extensions
{
    public static class SoKeyValuePairsExtensions
    {
        public static string ToUpdateSetSql(this SoKeyValuePairs kvs)
        {
            return string.Join(",", kvs.Select(kv => $"{kv.Key}=@{kv.Key}"));
        }

        public static DynamicParameters ToParameters(this SoKeyValuePairs kvs)
        {
            var param = new DynamicParameters();
            foreach (var (key, value) in kvs)
            {
                param.Add($"{key}", value);
            }
            return param;
        }
    }
}
