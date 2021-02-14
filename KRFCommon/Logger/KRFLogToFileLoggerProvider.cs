namespace KRFCommon.Logger
{
    using System;
    using System.IO;

    using KRFCommon.Constants;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    [ProviderAlias( "KRFLogToFile" )]
    public class KRFLogToFileLoggerProvider : ILoggerProvider
    {
        public readonly KRFLogToFileLoggerOptions Options;
        public readonly string BasePath;

        public KRFLogToFileLoggerProvider( IOptions<KRFLogToFileLoggerOptions> _options )
        {
            this.Options = _options.Value;

            if ( this.Options != null && !string.IsNullOrEmpty( this.Options.Path ) )
            {
                this.BasePath = Path.IsPathRooted( _options.Value.Path ) ? _options.Value.Path : string.Format( "{0}\\{1}", Environment.CurrentDirectory, _options.Value.Path );
            }
            else
            {
                this.BasePath = string.Format( "{0}\\{1}", Environment.CurrentDirectory, KRFConstants.DefaultLogsFolder );
            }

            if ( !Directory.Exists( this.BasePath ) )
            {
                Directory.CreateDirectory( this.BasePath );
            }

            var logLevelArray = Enum.GetValues( typeof( LogLevel ) );
            foreach ( var logLevel in logLevelArray )
            {
                if ( !( logLevel.ToString().Equals( LogLevel.None.ToString() ) || logLevel.ToString().Equals( LogLevel.Debug.ToString() ) ) )
                {
                    var logLevelPath = string.Format( "{0}\\{1}", this.BasePath, logLevel.ToString() );

                    if ( !Directory.Exists( logLevelPath ) )
                    {
                        Directory.CreateDirectory( logLevelPath );
                    }
                }
            }

            var sqlLogPath = string.Format( "{0}\\{1}", this.BasePath, KRFConstants.SQLLogFolder );
            if ( !Directory.Exists( sqlLogPath ) )
            {
                Directory.CreateDirectory( sqlLogPath );
            }

            var apiLogPath = string.Format( "{0}\\{1}", this.BasePath, KRFConstants.APILogFolder );
            if ( !Directory.Exists( apiLogPath ) )
            {
                Directory.CreateDirectory( apiLogPath );
            }
        }

        public ILogger CreateLogger( string categoryName )
        {
            return new KRFLogToFileLogger( this );
        }

        public void Dispose()
        {
        }
    }
}
