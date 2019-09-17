using System;

namespace HZC.DbUtil
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MyKeyAttribute : Attribute
    {
        public string DataTableColumn { get; set; } = "Id";

        public bool IsKey { get; set; } = true;

        public bool IsAutoIncrement { get; set; } = true;

        public MyKeyAttribute()
        {  }

        public MyKeyAttribute(string columnName)
        {
            DataTableColumn = columnName;
        }
    }
}
