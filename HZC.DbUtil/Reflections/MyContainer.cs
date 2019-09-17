using System;
using System.Collections.Concurrent;

namespace HZC.DbUtil
{
    public class MyContainer
    {
        /// <summary>
        /// 实体及实体信息的字典
        /// </summary>
        private static ConcurrentDictionary<string, MyEntity> _dict = new ConcurrentDictionary<string, MyEntity>();

        #region 公共方法
        public static MyEntity Get(Type type)
        {
            if (!_dict.TryGetValue(type.Name, out var result))
            {
                result = new MyEntity(type);
                _dict.TryAdd(type.Name, result);
            }
            return result;
        }
        #endregion
    }
}
