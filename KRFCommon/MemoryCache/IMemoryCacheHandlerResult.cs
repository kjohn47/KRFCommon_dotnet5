namespace KRFCommon.MemoryCache
{
    public interface IMemoryCacheHandlerResult<TResult> where TResult : class
    {
        bool PreventCacheWrite { get; }
        TResult Result { get; }
    }
}
