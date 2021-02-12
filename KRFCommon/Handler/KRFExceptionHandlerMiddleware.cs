namespace KRFCommon.Handler
{
    using System.Net;
    using System.IO;
    using System.Text.Json;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using KRFCommon.CQRS.Common;
    using System.Text;
    using System;
    using KRFCommon.Constants;

    public static class KRFExceptionHandlerMiddleware
    {
        public static IApplicationBuilder KRFLogAndExceptionHandlerConfigure( this IApplicationBuilder app, ILoggerFactory loggerFactory, bool enableLogs, string apiName, string tokenIdentifier, bool enableReadRequest, int? reqBufferSize = null )
        {
            if ( enableLogs && enableReadRequest )
            {
                if ( reqBufferSize.HasValue )
                    app.UseMiddleware<KRFLogRequestResponseMiddleware>( loggerFactory, apiName, tokenIdentifier, reqBufferSize );
                else
                    app.UseMiddleware<KRFLogRequestResponseMiddleware>( loggerFactory, apiName, tokenIdentifier );
            }

            app._KRFExceptionHandlerMiddlewareConfigure( loggerFactory, enableLogs, apiName, tokenIdentifier, enableReadRequest ? reqBufferSize : null );

            return app;
        }

        public static IApplicationBuilder KRFExceptionHandlerMiddlewareConfigure( this IApplicationBuilder app, ILoggerFactory loggerFactory, bool enableLogs, string apiName, string tokenIdentifier, bool enableReadRequest, int? reqBufferSize = null )
        {
            if ( enableLogs && enableReadRequest )
            {
                if ( reqBufferSize.HasValue )
                    app.UseMiddleware<KRFBodyRewindMiddleware>( reqBufferSize );
                else
                    app.UseMiddleware<KRFBodyRewindMiddleware>();
            }

            app._KRFExceptionHandlerMiddlewareConfigure( loggerFactory, enableLogs, apiName, tokenIdentifier, enableReadRequest ? reqBufferSize : null );

            return app;
        }

        public static IApplicationBuilder KRFExceptionHandlerMiddlewareConfigure( this IApplicationBuilder app, ILoggerFactory loggerFactory, bool logErrors, string apiName, string tokenIdentifier )
        {
            app._KRFExceptionHandlerMiddlewareConfigure( loggerFactory, logErrors, apiName, tokenIdentifier, null );

            return app;
        }

        private static IApplicationBuilder _KRFExceptionHandlerMiddlewareConfigure( this IApplicationBuilder app, ILoggerFactory loggerFactory, bool logErrors, string apiName, string tokenIdentifier, int? logReqLimit )
        {
            app.UseExceptionHandler( b => b.Run( async c =>
              {
                  var error = ( IExceptionHandlerFeature ) c.Features[ typeof( IExceptionHandlerFeature ) ];
                  if ( logErrors )
                  {
                      var requestToken = c.Request.Headers[ tokenIdentifier ];
                      string reqBody;
                      if ( c.Request.Body.CanSeek &&
                            ( logReqLimit == null || ( c.Request.ContentLength != null && ( int ) logReqLimit >= c.Request.ContentLength ) ) &&
                            !c.Request.Method.Equals( KRFConstants.GetMethod, StringComparison.InvariantCultureIgnoreCase ) &&
                            c.Request.ContentType.Contains( KRFConstants.JsonContentType, StringComparison.InvariantCultureIgnoreCase ) )
                      {
                          var body = new MemoryStream();
                          c.Request.Body.Seek( 0, SeekOrigin.Begin );
                          await c.Request.Body.CopyToAsync( body );
                          c.Request.Body.Seek( 0, SeekOrigin.Begin );
                          body.Seek( 0, SeekOrigin.Begin );

                          using ( var reqReader = new StreamReader( body ) )
                          {
                              reqBody = await reqReader.ReadToEndAsync();
                          }
                      }
                      else
                      {
                          reqBody = "Could not read request body: Get method or Request reading disabled or content too long";
                      }
                      var requestUrl = c.Request.Path + c.Request.QueryString;
                      var appLogger = loggerFactory.CreateLogger( string.Format( "{0} - {1}", apiName, "Exceptions" ) );

                      var log = new StringBuilder();
                      log.Append( "\n------------------------------------------------------" );
                      log.Append( "\n                     Request Log:" );
                      log.Append( "\n------------------------------------------------------" );
                      log.Append( "\nTimeStamp:" );
                      log.Append( DateTime.Now.ToString( KRFConstants.TimeStampFormat ) );
                      log.Append( "\nRequest: " );
                      log.Append( requestUrl );
                      log.Append( "\nRequest Method: " );
                      log.Append( c.Request.Method );
                      log.Append( "\nRequest Token: " );
                      log.Append( requestToken );
                      log.Append( "\nRequest Body:\n" );
                      log.Append( reqBody );
                      log.Append( "\n\n------------------------------------------------------" );
                      log.Append( "\n                     Exception:" );
                      log.Append( "\n------------------------------------------------------\n" );
                      log.Append( error.Error.Message );

                      appLogger.LogError( error.Error, log.ToString() );
                  }
                  c.Response.ContentType = KRFConstants.JsonContentUtf8Type;
                  string errorMessage = JsonSerializer.Serialize( new ErrorOut( ( HttpStatusCode ) c.Response.StatusCode, error.Error.Message, false, ResponseErrorType.Exception, "Exception" ) );
                  await c.Response.WriteAsync( errorMessage );
              } ) );

            return app;
        }
    }
}
