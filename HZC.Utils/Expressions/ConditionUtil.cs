using System;
using System.Linq.Expressions;

namespace HZC.Utils.Expressions
{
    /// <summary>
    /// 表达式目录树条件的帮助类
    /// </summary>
    public class ConditionUtil
    {
        public static Expression<Func<T, bool>> True<T>()
        {
            return obj => true;
        }

        public static Expression<Func<T, bool>> False<T>()
        {
            return obj => false;
        }
    }
}
