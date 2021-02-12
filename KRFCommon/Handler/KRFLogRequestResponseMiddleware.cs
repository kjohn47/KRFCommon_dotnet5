namespace KRFCommon.Handler
{
    using KRFCommon.Constants;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    public sealed class KRFLogRequestResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly int? _buffer;
        private readonly ILogger _logger;
        private readonly string _tokenIdentifier;

        public KRFLogRequestResponseMiddleware( RequestDelegate next, ILoggerFactory loggerFactory, string apiName, string tokenIdentifier )
        {
            this._next = next;
            this._logger = loggerFactory.CreateLogger( string.Format( "{0} - {1}", apiName, "Request/Response" ) );
            this._tokenIdentifier = tokenIdentifier;
        }
        public KRFLogRequestResponseMiddleware( RequestDelegate next, ILoggerFactory loggerFactory, string apiName, string tokenIdentifier, int? buffer )
        {
            this._next = next;
            this._buffer = buffer;
            this._logger = loggerFactory.CreateLogger( string.Format( "{0} - {1}", apiName, "Request/Response" ) );
            this._tokenIdentifier = tokenIdentifier;
        }

        public async Task Invoke( HttpContext context )
        {
            if ( !this._buffer.HasValue || 
                ( context.Request.ContentLength == null && context.Request.Method.Equals( KRFConstants.GetMethod, StringComparison.InvariantCultureIgnoreCase ) ) || 
                context.Request.ContentLength <= this._buffer )
            {
                if ( !context.Request.Body.CanSeek )
                {
                    if ( this._buffer.HasValue )
                        context.Request.EnableBuffering( this._buffer.Value );
                    else
                        context.Request.EnableBuffering();
                }

                //log request
                var requestToken = context.Request.Headers[ this._tokenIdentifier ];
                var requestUrl = context.Request.Path + context.Request.QueryString;
                string response;
                string request;

                if ( context.Request.ContentType != null &&
                    context.Request.ContentType.Contains( KRFConstants.JsonContentType, StringComparison.InvariantCultureIgnoreCase ) &&
                    !context.Request.Method.Equals( KRFConstants.GetMethod, StringComparison.InvariantCultureIgnoreCase ) )
                {
                    var reqBody = context.Request.Body;
                    reqBody.Seek( 0, SeekOrigin.Begin );
                    var reqReader = new StreamReader( reqBody );
                    request = await reqReader.ReadToEndAsync();
                    reqReader.Dispose();
                    reqBody.Seek( 0, SeekOrigin.Begin );
                }
                else
                {
                    request = "Could not read request body: Get method or not type application/json or Request reading disabled or content too long";
                }

                //backup response stream and set memory stream
                Stream originalBody = context.Response.Body;
                var respBody = new MemoryStream();
                context.Response.Body = respBody;

                //wait to finish
                await _next( context );

                respBody.Seek( 0, SeekOrigin.Begin );
                await respBody.CopyToAsync( originalBody );
                respBody.Seek( 0, SeekOrigin.Begin );

                //log response                
                if ( context.Response.ContentType == null || !context.Response.ContentType.Contains( KRFConstants.JsonContentType, StringComparison.InvariantCultureIgnoreCase ) )
                {
                    response = "No response logged - no application/json content fount";
                    context.Response.Body = originalBody;
                    await respBody.DisposeAsync();
                }
                else
                {
                    var respReader = new StreamReader( respBody );
                    response = await respReader.ReadToEndAsync();
                    respReader.Dispose();
                    context.Response.Body = originalBody;
                    await respBody.DisposeAsync();
                }

                var log = new StringBuilder();
                log.Append( "\n------------------------------------------------------" );
                log.Append( "\n                     Request Log:" );
                log.Append( "\n------------------------------------------------------" );
                log.Append( "\nRequest: " );
                log.Append( requestUrl );
                log.Append( "\nRequest Method: " );
                log.Append( context.Request.Method );
                log.Append( "\nRequest Token: " );
                log.Append( requestToken );
                log.Append( "\nRequest Body:\n" );
                log.Append( request );
                log.Append( "\n\n------------------------------------------------------" );
                log.Append( "\n                     Response Log:" );
                log.Append( "\n------------------------------------------------------" );
                log.Append( "\nResponse Body:\n" );
                log.Append( response );

                this._logger.LogInformation( log.ToString() );
            }
        }
    }
}
