namespace KRFCommon.MemoryCache
{
    public class KRFCacheResult<TResult> where TResult : class
    {
        public TResult Result { get; set; }
        public bool CacheMiss { get; set; }
    }
}
