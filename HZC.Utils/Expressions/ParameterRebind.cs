using System.Collections.Generic;
using System.Linq.Expressions;

namespace HZC.Utils.Expressions
{
    internal class ParameterRebind : ExpressionVisitor
    {
        /// <summary>
        /// 参数表达式的映射，前面是原始参数表达式，后面是要替换的参数表达式
        /// </summary>
        readonly Dictionary<ParameterExpression, ParameterExpression> _map;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="map">The map.</param>
        private ParameterRebind(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            _map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        /// <summary>
        /// 访问参数
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>Expression</returns>
        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (_map.TryGetValue(p, out var replacement))
            {
                p = replacement;
            }
            return base.VisitParameter(p);
        }

        /// <summary>
        /// 静态方法，替换参数
        /// </summary>
        /// <param name="map">参数映射</param>
        /// <param name="exp">要处理的表达式</param>
        /// <returns>Expression</returns>
        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebind(map).Visit(exp);
        }
        
    }
}
