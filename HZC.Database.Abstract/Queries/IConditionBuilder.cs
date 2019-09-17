namespace HZC.Database.Abstract.Queries
{
    public interface IConditionBuilder<out TConditionResult>
    {
        string TableName { get; set; }

        TConditionResult InvokeWhere();
    }
}
