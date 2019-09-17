namespace HZC.Framework.Authorizations
{
    /// <summary>
    /// 已登录用户
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public interface ISessionUser<TPrimaryKey>
    {
        TPrimaryKey Id { get; set; }

        string Name { get; set; }
    }
}
