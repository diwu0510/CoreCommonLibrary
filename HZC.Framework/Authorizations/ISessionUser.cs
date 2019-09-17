namespace HZC.Framework.Authorizations
{
    /// <summary>
    /// 默认，用户主键类型为int，角色主键类型为string的已登录用户
    /// </summary>
    public interface ISessionUser : ISessionUser<int>
    {
    }
}
