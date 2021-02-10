namespace KRFCommon.MemoryCache
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;

    public class KRFMemoryCache : MemoryCache,  IKRFMemoryCache
    {
        public KRFMemoryCache( Microsoft.Extensions.Options.IOptions<KRFMemoryCacheOptions> options ) : base( options )
        {
            if ( options != null && options.Value != null && options.Value.CachedKeySettings != null)
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

        private readonly IDictionary<string, KRFCachedKeySettings> CachedKeySettings;

        private const int DefaultCacheIntervalMinutes = 60;

        public TResult GetCachedItem<TResult>( string key, Func<TResult> queryFunc, string settingsKey = null, bool preventCacheUpdate = false )
        where TResult : class
        {
            TResult cachedValue;

            if ( !this.GetAndValidateCacheValue( key, out cachedValue ) )
            {
                var response = queryFunc();

                return preventCacheUpdate ? response : this.SetCacheValue( response, key, settingsKey );
            }

            return cachedValue;
        }

        public async Task<TResult> GetCachedItemAsync<TResult>( string key, Func<Task<TResult>> queryFunc, string settingsKey = null, bool preventCacheUpdate = false )
            where TResult : class
        {
            TResult cachedValue;

            if ( !this.GetAndValidateCacheValue( key, out cachedValue ) )
            {
                var response = await queryFunc();

                return preventCacheUpdate ? response : this.SetCacheValue( response, key, settingsKey );
            }

            return cachedValue;
        }

        public KRFCacheResult<TResult> GetCachedItemWithMissReturn<TResult>( string key, Func<TResult> queryFunc, string settingsKey = null, bool preventCacheUpdate = false ) where TResult : class
        {
            TResult cachedValue;

            if ( !this.GetAndValidateCacheValue( key, out cachedValue ) )
            {
                var response = queryFunc();

                return new KRFCacheResult<TResult>
                {
                    Result = preventCacheUpdate ? response : this.SetCacheValue( response, key, settingsKey ),
                    CacheMiss = true
                };
            }

            return new KRFCacheResult<TResult>
            {
                Result = cachedValue,
                CacheMiss = false
            };
        }

        public async Task<KRFCacheResult<TResult>> GetCachedItemWithMissReturnAsync<TResult>( string key, Func<Task<TResult>> queryFunc, string settingsKey = null, bool preventCacheUpdate = false ) where TResult : class
        {
            TResult cachedValue;

            if ( !this.GetAndValidateCacheValue( key, out cachedValue ) )
            {
                var response = await queryFunc();

                return new KRFCacheResult<TResult>
                {
                    Result = preventCacheUpdate ? response : this.SetCacheValue( response, key, settingsKey ),
                    CacheMiss = true
                };
            }

            return new KRFCacheResult<TResult>
            {
                Result = cachedValue,
                CacheMiss = false
            };
        }

        private bool GetAndValidateCacheValue<TResult>( string key, out TResult cachedValue )
            where TResult : class
        {
            cachedValue = null;

            if ( this.TryGetValue( key, out object value ) )
            {
                if ( !( value is TResult ) )
                {
                    throw new Exception( string.Format( "Invalid cache key {0} for object {1}", key, typeof( TResult ).Name ) );
                }

                cachedValue = value as TResult;
                return true;
            }
            else
            {
                return false;
            }
        }

        private TResult SetCacheValue<TResult>( TResult value, string key, string settingsKey )
        {
            var removeDate = DateTime.Now;
            if(this.CachedKeySettings.TryGetValue(string.IsNullOrEmpty(settingsKey) ? key : settingsKey, out KRFCachedKeySettings settings ))
            {
                removeDate = removeDate.AddHours( settings.RemoveHours ).AddMinutes( settings.RemoveMinutes ).AddSeconds( settings.RemoveSeconds );
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
