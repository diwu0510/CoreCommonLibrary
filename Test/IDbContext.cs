using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Dapper;
using HZC.MyDapper;

namespace Test
{
    public interface IDbContext<in TPrimaryKey>
    {
        int Insert<T>(T entity);

        int Insert<T>(IEnumerable<T> entities);

        int Update<T>(T entity);

        int Update<T>(IEnumerable<T> entities);

        int UpdateInclude<T>(T entity, IEnumerable<string> fields);

        int UpdateExclude<T>(T entity, IEnumerable<string> fields);

        int Set<T>(ConditionBuilder condition, FieldValuePairs fieldValuePairs);

        int Delete<T>(TPrimaryKey id);

        int Delete<T>(ConditionBuilder condition);

        T Load<T>(TPrimaryKey id);

        T FirstOrDefault<T>(ConditionBuilder condition);

        T FirstOrDefault<T>(ConditionBuilder condition, SortBuilder sort);

        IEnumerable<T> Fetch<T>();

        IEnumerable<T> Fetch<T>(ConditionBuilder condition);

        IEnumerable<T> Fetch<T>(SortBuilder sort);

        IEnumerable<T> Fetch<T>(ConditionBuilder condition, SortBuilder sort);

        IEnumerable<T> Query<T>(SortBuilder sort, int pageIndex, int pageSize);

        IEnumerable<T> Query<T>(ConditionBuilder condition, SortBuilder sort, int pageIndex, int pageSize);

        IEnumerable<T> QueryWithTotalCount<T>(SortBuilder sort, int pageIndex, int pageSize, out int total);

        IEnumerable<T> QueryWithTotalCount<T>(ConditionBuilder condition, SortBuilder sort, int pageIndex, int pageSize, out int total);

        IEnumerable<T> Fetch<T, TInclude1>(Func<T, TInclude1, T> func, ConditionBuilder condition, SortBuilder sort);

        IEnumerable<T> Fetch<T, TInclude1, TInclude2>(Func<T, TInclude1, TInclude2, T> func, ConditionBuilder condition, SortBuilder sort);

        IEnumerable<T> Fetch<T, TInclude1, TInclude2, TInclude3>(Func<T, TInclude1, TInclude2, TInclude3, T> func,
            ConditionBuilder condition,
            SortBuilder sort);

        IEnumerable<T> Fetch<T, TInclude1, TInclude2, TInclude3, TInclude4>(
            Func<T, TInclude1, TInclude2, TInclude3, TInclude4, T> func,
            ConditionBuilder condition,
            SortBuilder sort);

        IEnumerable<T> Query<T, TInclude1>(Func<T, TInclude1, T> func,
            int pageIndex,
            int pageSize,
            ConditionBuilder condition,
            SortBuilder sort);

        IEnumerable<T> Query<T, TInclude1, TInclude2>(Func<T, TInclude1, TInclude2, T> func,
            int pageIndex,
            int pageSize,
            ConditionBuilder condition,
            SortBuilder sort);

        IEnumerable<T> Query<T, TInclude1, TInclude2, TInclude3>(Func<T, TInclude1, TInclude2, TInclude3, T> func,
            int pageIndex,
            int pageSize,
            ConditionBuilder condition,
            SortBuilder sort);

        IEnumerable<T> Query<T, TInclude1, TInclude2, TInclude3, TInclude4>(
            Func<T, TInclude1, TInclude2, TInclude3, TInclude4, T> func,
            int pageIndex,
            int pageSize,
            ConditionBuilder condition,
            SortBuilder sort);

        IEnumerable<T> QueryWithTotalCount<T, TInclude1>(Func<T, TInclude1, T> func,
            int pageIndex,
            int pageSize,
            ConditionBuilder condition,
            SortBuilder sort,
            out int total);

        IEnumerable<T> QueryWithTotalCount<T, TInclude1, TInclude2>(Func<T, TInclude1, TInclude2, T> func,
            int pageIndex,
            int pageSize,
            ConditionBuilder condition,
            SortBuilder sort,
            out int total);

