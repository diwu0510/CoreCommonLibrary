using System;

namespace HZC.Framework.Datas
{
    public interface IUpdateAudit<TPrimary>
    {
        TPrimary UpdateBy { get; set; }

        DateTime UpdateAt { get; set; }
    }
}
