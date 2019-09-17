namespace HZC.Database.Abstract.Queries
{
    public interface ISortBuilder<out TSortResult>
    {
        TSortResult InvokeOrderBy();
    }
}
