namespace KRFCommon.Logger
{
    using System;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class KRFLogExtensions
    {
        public static ILoggingBuilder AddKRFLogToFileLogger( this ILoggingBuilder builder )
        {
            builder.Services.AddSingleton<ILoggerProvider, KRFLogToFileLoggerProvider>();
            return builder;
        }

        public static ILoggingBuilder AddKRFLogToFileLogger( this ILoggingBuilder builder, Action<KRFLogToFileLoggerOptions> configure )
        {
            if( configure == null )
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
