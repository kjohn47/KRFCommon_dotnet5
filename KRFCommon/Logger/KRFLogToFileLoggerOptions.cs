namespace KRFCommon.Logger
{
    using Microsoft.Extensions.Logging;

    public class KRFLogToFileLoggerOptions
    {
        public string Path { get; set; }

        public LogLevel[] FilterLogLevelArray { get; set; }

        public bool? DisableApiLogs { get; set; }

        public bool? DisableSQLLogs { get; set; }
    }
}
