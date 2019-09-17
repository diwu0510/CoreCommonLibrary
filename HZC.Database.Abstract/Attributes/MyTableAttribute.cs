using System;

namespace HZC.Database.Abstract.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MyTableAttribute : Attribute
    {
        public string TableName { get; set; }

        public MyTableAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
