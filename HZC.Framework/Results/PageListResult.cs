﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HZC.Framework
{
    public class PageListResult<T> : Result
    {
        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public int RecordCount { get; set; }

        public IEnumerable<T> Body { get; set; }

        public PageListResult()
        { }

        public PageListResult(int code, IEnumerable<T> body, int pageIndex, int pageSize, int recordCount, string message = "")
        {
            Code = code;
            Body = body == null ? default(IEnumerable<T>) : body;
            PageSize = pageSize;
            PageIndex = pageIndex;
            RecordCount = recordCount;
            Message = message;
        }
    }
}
