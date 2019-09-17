using System;

namespace HZC.DbUtil
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DataTableAttribute : Attribute
    {
        public string Table { get; set; }

        public DataTableAttribute(string tableName)
        {
            Table = tableName;
        }
    }
}
