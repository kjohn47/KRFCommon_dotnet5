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

        public TResult GetCachedItem<TResult>( string key, Func<TResult> queryFunc ) where TResult : class
        {
            return this.GetCachedItem<TResult>( key, queryFunc, null, false );
        }

        public TResult GetCachedItem<TResult>( string key, Func<TResult> queryFunc, string settingsKey ) where TResult : class
        {
            return this.GetCachedItem<TResult>( key, queryFunc, settingsKey, false );
        }

        public TResult GetCachedItem<TResult>( string key, Func<TResult> queryFunc, bool preventCacheUpdate ) where TResult : class
        {
            return this.GetCachedItem<TResult>( key, queryFunc, null, preventCacheUpdate );
        }

        public Task<TResult> GetCachedItemAsync<TResult>( string key, Func<Task<TResult>> queryFunc ) where TResult : class
        {
            return this.GetCachedItemAsync<TResult>( key, queryFunc, null, false );
        }

        public Task<TResult> GetCachedItemAsync<TResult>( string key, Func<Task<TResult>> queryFunc, string settingsKey ) where TResult : class
        {
            return this.GetCachedItemAsync<TResult>( key, queryFunc, settingsKey, false );
        }

        public Task<TResult> GetCachedItemAsync<TResult>( string key, Func<Task<TResult>> queryFunc, bool preventCacheUpdate ) where TResult : class
        {
            return this.GetCachedItemAsync<TResult>( key, queryFunc, null, preventCacheUpdate );
        }

        public KRFCacheResult<TResult> GetCachedItemWithMissReturn<TResult>( string key, Func<TResult> queryFunc ) where TResult : class
        {
            return this.GetCachedItemWithMissReturn<TResult>( key, queryFunc, null, false );
        }

        public KRFCacheResult<TResult> GetCachedItemWithMissReturn<TResult>( string key, Func<TResult> queryFunc, string settingsKey ) where TResult : class
        {
            return this.GetCachedItemWithMissReturn<TResult>( key, queryFunc, settingsKey, false );
        }

        public KRFCacheResult<TResult> GetCachedItemWithMissReturn<TResult>( string key, Func<TResult> queryFunc, bool preventCacheUpdate ) where TResult : class
        {
            return this.GetCachedItemWithMissReturn<TResult>( key, queryFunc, null, preventCacheUpdate );
        }

        public Task<KRFCacheResult<TResult>> GetCachedItemWithMissReturnAsync<TResult>( string key, Func<Task<TResult>> queryFunc ) where TResult : class
        {
            return this.GetCachedItemWithMissReturnAsync<TResult>( key, queryFunc, null, false );
        }

        public Task<KRFCacheResult<TResult>> GetCachedItemWithMissReturnAsync<TResult>( string key, Func<Task<TResult>> queryFunc, string settingsKey ) where TResult : class
        {
            return this.GetCachedItemWithMissReturnAsync<TResult>( key, queryFunc, settingsKey, false );
        }

        public Task<KRFCacheResult<TResult>> GetCachedItemWithMissReturnAsync<TResult>( string key, Func<Task<TResult>> queryFunc, bool preventCacheUpdate ) where TResult : class
        {
            return this.GetCachedItemWithMissReturnAsync<TResult>( key, queryFunc, null, preventCacheUpdate );
        }

        public TResult GetCachedItemWithHandler<TResult>( string key, Func<TResult> queryFunc, Func<KRFCacheResult<TResult>, TResult> handler ) where TResult : class
        {
            return this.GetCachedItemWithHandler<TResult>(key, queryFunc, null, false, handler );
        }

        public TResult GetCachedItemWithHandler<TResult>( string key, Func<TResult> queryFunc, string settingsKey, Func<KRFCacheResult<TResult>, TResult> handler ) where TResult : class
        {
            return this.GetCachedItemWithHandler<TResult>( key, queryFunc, settingsKey, false, handler );
        }

        public TResult GetCachedItemWithHandler<TResult>( string key, Func<TResult> queryFunc, bool preventCacheUpdate, Func<KRFCacheResult<TResult>, TResult> handler ) where TResult : class
        {
            return this.GetCachedItemWithHandler<TResult>( key, queryFunc, null, preventCacheUpdate, handler );
        }

        public Task<TResult> GetCachedItemWithHandlerAsync<TResult>( string key, Func<Task<TResult>> queryFunc, Func<KRFCacheResult<TResult>, TResult> handler ) where TResult : class
        {
            return this.GetCachedItemWithHandlerAsync<TResult>( key, queryFunc, null, false, handler );
        }

        public Task<TResult> GetCachedItemWithHandlerAsync<TResult>( string key, Func<Task<TResult>> queryFunc, string settingsKey, Func<KRFCacheResult<TResult>, TResult> handler ) where TResult : class
        {
            return this.GetCachedItemWithHandlerAsync<TResult>( key, queryFunc, settingsKey, false, handler );
        }

        public Task<TResult> GetCachedItemWithHandlerAsync<TResult>( string key, Func<Task<TResult>> queryFunc, bool preventCacheUpdate, Func<KRFCacheResult<TResult>, TResult> handler ) where TResult : class
        {
            return this.GetCachedItemWithHandlerAsync<TResult>( key, queryFunc, null, preventCacheUpdate, handler );
        }

        public TResult GetCachedItem<TResult>( string key, Func<TResult> queryFunc, string settingsKey, bool preventCacheUpdate )
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

        public async Task<TResult> GetCachedItemAsync<TResult>( string key, Func<Task<TResult>> queryFunc, string settingsKey, bool preventCacheUpdate )
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

        public KRFCacheResult<TResult> GetCachedItemWithMissReturn<TResult>( string key, Func<TResult> queryFunc, string settingsKey, bool preventCacheUpdate ) where TResult : class
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

        public async Task<KRFCacheResult<TResult>> GetCachedItemWithMissReturnAsync<TResult>( string key, Func<Task<TResult>> queryFunc, string settingsKey, bool preventCacheUpdate ) where TResult : class
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

        public TResult GetCachedItemWithHandler<TResult>( string key, Func<TResult> queryFunc, string settingsKey, bool preventCacheUpdate, Func<KRFCacheResult<TResult>, TResult> handler ) where TResult : class
        {
            TResult cachedValue;

            if ( !this.GetAndValidateCacheValue( key, out cachedValue ) )
            {
                var response = queryFunc();

                return handler( new KRFCacheResult<TResult>
                {
                    Result = preventCacheUpdate ? response : this.SetCacheValue( response, key, settingsKey ),
                    CacheMiss = true
                } );
            }

            return handler( new KRFCacheResult<TResult>
                {
                    Result = cachedValue,
                    CacheMiss = false
                } );
        }

        public async Task<TResult> GetCachedItemWithHandlerAsync<TResult>( string key, Func<Task<TResult>> queryFunc, string settingsKey, bool preventCacheUpdate, Func<KRFCacheResult<TResult>, TResult> handler ) where TResult : class
        {
            TResult cachedValue;

            if ( !this.GetAndValidateCacheValue( key, out cachedValue ) )
            {
                var response = await queryFunc();

                return handler( new KRFCacheResult<TResult>
                {
                    Result = preventCacheUpdate ? response : this.SetCacheValue( response, key, settingsKey ),
                    CacheMiss = true
                } );
            }

            return handler( new KRFCacheResult<TResult>
            {
                Result = cachedValue,
                CacheMiss = false
            } );
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
