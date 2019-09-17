using System;

namespace HZC.MyDapper
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
