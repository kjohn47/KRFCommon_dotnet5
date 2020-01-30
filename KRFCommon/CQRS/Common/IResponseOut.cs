namespace KRFCommon.CQRS.Common
{
    public interface IResponseOut<out Toutput> where Toutput : class
    {
        Toutput Result { get; }
        ErrorOut Error { get; }
    }
}
