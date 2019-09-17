using System;

namespace HZC.Database.Abstract.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class MyFieldAttribute : Attribute
    {
        /// <summary>
        /// 数据列名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 插入是忽略该列
        /// </summary>
        public bool InsertIgnore { get; set; } = false;

        /// <summary>
        /// 更新时忽略该列
        /// </summary>
        public bool UpdateIgnore { get; set; } = false;

        /// <summary>
        /// 是否映射
        /// </summary>
        public bool IsMap { get; set; } = true;
    }
}
