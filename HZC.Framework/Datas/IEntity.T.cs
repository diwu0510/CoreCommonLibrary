namespace HZC.Framework.Datas
{
    public interface IEntity<TPrimaryKey>
    {
        TPrimaryKey Id { get; set; }
    }
}
