namespace KRFCommon.Database
{
    public class QueryCommand
    {
        public QueryResult Result { get; set; }
        public string ResultDescription { get; set; }
    }

    public enum QueryResult
    {
        Success,
        Error
    }
}
