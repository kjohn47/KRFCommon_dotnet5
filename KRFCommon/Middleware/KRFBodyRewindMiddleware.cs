namespace KRFCommon.Middleware
{
    using KRFCommon.Api;

    using Microsoft.AspNetCore.Http;

    using System.Threading.Tasks;
    public sealed class KRFBodyRewindMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly int? _buffer;

        public KRFBodyRewindMiddleware( RequestDelegate next, AppConfiguration configuration )
        {
            this._next = next;
            this._buffer = configuration.RequestBufferSize;
        }

        public async Task Invoke( HttpContext context )
        {
            if ( !this._buffer.HasValue || context.Request.ContentLength <= this._buffer.Value )
            {
                if ( !context.Request.Body.CanSeek )
                {
                    if ( this._buffer.HasValue )
                        context.Request.EnableBuffering( this._buffer.Value );
                    else
                        context.Request.EnableBuffering();
                }
            }

            await _next( context );
        }
    }
}
