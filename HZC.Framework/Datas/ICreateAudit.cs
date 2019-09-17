using System;

namespace HZC.Framework.Datas
{
    public interface ICreateAudit<TPrimary>
    {
        TPrimary CreateBy { get; set; }

        DateTime CreateAt { get; set; }
    }
}
