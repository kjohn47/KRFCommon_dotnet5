using KRFCommon.CQRS.Common;

namespace KRFCommon.CQRS.Query
{
    public interface IQueryOut<out Toutput> where Toutput : class
    {
        Toutput Result { get; }
        ErrorOut Error { get; }
        bool HasError { get; }
    }
}
