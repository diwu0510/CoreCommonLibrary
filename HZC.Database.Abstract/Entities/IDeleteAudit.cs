using System;

namespace HZC.Database.Abstract.Entities
{
    public interface IDeleteAudit
    {
        int DeleteBy { get; set; }
        
        DateTime DeleteAt { get; set; }
    }
}
