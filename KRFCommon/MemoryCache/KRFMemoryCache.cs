namespace KRFCommon.MemoryCache
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;

    public class KRFMemoryCache : MemoryCache,  IKRFMemoryCache
    {
        private readonly IDictionary<string, KRFCachedKeySettings> CachedKeySettings;

        private const int DefaultCacheIntervalMinutes = 60;

        public KRFMemoryCache( Microsoft.Extensions.Options.IOptions<KRFMemoryCacheOptions> options ) : base( options )
        {
            if ( options != null && options.Value != null && options.Value.CachedKeySettings != null )
            {
                this.CachedKeySettings = options.Value.CachedKeySettings;
            }
            else
            {
                this.CachedKeySettings = new Dictionary<string, KRFCachedKeySettings>();
            }
        }

        public KRFMemoryCache( Microsoft.Extensions.Options.IOptions<KRFMemoryCacheOptions> options, ILoggerFactory loggerFactory ) : base( options, loggerFactory )
        {
            if ( options != null && options.Value != null && options.Value.CachedKeySettings != null )
            {
                this.CachedKeySettings = options.Value.CachedKeySettings;
            }
            else
            {
                this.CachedKeySettings = new Dictionary<string, KRFCachedKeySettings>();
            }
        }

        public KRFCacheResult<TResult> GetCachedItem<TResult>( string key )
            where TResult : class
        {
            return this.GetAndValidateCacheValue<TResult>( key );
        }

        public void SetCachedItem<TItem>( string key, TItem value, string settingsKey = null )
            where TItem : class
        {
            this.SetCacheValue( value, key, settingsKey );
        }

        public void RemoveCachedItem( string key )
        {
            this.Remove( key );
        }

        public async Task<KRFCacheResult<TResult>> GetOrInsertCachedItemAsync<TResult>( string key, Func<Task<TResult>> fetchDataHandler, Func<TResult, IMemoryCacheHandlerResult<TResult>> fetchDataResultHandler = null ) where TResult : class
        {
            return await this.GetItemFromCacheAsync( key, fetchDataHandler, fetchDataResultHandler, null );
        }

        public async Task<KRFCacheResult<TResult>> GetOrInsertCachedItemAsync<TResult>( string key, string settingsKey, Func<Task<TResult>> fetchDataHandler, Func<TResult, IMemoryCacheHandlerResult<TResult>> fetchDataResultHandler = null ) where TResult : class
        {
            return await this.GetItemFromCacheAsync( key, fetchDataHandler, fetchDataResultHandler, settingsKey );
        }

        public async Task<TResult> GetOrInsertCachedItemAsync<TResult>( string key, Func<Task<TResult>> fetchDataHandler, Func<KRFCacheResult<TResult>, TResult> resultHandler, Func<TResult, IMemoryCacheHandlerResult<TResult>> fetchDataResultHandler = null ) where TResult : class
        {
            return await this.GetHandledItemFromCacheAsync( key, fetchDataHandler, resultHandler, fetchDataResultHandler, null );
        }

        public async Task<TResult> GetOrInsertCachedItemAsync<TResult>( string key, string settingsKey, Func<Task<TResult>> fetchDataHandler, Func<KRFCacheResult<TResult>, TResult> resultHandler, Func<TResult, IMemoryCacheHandlerResult<TResult>> fetchDataResultHandler = null ) where TResult : class
        {
            return await this.GetHandledItemFromCacheAsync( key, fetchDataHandler, resultHandler, fetchDataResultHandler, settingsKey );
        }

        private async Task<TResult> GetHandledItemFromCacheAsync<TResult>( string key, Func<Task<TResult>> fetchDataHandler, Func<KRFCacheResult<TResult>, TResult> resultHandler, Func<TResult, IMemoryCacheHandlerResult<TResult>> fetchDataResultHandler = null, string settingsKey = null )
            where TResult : class
        {
            var cachedValue = await this.GetItemFromCacheAsync( key, fetchDataHandler, fetchDataResultHandler, settingsKey );
            return resultHandler( cachedValue );
        }

        private async Task<KRFCacheResult<TResult>> GetItemFromCacheAsync<TResult>( string key, Func<Task<TResult>> fetchDataHandler, Func<TResult, IMemoryCacheHandlerResult<TResult>> fetchDataResultHandler = null, string settingsKey = null )
            where TResult : class
        {
            KRFCacheResult<TResult> cachedValue = this.GetAndValidateCacheValue<TResult>( key );
            if ( cachedValue.CacheMiss )
            {
                var response = await fetchDataHandler();
                if ( fetchDataResultHandler != null )
                {
                    var handledResponse = fetchDataResultHandler( response );
                    if ( handledResponse.PreventCacheWrite )
                    {
                        cachedValue.Result = handledResponse.Result;
                        return cachedValue;
                    }
                    response = handledResponse.Result;
                }

                cachedValue.Result = this.SetCacheValue( response, key, settingsKey );
            }

            return cachedValue;
        }

        private KRFCacheResult<TResult> GetAndValidateCacheValue<TResult>( string key )
            where TResult : class
        {
            if ( this.TryGetValue( key, out object value ) )
            {
                if ( !( value is TResult ) )
                {
                    throw new KRFMemoryCacheException(
                        "CACHEDVALUETYPEEXCEPTION",
                        string.Format( "Invalid cache key {0} for object {1}", key, typeof( TResult ).Name ) );
                }

                return new KRFCacheResult<TResult>
                {
                    Result = value as TResult
                };
            }
            else
            {
                return new KRFCacheResult<TResult>()
                {
                    CacheMiss = true
                };
            }
        }

        private TResult SetCacheValue<TResult>( TResult value, string key, string settingsKey )
        {
            var removeDate = DateTime.Now;
            if ( this.CachedKeySettings.TryGetValue( string.IsNullOrEmpty( settingsKey ) ? key : settingsKey, out KRFCachedKeySettings settings ) )
            {
                removeDate = removeDate.AddHours( settings.RemoveHours )
                                       .AddMinutes( settings.RemoveMinutes )
                                       .AddSeconds( settings.RemoveSeconds );
            }
            else
            {
                removeDate = removeDate.AddMinutes( DefaultCacheIntervalMinutes );
            }

            this.Set( key, value, removeDate );
            return value;
        }
    }
}
