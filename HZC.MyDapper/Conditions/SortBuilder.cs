using System.Collections.Generic;

namespace HZC.MyDapper.Conditions
{
    public class SortBuilder
    {
        private readonly List<string> _sorts = new List<string>();

        public static SortBuilder New()
        {
            return new SortBuilder();
        }

        public SortBuilder OrderBy(string field)
        {
            _sorts.Add($"{field}");
            return this;
        }

        public SortBuilder OrderByDesc(string field)
        {
            _sorts.Add($"{field} DESC");
            return this;
        }

        public string ToOrderBy()
        {
            return _sorts.Count == 0 ? "Id" : string.Join(",", _sorts);
        }
    }
}
