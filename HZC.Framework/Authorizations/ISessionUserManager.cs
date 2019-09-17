namespace HZC.Framework.Authorizations
{
    public interface ISessionUserManager<TSessionUser, TPrimaryKey> 
        where TSessionUser : ISessionUser<TPrimaryKey>
    {
        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <returns></returns>
        ISessionUser<TPrimaryKey> GetCurrentUser();
    }
}
