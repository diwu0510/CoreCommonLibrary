namespace HZC.Framework.Authorizations
{
    /// <summary>
    /// 系统用户
    /// </summary>
    /// <typeparam name="TPrimaryKey">主键类型</typeparam>
    public interface IAppUser<TPrimaryKey>
    {
        TPrimaryKey Id { get; set; }

        string Name { get; set; }
    }
}
