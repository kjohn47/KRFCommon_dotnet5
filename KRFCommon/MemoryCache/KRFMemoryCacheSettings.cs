namespace KRFCommon.MemoryCache
{
    using System.Collections.Generic;

    public class KRFMemoryCacheSettings
    {
        private const int _defaultCleanupInterval = 10;

        public KRFMemoryCacheSettings()
        { }

        private KRFMemoryCacheCleanup _cacheCleanupInterval;

        public KRFMemoryCacheSize MemoryCacheSize { get; set; }

        public KRFMemoryCacheCleanup CacheCleanupInterval
        {
            get
            {
                if( this._cacheCleanupInterval != null )
                {
                    return this._cacheCleanupInterval;
                }

                this._cacheCleanupInterval = new KRFMemoryCacheCleanup
                {
                    Minutes = _defaultCleanupInterval
                };

                return this._cacheCleanupInterval;
            }

            set
            {
                this._cacheCleanupInterval = value;
            }
        }

        public IDictionary<string, KRFCachedKeySettings> CachedKeySettings { get; set; }
    }
}
