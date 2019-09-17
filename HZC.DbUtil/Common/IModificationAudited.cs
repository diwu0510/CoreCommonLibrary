using System;

namespace HZC.DbUtil
{
    public interface IModificationAudited
    {
        DateTime UpdateAt { get; set; }

        string Updator { get; set; }
    }
}
