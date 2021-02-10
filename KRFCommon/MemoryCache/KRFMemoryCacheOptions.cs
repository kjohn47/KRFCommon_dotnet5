namespace KRFCommon.MemoryCache
{
    using System.Collections.Generic;

    using Microsoft.Extensions.Caching.Memory;
    public class KRFMemoryCacheOptions : MemoryCacheOptions
    {
        public IDictionary<string, KRFCachedKeySettings> CachedKeySettings { get; set; }
    }
}
