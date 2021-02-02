namespace KRFCommon.Database
{
    public class QueryCommand: IQueryCommand
    {
        public QueryResultEnum Result { get; set; }
        public string ResultDescription { get; set; }
    }
}
