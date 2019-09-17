using System;

namespace HZC.Database.Abstract.Entities
{
    public interface ICreateAudit
    {
        int CreateBy { get; set; }

        DateTime CreateAt { get; set; }
    }
}
