using System;
using System.Reflection;

namespace HZC.DbUtil
{
    public class MyProperty
    {
        /// <summary>
        /// 属性类型
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// 属性名
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 对应的数据表列名
        /// </summary>
        public string DataTableColumn { get; set; }

        /// <summary>
        /// 是不是主键
        /// </summary>
        public bool IsPrimary { get; set; } = false;

        /// <summary>
        /// 是不是用户自定义类别
        /// </summary>
        public bool IsNavProperty { get; set; } = false;

        /// <summary>
        /// 外键
        /// </summary>
        public string ForeignKey { get; set; }

        /// <summary>
        /// 插入时是否忽略该字段
        /// </summary>
        public bool InsertIgnore { get; set; }

        /// <summary>
        /// 更新时是否忽略该字段
        /// </summary>
        public bool UpdateIgnore { get; set; }

        /// <summary>
        /// 是否映射
        /// </summary>
        public bool IsMap { get; set; } = true;

        /// <summary>
        /// 是否可赋值
        /// </summary>
        public bool IsAssignable { get; set; } = true;

        public MyProperty(PropertyInfo propertyInfo)
        {
            PropertyName = propertyInfo.Name;
            Type = propertyInfo.PropertyType;

            var keyAttribute = propertyInfo.GetCustomAttribute<MyKeyAttribute>();
            if(keyAttribute != null)
            {
                IsPrimary = true;
                IsAssignable = false;
                DataTableColumn = keyAttribute.DataTableColumn;
                if (keyAttribute.IsAutoIncrement)
                {
                    InsertIgnore = true;
                    UpdateIgnore = true;
                }
                else
                {
                    InsertIgnore = false;
                    UpdateIgnore = false;
                }
                return;
            }
            else
            {
                if (propertyInfo.Name == "Id")
                {
                    IsPrimary = true;
                    InsertIgnore = true;
                    UpdateIgnore = true;
                    DataTableColumn = "Id";
                    IsAssignable = false;

                    return;
                }
            }

            if(propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType.Name == "String")
            {
                var columnAttr = propertyInfo.GetCustomAttribute<DataColumnAttribute>();
                if(columnAttr == null)
                {
                    DataTableColumn = propertyInfo.Name;
                }
                else
                {
                    if(columnAttr.Ignore)
                    {
                        InsertIgnore = true;
                        UpdateIgnore = true;
                    }
                    else
                    {
                        InsertIgnore = columnAttr.InsertIgnore;
                        UpdateIgnore = columnAttr.UpdateIgnore;
                    }

                    DataTableColumn = string.IsNullOrWhiteSpace(columnAttr.Column) ? propertyInfo.Name : columnAttr.Column;

                    IsMap = columnAttr.IsMap;
                }

                if(propertyInfo.Name == "IsDel")
                {
                    UpdateIgnore = true;
                }

                if (propertyInfo.Name == "CreateAt" || propertyInfo.Name == "Creator")
                {
                    UpdateIgnore = true;
                }
            }
            else
            {
                if (propertyInfo.Name == "Version" && propertyInfo.PropertyType.Name == "Byte[]")
                {
                    IsMap = true;
                    InsertIgnore = true;
                    UpdateIgnore = true;
                    DataTableColumn = "Version";
                    IsAssignable = false;
                }
                else
                {
                    InsertIgnore = true;
                    UpdateIgnore = true;
                    IsMap = false;
                    IsAssignable = false;

                    if (propertyInfo.PropertyType.IsClass)
                    {
                        var foreignAttr = propertyInfo.GetCustomAttribute<MyForeignKeyAttribute>();
                        if (foreignAttr != null)
                        {
                            IsNavProperty = true;
                            ForeignKey = string.IsNullOrWhiteSpace(foreignAttr.ForeignKey) ? propertyInfo.Name + "Id" : foreignAttr.ForeignKey;
                        }
                        else
                        {
                            IsNavProperty = true;
                            ForeignKey = propertyInfo.Name + "Id";
                        }
                    }
                }
            }
        }
    }
}