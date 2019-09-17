using System;
using System.Collections.Generic;
using System.Text;

namespace HZC.Framework.Datas
{
    public class PageList<T>
    {
        public int RecordCount { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public IEnumerable<T> Body { get; set; }
    }
}
