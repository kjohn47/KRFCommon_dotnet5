namespace KRFCommon.MemoryCache
{
    using System;

    using KRFCommon.Constants;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class KRFMemoryCacheServiceHelper
    {
        public static IServiceCollection AddKRFMemoryCache( this IServiceCollection services, IConfiguration configuration )
        {
            if ( services == null )
            {
                throw new ArgumentNullException( nameof( services ) );
            }

            if ( configuration == null )
            {
                throw new ArgumentNullException( nameof( configuration ) );
            }

            KRFMemoryCacheSettings cacheSettings = configuration.GetSection( KRFApiSettings.MemoryCacheSettings_Key ).Get<KRFMemoryCacheSettings>();

            return services.AddKRFMemoryCache( cacheSettings );
        }

        public static IServiceCollection AddKRFMemoryCache( this IServiceCollection services, KRFMemoryCacheSettings settings = null )
        {
            if ( services == null )
            {
                throw new ArgumentNullException( nameof( services ) );
            }

            var cacheSettings = settings ?? new KRFMemoryCacheSettings();

            services.AddOptions();

            services.TryAdd( ServiceDescriptor.Singleton<IKRFMemoryCache, KRFMemoryCache>() );

            services.Configure<KRFMemoryCacheOptions>( x =>
            {
                x.ExpirationScanFrequency = new TimeSpan(
                    cacheSettings.CacheCleanupInterval.Hours,
                    cacheSettings.CacheCleanupInterval.Minutes,
                    cacheSettings.CacheCleanupInterval.Seconds );

                if ( cacheSettings.MemoryCacheSize != null )
                {
                    if ( cacheSettings.MemoryCacheSize.MaxSize.HasValue )
                    {
                        x.SizeLimit = cacheSettings.MemoryCacheSize.MaxSize.Value;
                    }

                    if ( cacheSettings.MemoryCacheSize.CompactionPercentage.HasValue )
                    {
                        x.SizeLimit = cacheSettings.MemoryCacheSize.CompactionPercentage.Value;
                    }
                }

                x.CachedKeySettings = cacheSettings.CachedKeySettings;
            } );

            return services;
        }
    }
}
