namespace KRFCommon.CQRS.Common
{
    public interface IResponseOut<out Toutput> where Toutput : ICQRSResponse
    {
        Toutput Result { get; }
        ErrorOut Error { get; }
    }
}
