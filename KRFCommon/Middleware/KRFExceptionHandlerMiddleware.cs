namespace KRFCommon.Middleware
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
    using System.Net.Http;
    using KRFCommon.Api;
    using KRFCommon.JSON;
    using KRFCommon.MemoryCache;

    public static class KRFExceptionHandlerMiddleware
    {
        public static IApplicationBuilder KRFLogAndExceptionHandlerConfigure( this IApplicationBuilder app, ILoggerFactory loggerFactory, AppConfiguration configuration, bool isDev = false )
        {
            if ( configuration.EnableReqLogs || isDev )
            {
                    app.UseMiddleware<KRFLogRequestResponseMiddleware>( loggerFactory, configuration );
            }

            app._KRFExceptionHandlerMiddlewareConfigure( loggerFactory, configuration, isDev );

            return app;
        }

        public static IApplicationBuilder KRFExceptionHandlerMiddlewareConfigure( this IApplicationBuilder app, ILoggerFactory loggerFactory, AppConfiguration configuration, bool isDev = false )
        {
            if ( configuration.EnableReqLogs || isDev )
            {
                app.UseMiddleware<KRFBodyRewindMiddleware>( configuration );
            }

            app._KRFExceptionHandlerMiddlewareConfigure( loggerFactory, configuration, isDev );

            return app;
        }

        private static IApplicationBuilder _KRFExceptionHandlerMiddlewareConfigure( this IApplicationBuilder app, ILoggerFactory loggerFactory, AppConfiguration configuration, bool isDev = false )
        {
            var _eventId = new EventId( KRFConstants.ApiEventId, KRFConstants.LogExceptionEvtName );
            app.UseExceptionHandler( b => b.Run( async c =>
              {
                  var error = ( IExceptionHandlerFeature ) c.Features[ typeof( IExceptionHandlerFeature ) ];

                  if ( configuration.EnableReqLogs || isDev )
                  {
                      var requestToken = c.Request.Headers[ configuration.TokenIdentifier ];
                      string reqBody;
                      if ( c.Request.Body.CanSeek &&
                            !c.Request.Method.Equals( KRFConstants.GetMethod, StringComparison.InvariantCultureIgnoreCase ) &&
                            !c.Request.Method.Equals( KRFConstants.DeleteMethod, StringComparison.InvariantCultureIgnoreCase ) &&
                            c.Request.ContentLength != null && ( !configuration.RequestBufferSize.HasValue || configuration.RequestBufferSize.Value >= c.Request.ContentLength ) &&
                            c.Request.ContentType != null && c.Request.ContentType.Contains( KRFConstants.JsonContentType, StringComparison.InvariantCultureIgnoreCase ) )
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
                      var appLogger = loggerFactory.CreateLogger( string.Format( "{0} - {1}", configuration.ApiName, "Exceptions" ) );

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
                      log.Append( "\nStatusCode:" );
                      log.Append( c.Response.StatusCode.ToString() );
                      log.Append( "\nRequest Body:\n" );
                      log.Append( reqBody );
                      log.Append( "\n\n------------------------------------------------------" );
                      log.Append( "\n                     Exception:" );
                      log.Append( "\n------------------------------------------------------\n" );
                      log.Append( error.Error.Message );

                      appLogger.LogError( _eventId, error.Error, log.ToString() );
                  }

                  //Check specific exceptons first
                  var serializerOpt = KRFJsonSerializerOptions.GetJsonSerializerOptions();
                  switch ( error.Error )
                  {
                      case HttpRequestException httpEx:
                      {
                          await c.Response.WriteAsJsonAsync( new ErrorOut( ( HttpStatusCode ) c.Response.StatusCode, "Could not execute request to the server", ResponseErrorType.Exception, "HttpRequest", KRFConstants.HttpExErrorCode ), serializerOpt );
                          break;
                      }
                      case KRFMemoryCacheException memoryCacheException:
                      {
                          await c.Response.WriteAsJsonAsync( new ErrorOut( ( HttpStatusCode ) c.Response.StatusCode, memoryCacheException.Message, ResponseErrorType.Exception, "MemoryCache", memoryCacheException.Code ), serializerOpt );
                          break;
                      }
                      default:
                      {
                          await c.Response.WriteAsJsonAsync( new ErrorOut( ( HttpStatusCode ) c.Response.StatusCode, error.Error.Message, ResponseErrorType.Exception, "Exception", KRFConstants.DefaultErrorCode ), serializerOpt );
                          break;
                      }
                  }

              } ) );


            return app;
        }
    }
}
