namespace KRFCommon.Context
{     
    using System.Net;
    using System.Text.Json;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;

    using KRFCommon.CQRS.Common;

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
                            ctx.HttpContext.Response.ContentType = "application/json";
                            string errorMessage = JsonSerializer.Serialize(new ErrorOut(HttpStatusCode.Unauthorized, "User is not authenticated", false, ResponseErrorType.Application, "Authentication"));
                            await ctx.HttpContext.Response.WriteAsync(errorMessage);
                            break;
                        }
                    case ((int)HttpStatusCode.Forbidden):
                        {
                            ctx.HttpContext.Response.ContentType = "application/json";
                            string errorMessage = JsonSerializer.Serialize(new ErrorOut(HttpStatusCode.Forbidden, "User is not allowed to resource", false, ResponseErrorType.Application, "Authorization"));
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
