namespace KRFCommon.Logger
{
    public class KRFLogConfiguration
    {
        public KRFLogDestinationEnum[] KRFLogDestination { get; set; }
        public KRFLogToFileLoggerOptions KRFFileLogger { get; set; }
        public string KRFServerLogger { get; set; }
    }
}
