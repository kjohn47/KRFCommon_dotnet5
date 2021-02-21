namespace KRFCommon.Proxy
{
    public class KRFHttpRequestWithBody<TBody> : KRFHttpRequest
        where TBody : class
    {
        public TBody Body { get; set; }
    }
}
