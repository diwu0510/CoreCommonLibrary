using System;

namespace HZC.Framework.Datas
{
    public interface IDeleteAudit<TPrimaryKey>
    {
        TPrimaryKey DeleteBy { get; set; }

        DateTime? DeleteAt { get; set; }
    }
}
