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

    public static class KRFExceptionHandlerMiddleware
    {
        public static IApplicationBuilder KRFExceptionHandlerMiddlewareConfigure( this IApplicationBuilder app, ILoggerFactory loggerFactory, bool logErrors, string apiName, string tokenIdentifier, int? logReqLimit = null )
        {
            app.UseExceptionHandler( b => b.Run( async c =>
              {
                  var error = ( IExceptionHandlerFeature ) c.Features[ typeof( IExceptionHandlerFeature ) ];
                  if ( logErrors )
                  {
                      var requestToken = c.Request.Headers[ tokenIdentifier ];
                      string reqBody = "";
                      if ( c.Request.Body.CanSeek && ( logReqLimit == null || ( int ) logReqLimit >= c.Request.ContentLength ) )
                      {
                          var body = c.Request.Body;
                          body.Seek( 0, SeekOrigin.Begin );
                          string buffer = await new StreamReader( body ).ReadToEndAsync();
                          body.Seek( 0, SeekOrigin.Begin );
                          reqBody = buffer;
                      }
                      else
                      {
                          reqBody = "Could not read request body: Request reading disabled or content too long";
                      }
                      var requestUrl = c.Request.Path + c.Request.QueryString;
                      var appLogger = loggerFactory.CreateLogger( apiName );
                      string reqLog = "\n------------------------------------------------------\n" +
                                      "                     Request Log:\n" +
                                      "------------------------------------------------------\n" +
                                      "Request: " + requestUrl + " \n" +
                                      "Request Method: " + c.Request.Method + "\n" +
                                      "Request Token: " + requestToken + "\n" +
                                      "Request Body: " + reqBody + "\n" +
                                      "------------------------------------------------------\n" +
                                      "                      Exception:\n" +
                                      "------------------------------------------------------";
                      appLogger.LogError( error.Error, reqLog + "\n" + error.Error.Message );
                  }
                  c.Response.ContentType = "application/json";
                  string errorMessage = JsonSerializer.Serialize( new ErrorOut( ( HttpStatusCode ) c.Response.StatusCode, error.Error.Message, false, ResponseErrorType.Exception, "Exception" ) );
                  await c.Response.WriteAsync( errorMessage );
              } ) );

            return app;
        }
    }
}
