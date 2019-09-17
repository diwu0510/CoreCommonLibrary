using System;

namespace HZC.DbUtil
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MyForeignKeyAttribute : Attribute
    {
        public string ForeignKey { get; set; }

        public MyForeignKeyAttribute(string foreignKey)
        {
            ForeignKey = foreignKey;
        }
    }
}
