namespace KRFCommon.MemoryCache
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Caching.Memory;

    public static class KRFMemoryCacheExtensions
    {
        public static TResult GetCachedItem<TResult>( this IMemoryCache memoryCache, string key, int minuteInterval, Func<TResult> queryFunc )
        {
            TResult response;

            if ( !memoryCache.TryGetValue<TResult>( key, out response ) )
            {
                response = queryFunc();

                memoryCache.Set<TResult>( key, response, DateTime.Now.AddMinutes( minuteInterval ) );

                return response;
            }

            return response;
        }

        public static async Task<TResult> GetCachedItemAsync<TResult>( this IMemoryCache memoryCache, string key, int minuteInterval, Func<Task<TResult>> queryFunc )
        {
            TResult response;

            if ( !memoryCache.TryGetValue<TResult>( key, out response ) )
            {
                response = await queryFunc();

                memoryCache.Set<TResult>( key, response, DateTime.Now.AddMinutes( minuteInterval ) );

                return response;
            }

            return response;
        }
    }
}
