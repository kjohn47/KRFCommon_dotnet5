using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
                            await ctx.HttpContext.Response.WriteAsync("User is not authenticated");
                            break;
                        }
                    case ((int)HttpStatusCode.Forbidden):
                        {
                            await ctx.HttpContext.Response.WriteAsync("User is not allowed to resource");
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
