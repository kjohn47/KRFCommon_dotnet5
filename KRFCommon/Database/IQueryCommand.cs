namespace KRFCommon.Database
{
    public interface IQueryCommand
    {
        QueryResultEnum Result { get; set; }
        string ResultDescription { get; set; }
    }
}
