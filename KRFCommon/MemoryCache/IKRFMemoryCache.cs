namespace KRFCommon.MemoryCache
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Caching.Memory;

    public interface IKRFMemoryCache : IMemoryCache
    {
        KRFCacheResult<TResult> GetCachedItem<TResult>( string key ) where TResult : class;

        void SetCachedItem<TItem>( string key, TItem value, string settingsKey = null ) where TItem : class;

        void RemoveCachedItem( string key );

        Task<KRFCacheResult<TResult>> GetOrInsertCachedItemAsync<TResult>( string key, Func<Task<TResult>> fetchDataHandler, Func<TResult, IMemoryCacheHandlerResult<TResult>> fetchDataResultHandler = null ) where TResult : class;

        Task<KRFCacheResult<TResult>> GetOrInsertCachedItemAsync<TResult>( string key, string settingsKey, Func<Task<TResult>> fetchDataHandler, Func<TResult, IMemoryCacheHandlerResult<TResult>> fetchDataResultHandler = null ) where TResult : class;

        Task<TResult> GetOrInsertCachedItemAsync<TResult>( string key, Func<Task<TResult>> fetchDataHandler, Func<KRFCacheResult<TResult>, TResult> resultHandler, Func<TResult, IMemoryCacheHandlerResult<TResult>> fetchDataResultHandler = null ) where TResult : class;

        Task<TResult> GetOrInsertCachedItemAsync<TResult>( string key, string settingsKey, Func<Task<TResult>> fetchDataHandler, Func<KRFCacheResult<TResult>, TResult> resultHandler, Func<TResult, IMemoryCacheHandlerResult<TResult>> fetchDataResultHandler = null ) where TResult : class;
    }
}