        IEnumerable<T> QueryWithTotalCount<T, TInclude1, TInclude2, TInclude3>(
            Func<T, TInclude1, TInclude2, TInclude3, T> func,
            int pageIndex,
            int pageSize,
            ConditionBuilder condition,
            SortBuilder sort,
            out int total);

        IEnumerable<T> QueryWithTotalCount<T, TInclude1, TInclude2, TInclude3, TInclude4>(
            Func<T, TInclude1, TInclude2, TInclude3, TInclude4, T> func,
            int pageIndex,
            int pageSize,
            ConditionBuilder condition,
            SortBuilder sort,
            out int total);
    }

    public class ConditionBuilder
    {
        public static ConditionBuilder New()
        {
            return new ConditionBuilder();
        }

        public static ConditionBuilder New(object id)
        {
            var builder = new ConditionBuilder();

            // TODO: 添加条件Id=@Id

            return builder;
        }

        public string ToCondition()
        {
            return string.Empty;
        }
    }

    public class SortBuilder
    {
        public string Test()
        {
            return @"
SELECT 
    Students.*,
    Clazzes.* 
FROM 
    Students LEFT JOIN Clazzes ON Students.ClazzId=Clazzes.Id 
WHERE ... 
ORDER BY ...
";
        }
    }

    public class SqlParameterPair
    {
        public string Sql { get; set; }

        public DynamicParameters Parameters { get; set; }
    }

    public class FieldValueEntry
    {
        public string Field { get; set; }

        public object Value { get; set; }

        public bool HasValue { get; set; }
    }

    public class FieldValuePairs : List<FieldValueEntry>
    {
        public FieldValuePairs Add(string field, object value)
        {
             Add(new FieldValueEntry { Field = field, Value = value, HasValue = true });
             return this;
        }

        public FieldValuePairs Add(string field)
        {
            Add(new FieldValueEntry {Field = field, Value = null, HasValue = false});
            return this;
        }

        public static FieldValuePairs New()
        {
            return new FieldValuePairs();
        }

        public SqlParameterPair Invoke(string prefix = "@")
        {
            if (!this.Any())
            {
                return null;
            }

            var clauses = new List<string>();
            var parameters = new DynamicParameters();
            foreach (var pair in this)
            {
                if (pair.HasValue)
                {
                    clauses.Add($"{pair.Field}={prefix}{pair.Field}");
                    parameters.Add(pair.Field, pair.Value);
                }
                clauses.Add(pair.Field);
            }

            return new SqlParameterPair {Sql = string.Join(",", clauses), Parameters = parameters};
        }

        public class QueryAble<T>
        {
            private readonly MyEntity _masterEntity;
            private readonly List<IncludeEntity> _entityInfoList = new List<IncludeEntity>();

            public QueryAble()
            {
                _masterEntity = MyContainer.Get(typeof(T));
            }

            public QueryAble<T> Include<TInclude>(string propertyName)
            {
                var property = _masterEntity.Properties.SingleOrDefault(p => p.PropertyName == propertyName);
                if (property == null)
                {
                    throw new ArgumentNullException(propertyName, $"名为 {propertyName} 的属性不存在");
                }

                if (!property.IsNavProperty)
                {
                    throw new ArgumentNullException(propertyName, $" {propertyName} 非导航属性");
                }

                _entityInfoList.Add(new IncludeEntity
                    {PrefixTableName = propertyName, EntityInfo = MyContainer.Get(property.Type), JoinKey = property.ForeignKey});

                return this;
            }

            public QueryAble<T> Where(ConditionBuilder condition)
            {
                return this;
            }

            public QueryAble<T> OrderBy(SortBuilder sort)
            {
                return this;
            }

            public IEnumerable<T> ToList()
            {
                var sql = "";
                var parameters = "";

                return null;
            }

            class IncludeEntity
            {
                public string PrefixTableName { get; set; }

                public string JoinKey { get; set; }

                public MyEntity EntityInfo { get; set; }
            }
        }
    }

    public class SqlServerSqlBuilder
    {
        public static string GetInsertClause(MyEntity entity)
        {
            return entity.GetInsertSql();
        }

        public static string GetUpdateByIdClause(MyEntity entity)
        {
            return entity.GetDefaultUpdateSql()
        }
    }
}
