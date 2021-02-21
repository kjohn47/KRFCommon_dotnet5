namespace KRFCommon.Proxy
{
    using System.Net;
    using System.Net.Http.Headers;

    using KRFCommon.CQRS.Common;

    public class KRFHttpResponse<TResponse>
    {
        public TResponse Response { get; set; }
        public ErrorOut Error { get; set; }
        public HttpStatusCode HttpStatus { get; set; }
        public HttpResponseHeaders ResponseHeaders { get; set; }

        public bool HasError => this.Error != null;
    }
}
