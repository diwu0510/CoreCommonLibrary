using System;

namespace HZC.DbUtil
{
    public interface ICreationAudited
    {
        DateTime CreateAt { get; set; }

        string Creator { get; set; }
    }
}
