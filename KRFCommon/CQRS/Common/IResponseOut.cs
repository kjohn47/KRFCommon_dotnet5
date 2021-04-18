namespace KRFCommon.CQRS.Common
{
    using KRFCommon.Common;
    public interface IResponseOut<out Toutput> where Toutput : ICQRSResponse
    {
        Toutput Result { get; }
        NullableObject<ErrorOut> Error { get; }
    }
}
