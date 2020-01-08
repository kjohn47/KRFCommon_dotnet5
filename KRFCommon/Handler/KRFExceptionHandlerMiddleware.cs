using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace KRFCommon.Handler
{
    public static class KRFExceptionHandlerMiddleware
    {
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, string apiName, string tokenIdentifier)
        {
            app.UseExceptionHandler(b => b.Run(async c =>
            {
                var error = (IExceptionHandlerFeature)c.Features[typeof(IExceptionHandlerFeature)];
                await c.Response.WriteAsync(error.Error.Message);
                if (env.IsDevelopment())
                {
                    var requestToken = c.Request.Headers[tokenIdentifier];
                    var body = c.Request.Body;
                    var buffer = new byte[Convert.ToInt32(c.Request.ContentLength)];
                    await c.Request.Body.ReadAsync(buffer, 0, buffer.Length);
                    var requestBody = Encoding.UTF8.GetString(buffer);
                    var requestUrl = c.Request.Path + c.Request.QueryString;
                    var appLogger = loggerFactory.CreateLogger(apiName);
                    appLogger.LogError("Request: " + requestUrl);
                    appLogger.LogError("Request Token: " + requestToken);
                    appLogger.LogError("Request Body: " + requestBody);
                    appLogger.LogError(error.Error, error.Error.Message);
                }
            }));
        }
    }
}
