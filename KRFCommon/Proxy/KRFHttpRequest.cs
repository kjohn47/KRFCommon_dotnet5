﻿namespace KRFCommon.Proxy
{
    public class KRFHttpRequest
    {
        public string Url { get; set; }
        public string Route { get; set; }
        public string QueryString { get; set; }
        public HttpMethodEnum Method { get; set; }
        public string KRFBearerToken { get; set; }
        public string KRFBearerTokenHeader { get; set; }
        public string CertificatePath { get; set; }
        public string CertificateKey { get; set; }
        public bool? ForceDisableSSL { get; set; }
        public int? Timeout { get; set; }
    }
}
