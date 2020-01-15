using KRFCommon.CQRS.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;

namespace KRFCommon.Context
{
    public static class AuthConfigure
    {
        public static void Configure(IApplicationBuilder app)
        {
            app.UseStatusCodePages(async ctx =>
            {
                switch (ctx.HttpContext.Response.StatusCode)
                {
                    case ((int)HttpStatusCode.Unauthorized):
                        {
                            ctx.HttpContext.Response.ContentType = "plain/text";
                            string errorMessage = JsonConvert.SerializeObject(new ErrorOut(HttpStatusCode.Unauthorized, "User is not authenticated", false, "Authentication"));
                            await ctx.HttpContext.Response.WriteAsync(errorMessage);
                            break;
                        }
                    case ((int)HttpStatusCode.Forbidden):
                        {
                            ctx.HttpContext.Response.ContentType = "plain/text";
                            string errorMessage = JsonConvert.SerializeObject(new ErrorOut(HttpStatusCode.Forbidden, "User is not allowed to resource", false, "Authorization"));
                            await ctx.HttpContext.Response.WriteAsync(errorMessage);
                            break;
                        }
                }
            });

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthentication();

            app.UseAuthorization();
            
        }
    }
}
