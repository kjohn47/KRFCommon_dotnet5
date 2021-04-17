namespace KRFCommon.MemoryCache
{
    using System;

    public class KRFMemoryCacheException : Exception
    {
        public KRFMemoryCacheException( string code, string message ) : base( message )
        {
            this.Code = code;
        }

        public string Code { get; set; }
    }
}
