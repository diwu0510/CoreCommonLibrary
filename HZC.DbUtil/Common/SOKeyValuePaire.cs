using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace HZC.DbUtil
{
    /// <summary>
    /// KEY为string，Value为OBJECT的键值组合
    /// </summary>
    public class SOKeyValuePaires : List<KeyValuePair<string, object>>
    {
        public static SOKeyValuePaires New()
        {
            return new SOKeyValuePaires();
        }

        public SOKeyValuePaires Add(string key, object value)
        {
            Add(new KeyValuePair<string, object>(key, value));
            return this;
        }

        public string ToUpdateSetSql()
        {
            return string.Join(",", this.Select(kv => $"{kv.Key}=@{kv.Key}"));
        }

        public DynamicParameters ToDynamicParameters()
        {
            var param = new DynamicParameters();
            foreach(var kv in this)
            {
                param.Add($"@{kv.Key}", kv.Value);
            }
            return param;
        }
    }
}
