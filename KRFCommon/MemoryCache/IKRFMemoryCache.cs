namespace KRFCommon.MemoryCache
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Caching.Memory;

    public interface IKRFMemoryCache : IMemoryCache
    {
        TResult GetCachedItem<TResult>( string key, Func<TResult> queryFunc ) where TResult : class;

        TResult GetCachedItem<TResult>( string key, Func<TResult> queryFunc, string settingsKey ) where TResult : class;

        TResult GetCachedItem<TResult>( string key, Func<TResult> queryFunc, bool preventCacheUpdate ) where TResult : class;

        TResult GetCachedItem<TResult>( string key, Func<TResult> queryFunc, string settingsKey, bool preventCacheUpdate ) where TResult : class;

        Task<TResult> GetCachedItemAsync<TResult>( string key, Func<Task<TResult>> queryFunc ) where TResult : class;

        Task<TResult> GetCachedItemAsync<TResult>( string key, Func<Task<TResult>> queryFunc, string settingsKey ) where TResult : class;

        Task<TResult> GetCachedItemAsync<TResult>( string key, Func<Task<TResult>> queryFunc, bool preventCacheUpdate ) where TResult : class;

        Task<TResult> GetCachedItemAsync<TResult>( string key, Func<Task<TResult>> queryFunc, string settingsKey, bool preventCacheUpdate ) where TResult : class;

        KRFCacheResult<TResult> GetCachedItemWithMissReturn<TResult>( string key, Func<TResult> queryFunc ) where TResult : class;

        KRFCacheResult<TResult> GetCachedItemWithMissReturn<TResult>( string key, Func<TResult> queryFunc, string settingsKey ) where TResult : class;

        KRFCacheResult<TResult> GetCachedItemWithMissReturn<TResult>( string key, Func<TResult> queryFunc, bool preventCacheUpdate ) where TResult : class;

        KRFCacheResult<TResult> GetCachedItemWithMissReturn<TResult>( string key, Func<TResult> queryFunc, string settingsKey, bool preventCacheUpdate ) where TResult : class;

        Task<KRFCacheResult<TResult>> GetCachedItemWithMissReturnAsync<TResult>( string key, Func<Task<TResult>> queryFunc ) where TResult : class;

        Task<KRFCacheResult<TResult>> GetCachedItemWithMissReturnAsync<TResult>( string key, Func<Task<TResult>> queryFunc, string settingsKey ) where TResult : class;

        Task<KRFCacheResult<TResult>> GetCachedItemWithMissReturnAsync<TResult>( string key, Func<Task<TResult>> queryFunc, bool preventCacheUpdate ) where TResult : class;

        Task<KRFCacheResult<TResult>> GetCachedItemWithMissReturnAsync<TResult>( string key, Func<Task<TResult>> queryFunc, string settingsKey, bool preventCacheUpdate ) where TResult : class;

        TResult GetCachedItemWithHandler<TResult>( string key, Func<TResult> queryFunc, Func<KRFCacheResult<TResult>, TResult> handler ) where TResult : class;

        TResult GetCachedItemWithHandler<TResult>( string key, Func<TResult> queryFunc, string settingsKey, Func<KRFCacheResult<TResult>, TResult> handler ) where TResult : class;

        TResult GetCachedItemWithHandler<TResult>( string key, Func<TResult> queryFunc, bool preventCacheUpdate, Func<KRFCacheResult<TResult>, TResult> handler ) where TResult : class;

        TResult GetCachedItemWithHandler<TResult>( string key, Func<TResult> queryFunc, string settingsKey, bool preventCacheUpdate, Func<KRFCacheResult<TResult>, TResult> handler ) where TResult : class;

        Task<TResult> GetCachedItemWithHandlerAsync<TResult>( string key, Func<Task<TResult>> queryFunc, Func<KRFCacheResult<TResult>, TResult> handler ) where TResult : class;

        Task<TResult> GetCachedItemWithHandlerAsync<TResult>( string key, Func<Task<TResult>> queryFunc, string settingsKey, Func<KRFCacheResult<TResult>, TResult> handler ) where TResult : class;

        Task<TResult> GetCachedItemWithHandlerAsync<TResult>( string key, Func<Task<TResult>> queryFunc, bool preventCacheUpdate, Func<KRFCacheResult<TResult>, TResult> handler ) where TResult : class;

        Task<TResult> GetCachedItemWithHandlerAsync<TResult>( string key, Func<Task<TResult>> queryFunc, string settingsKey, bool preventCacheUpdate, Func<KRFCacheResult<TResult>, TResult> handler ) where TResult : class;
    }
}
