namespace KRFCommon.Logger
{
    using System;
    using System.Linq;

    using KRFCommon.Constants;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class KRFLogExtensions
    {
        public static ILoggingBuilder AddKRFLogger( this ILoggingBuilder builder, IConfiguration configuration )
        {
            if ( configuration == null || builder == null )
            {
                throw new Exception( "Missing configuration or log builder" );
            }

            KRFLogConfiguration settings = configuration.GetSection( KRFApiSettings.KRFLoggerConfig_Key ).Get<KRFLogConfiguration>() ?? null;

            return builder.AddKRFLogger( settings, configuration );
        }

        public static ILoggingBuilder AddKRFLogger( this ILoggingBuilder builder, KRFLogConfiguration settings, IConfiguration configuration )
        {
            if ( settings == null )
            {
                throw new Exception( "Configuration is missing KRFLogger settings" );
            }

            if ( settings.KRFLogDestination == null || settings.KRFLogDestination.Length == 0 )
            {
                throw new Exception( "Configuration is missing KRFLogDestination settings on KRFLogger" );
            }

            builder.ClearProviders();
            builder.AddConfiguration( configuration.GetSection( KRFApiSettings.Logging_Key ) );

            if ( settings.KRFLogDestination.Any( ld => ld.Equals( KRFLogDestinationEnum.ToConsole ) ) )
            {
                builder.AddConsole();
            }

            if ( settings.KRFLogDestination.Any( ld => ld.Equals( KRFLogDestinationEnum.ToDebug ) ) )
            {
                builder.AddDebug();
            }

            if ( settings.KRFLogDestination.Any( ld => ld.Equals( KRFLogDestinationEnum.ToEvents ) ) )
            {
                builder.AddEventLog();
            }

            if ( settings.KRFLogDestination.Any( ld => ld.Equals( KRFLogDestinationEnum.ToEventSource ) ) )
            {
                builder.AddEventSourceLogger();
            }

            if ( settings.KRFLogDestination.Any( ld => ld.Equals( KRFLogDestinationEnum.ToFile ) ) && settings.KRFFileLogger != null )
            {
                builder.AddKRFLogToFileLogger( o => { o = settings.KRFFileLogger; } );
            }

            if ( settings.KRFLogDestination.Any( ld => ld.Equals( KRFLogDestinationEnum.ToServer ) ) )
            {
                throw new Exception( "Logging to server not implemented" );
                //builder.AddKRFLogToServerLogger( loggerConfig.GetSection( KRFApiSettings.KRFServerLoggerConfig_Key ) );
            }

            return builder;
        }

        public static ILoggingBuilder AddKRFLogToFileLogger( this ILoggingBuilder builder )
        {
            builder.Services.AddSingleton<ILoggerProvider, KRFLogToFileLoggerProvider>();
            return builder;
        }

        public static ILoggingBuilder AddKRFLogToFileLogger( this ILoggingBuilder builder, Action<KRFLogToFileLoggerOptions> configure )
        {
            if ( configure == null )
            {
                throw new Exception( "No log configuration defined" );
            }

            builder.Services.AddSingleton<ILoggerProvider, KRFLogToFileLoggerProvider>();
            builder.Services.Configure( configure );
            return builder;
        }

        public static ILoggingBuilder AddKRFLogToFileLogger( this ILoggingBuilder builder, IConfiguration configuration )
        {
            builder.Services.AddSingleton<ILoggerProvider, KRFLogToFileLoggerProvider>();

            if ( configuration != null && configuration.Get<KRFLogToFileLoggerOptions>() != null )
            {
                builder.Services.Configure<KRFLogToFileLoggerOptions>( configuration );
            }

            return builder;
        }
    }
}
