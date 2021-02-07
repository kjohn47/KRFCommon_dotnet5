namespace KRFCommon.MemoryCache
{
    public class MemoryCacheSettingsBase
    {
        private const int _defaultCleanupInterval = 10;

        public MemoryCacheSettingsBase()
        { }

        private MemoryCacheCleanup _cacheCleanupInterval;

        public MemoryCacheSize MemoryCacheSize { get; set; }

        public MemoryCacheCleanup CacheCleanupInterval
        {
            get
            {
                if( this._cacheCleanupInterval != null )
                {
                    return this._cacheCleanupInterval;
                }

                this._cacheCleanupInterval = new MemoryCacheCleanup
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
    }
}
