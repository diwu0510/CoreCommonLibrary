using System;

namespace HZC.Database.Abstract.Entities
{
    public interface IUpdateAudit
    {
        int UpdateBy { get; set; }

        DateTime UpdateAt { get; set; }
    }
}
