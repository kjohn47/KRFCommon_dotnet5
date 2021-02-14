namespace KRFCommon.Middleware
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
        private readonly EventId _eventID;

        public KRFLogRequestResponseMiddleware( RequestDelegate next, ILoggerFactory loggerFactory, string apiName, string tokenIdentifier )
        {
            this._next = next;
            this._logger = loggerFactory.CreateLogger( string.Format( "{0} - {1}", apiName, "Request/Response" ) );
            this._tokenIdentifier = tokenIdentifier;
            this._eventID = new EventId( KRFConstants.ApiEventId, KRFConstants.LogReqRespEvtName );
        }
        public KRFLogRequestResponseMiddleware( RequestDelegate next, ILoggerFactory loggerFactory, string apiName, string tokenIdentifier, int? buffer = null ) 
            : this( next, loggerFactory, apiName, tokenIdentifier )
        {
            this._buffer = buffer;
        }

        public async Task Invoke( HttpContext context )
        {
            if ( !context.Request.Path.StartsWithSegments( "/swagger" ) && ( !this._buffer.HasValue ||
                ( context.Request.ContentLength == null && context.Request.Method.Equals( KRFConstants.GetMethod, StringComparison.InvariantCultureIgnoreCase ) ) ||
                context.Request.ContentLength <= this._buffer ) )
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
                var requestTime = DateTime.Now.ToString( KRFConstants.TimeStampFormat );
                string response;
                string request;

                if ( context.Request.ContentType != null &&
                    context.Request.ContentType.Contains( KRFConstants.JsonContentType, StringComparison.InvariantCultureIgnoreCase ) &&
                    !context.Request.Method.Equals( KRFConstants.GetMethod, StringComparison.InvariantCultureIgnoreCase ) )
                {
                    var reqBody = new MemoryStream();
                    context.Request.Body.Seek( 0, SeekOrigin.Begin );
                    await context.Request.Body.CopyToAsync( reqBody );
                    context.Request.Body.Seek( 0, SeekOrigin.Begin );
                    reqBody.Seek( 0, SeekOrigin.Begin );

                    using ( var reqReader = new StreamReader( reqBody ) )
                    {
                        request = await reqReader.ReadToEndAsync();
                    }
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
                    using ( var respReader = new StreamReader( respBody ) )
                    {
                        response = await respReader.ReadToEndAsync();
                    }
                    context.Response.Body = originalBody;
                }

                var log = new StringBuilder();
                log.Append( "\n------------------------------------------------------" );
                log.Append( "\n                     Request Log:" );
                log.Append( "\n------------------------------------------------------" );
                log.Append( "\nTimeStamp:" );
                log.Append( requestTime );
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
                log.Append( "\nTimeStamp:" );
                log.Append( DateTime.Now.ToString( KRFConstants.TimeStampFormat ) );
                log.Append( "\nResponse Body:\n" );
                log.Append( response );

                this._logger.LogInformation( this._eventID, log.ToString() );
            }
            else
            {
                await _next( context );
            }
        }
    }
}
