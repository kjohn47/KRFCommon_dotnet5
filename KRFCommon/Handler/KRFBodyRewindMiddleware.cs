namespace KRFCommon.Handler
{
    using Microsoft.AspNetCore.Http;

    using System.Threading.Tasks;
    public sealed class KRFBodyRewindMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly int _buffer;
        private readonly bool _onlyBuffer;

        public KRFBodyRewindMiddleware( RequestDelegate next )
        {
            this._next = next;
            this._buffer = 30000;
        }
        public KRFBodyRewindMiddleware( RequestDelegate next, int buffer )
        {
            this._next = next;
            this._buffer = buffer;
        }

        public KRFBodyRewindMiddleware( RequestDelegate next, int buffer, bool onlyBuffer )
        {
            this._next = next;
            this._buffer = buffer;
            this._onlyBuffer = onlyBuffer;
        }

        public async Task Invoke( HttpContext context )
        {
            if ( !this._onlyBuffer || context.Request.ContentLength <= this._buffer )
            {
                context.Request.EnableBuffering( this._buffer );
            }

            await _next( context );
        }
    }
}
