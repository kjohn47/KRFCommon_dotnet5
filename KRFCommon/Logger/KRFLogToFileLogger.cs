namespace KRFCommon.Logger
{
    using System;
    using System.IO;
    using System.Linq;

    using KRFCommon.Constants;

    using Microsoft.Extensions.Logging;

    public class KRFLogToFileLogger : ILogger
    {
        protected readonly KRFLogToFileLoggerProvider _KRFLogToFileLoggerProvider;
        private Object fileWriterLock = new Object();

        public KRFLogToFileLogger( KRFLogToFileLoggerProvider krfLogToFileLoggerProvider )
        {
            this._KRFLogToFileLoggerProvider = krfLogToFileLoggerProvider;
        }

        public IDisposable BeginScope<TState>( TState state )
        {
            return null;
        }

        public bool IsEnabled( LogLevel logLevel )
        {
            return logLevel != LogLevel.None && logLevel != LogLevel.Debug;
        }

        public void Log<TState>( LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter )
        {
            if ( !IsEnabled( logLevel ) )
            {
                return;
            }

            string logPath;
            var isApiLog = false;

            //SQL Query Log
            if ( eventId.Id == KRFConstants.EntityFrameworkEventId )
            {
                if( this._KRFLogToFileLoggerProvider.Options != null && 
                    this._KRFLogToFileLoggerProvider.Options.DisableSQLLogs.HasValue && 
                    this._KRFLogToFileLoggerProvider.Options.DisableSQLLogs.Value )
                {
                    return;
                }
                
                logPath = KRFConstants.SQLLogFolder;
            }
            //Log Api events
            else if ( eventId.Id == KRFConstants.ApiEventId )
            {
                if ( this._KRFLogToFileLoggerProvider.Options != null &&
                    this._KRFLogToFileLoggerProvider.Options.DisableApiLogs.HasValue &&
                    this._KRFLogToFileLoggerProvider.Options.DisableApiLogs.Value )
                {
                    return;
                }

                logPath = KRFConstants.APILogFolder;
                isApiLog = true;
            }
            else
            {
                if ( this._KRFLogToFileLoggerProvider.Options == null || 
                    this._KRFLogToFileLoggerProvider.Options.FilterLogLevelArray == null ||
                    !this._KRFLogToFileLoggerProvider.Options.FilterLogLevelArray.Any( l => l == logLevel ) )
                {
                    return;
                }

                //Create path for current date log and logLevel
                logPath = logLevel.ToString();
            }
            var dateStamp = DateTimeOffset.UtcNow.ToString( KRFConstants.DateWithoutSeparatorFormat );
            var fileName = isApiLog ? string.Format( "{0}_{1}", eventId.Name, dateStamp ) : dateStamp;

            var fullFilePath = string.Format( "{0}\\{1}\\{2}.log", this._KRFLogToFileLoggerProvider.BasePath, logPath, fileName );

            //Create line to be recorded on log
            var logRecord = string.Format( "[{0}] {1} {2}", DateTimeOffset.UtcNow.ToString( KRFConstants.TimeStampFormat ), formatter( state, exception ), exception != null ? exception.StackTrace : "" );
            lock (fileWriterLock)
            {
                try
                {
                    //Try add new record
                    using (var streamWriter = new StreamWriter(fullFilePath, true))
                    {
                        streamWriter.WriteLine(logRecord);
                    }
                }
                catch
                {
                    try
                    {
                        //Failed, try to record on auxiliar file
                        using (var streamWriter = new StreamWriter(string.Format("{0}.aux.txt", fullFilePath), true))
                        {
                            streamWriter.WriteLine(logRecord);
                        }
                    }
                    catch
                    {
                        //Failed again? just ignore
                        return;
                    }
                    return;
                }
            }
        }
    }
}
