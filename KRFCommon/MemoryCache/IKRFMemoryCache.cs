namespace KRFCommon.MemoryCache
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Caching.Memory;

    public interface IKRFMemoryCache : IMemoryCache
    {
        TResult GetCachedItem<TResult>( string key, Func<TResult> queryFunc, string settingsKey = null ) where TResult : class;

        Task<TResult> GetCachedItemAsync<TResult>( string key, Func<Task<TResult>> queryFunc, string settingsKey = null ) where TResult : class;
    }
}
