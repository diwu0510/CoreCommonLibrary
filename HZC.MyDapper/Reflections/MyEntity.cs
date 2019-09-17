using HZC.MyDapper.Conditions;
using HZC.MyDapper.SqlBuilders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HZC.MyDapper
{
    public class MyEntity
    {
        // 基础Insert语句，
        // INSERT INTO [表名] (字段名...) VALUES (@属性名)
        private readonly string _insertSql;

        private readonly string _updateSql;

        private readonly string _deleteSql;

        // [表名].[字段名] AS [属性名]
        private readonly string _fullColumns;

        #region 属性
        public Type Type { get; set; }

        /// <summary>
        /// 实体名称
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// 对应的数据表名称
        /// </summary>
        public string DataTableName { get; set; }

        /// <summary>
        /// 主键名称
        /// </summary>
        public string KeyPropertyName { get; set; }

        /// <summary>
        /// 主键对应的列名
        /// </summary>
        public string KeyColumnName { get; set; }

        /// <summary>
        /// 属性描述列表
        /// </summary>
        public List<MyProperty> Properties { get; set; }

        /// <summary>
        /// 是否创建审计
        /// </summary>
        public bool IsCreateAudited { get; set; }

        /// <summary>
        /// 是否更新审计
        /// </summary>
        public bool IsModificationAudited { get; set; }

        public bool IsDeleteAudit { get; set; }

        /// <summary>
        /// 是否控制行版本
        /// </summary>
        public bool IsVersion { get; set; }

        /// <summary>
        /// 是否软删除
        /// </summary>
        public bool IsSoftDelete { get; set; }
        #endregion

        public MyEntity(Type type)
        {
            Type = type;
            EntityName = type.Name;

            var tableAttr = type.GetCustomAttributes(typeof(DataTableAttribute), false);
            if(tableAttr.Length > 0)
            {
                var tableName = ((DataTableAttribute)tableAttr[0]).Table;
                DataTableName = string.IsNullOrWhiteSpace(tableName) ? type.Name.Replace("Entity", "") : tableName;
            }
            else
            {
                DataTableName = EntityName;
            }

            if(type.GetInterface("ISoftDelete`1") != null)
            {
                IsSoftDelete = true;
            }
            if (type.GetInterface("ICreateAudit`1") != null)
            {
                IsCreateAudited = true;
            }
            if (type.GetInterface("IUpdateAudit`1") != null)
            {
                IsModificationAudited = true;
            }
            if (type.GetInterface("IDeleteAudit`1") != null)
            {
                IsDeleteAudit = true;
            }

            Properties = new List<MyProperty>();
            foreach(var prop in type.GetProperties())
            {
                Properties.Add(new MyProperty(prop));
            }

            var keyProperties = Properties.Where(p => p.IsPrimary).ToArray();
            if (keyProperties == null || keyProperties.Count() > 1)
            {
                throw new Exception($"实体[{EntityName}]必须有且只有一个主键");
            }

            var keyProperty = keyProperties.First();
            KeyPropertyName = keyProperty.PropertyName;
            KeyColumnName = keyProperty.DataTableColumn;

            _insertSql = SqlServerBuilder.GetInsertSql(this);
            _updateSql = SqlServerBuilder.GetUpdateSql(this);
            _fullColumns = string.Join(",", Properties.Where(p => p.IsMap)
                .Select(p => $"[{p.DataTableColumn}] AS {p.PropertyName}"));
            _deleteSql = IsSoftDelete ? $"UPDATE [{DataTableName}] SET IsDel=1" : $"DELETE [{DataTableName}]";
        }

        public string GetInsertSql()
        {
            return _insertSql;
        }

        public string GetInsertSqlWithIdentity()
        {
            return $"{_insertSql};SELECT SCOPE_IDENTITY();";
        }

        public string GetInsertIfNotExistsSql(ConditionBuilder search)
        {
            return $"IF NOT EXISTS (SELECT Id FROM [{DataTableName}] WHERE {search.ToCondition()}) {_insertSql}";
        }

        public string GetInsertIfNotExistsWithIdentitySql(ConditionBuilder search)
        {
            return $"IF NOT EXISTS (SELECT Id FROM [{DataTableName}] WHERE {search.ToCondition()}) {_insertSql};SELECT SCOPE_IDENTITY();";
        }

        public string GetDefaultUpdateSql()
        {
            return $"{_updateSql} WHERE Id=@Id";
        }

        public string GetDefaultUpdateIfNotExistsSql(ConditionBuilder existsSearch)
        {
            return $"IF NOT EXISTS (SELECT Id FROM [{DataTableName}] WHERE {existsSearch.ToCondition()}) {_updateSql} WHERE Id=@Id";
        }

        public string GetUpdateSql(ConditionBuilder searchUtil)
        {
            return $"{_updateSql} WHERE {searchUtil.ToCondition()}";
        }

        public string GetUpdateIfNotExistsSql(ConditionBuilder existsSearch, ConditionBuilder searchUtil)
        {
            return $"IF NOT EXISTS (SELECT Id FROM [{DataTableName}] WHERE {existsSearch.ToCondition()}) {_updateSql} WHERE {searchUtil.ToCondition()}";
        }

        public string GetDefaultDeleteSql()
        {
            if(IsSoftDelete)
            {
                return $"UPDATE [{DataTableName}] SET IsDel=1 WHERE Id=@Id";
            }
            else
            {
                return $"DELETE [{DataTableName}] WHERE Id=@Id";
            }
        }

        public string GetDeleteSql(ConditionBuilder util)
        {
            return $"{_deleteSql} WHERE {util.ToCondition()}";
        }

        public string GetFullSelectColumns()
        {
            return _fullColumns;
        }
    }
}
