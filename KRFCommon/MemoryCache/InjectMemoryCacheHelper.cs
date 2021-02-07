namespace KRFCommon.MemoryCache
{
    using System;

    using Microsoft.Extensions.DependencyInjection;

    public static class InjectMemoryCacheHelper
    {
        public static void InjectMemoryCache<TSettings>( this IServiceCollection services, TSettings settings )
            where TSettings : MemoryCacheSettingsBase
        {
            var cacheSettings = settings ?? new MemoryCacheSettingsBase();

            services.AddSingleton( ( TSettings ) cacheSettings );
            services.AddMemoryCache( x =>
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
            } );
        }
    }
}
