namespace KRFCommon.Proxy
{
    public class KRFServiceSettings
    {
        public string ServerUrl { get; set; }
        public string CertificatePath { get; set; }
        public string CertificateKey { get; set; }
        public string TokenIdentifier { get; set; }
        public string TokenKey { get; set; }
        public int? Timeout { get; set; }
        public bool? ForceDisableSSL { get; set; }
    }
}
