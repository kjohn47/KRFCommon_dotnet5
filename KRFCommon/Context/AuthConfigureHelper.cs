namespace KRFCommon.Context
{
    using System.Net;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;

    using KRFCommon.CQRS.Common;

    public static class AuthConfigureHelper
    {
        public static IApplicationBuilder AuthConfigure( this IApplicationBuilder app )
        {
            app.UseStatusCodePages( async ctx =>
             {
                 switch ( ctx.HttpContext.Response.StatusCode )
                 {
                     case ( ( int ) HttpStatusCode.Unauthorized ):
                     {
                         await ctx.HttpContext.Response.WriteAsJsonAsync( new ErrorOut( HttpStatusCode.Unauthorized, "User is not authenticated", false, ResponseErrorType.Application, "Authentication" ) );
                         break;
                     }
                     case ( ( int ) HttpStatusCode.Forbidden ):
                     {
                         await ctx.HttpContext.Response.WriteAsJsonAsync( new ErrorOut( HttpStatusCode.Forbidden, "User is not allowed to resource", false, ResponseErrorType.Application, "Authorization" ) );
                         break;
                     }
                 }
             } );

            app.UseCors( x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader() );

            app.UseAuthentication();

            app.UseAuthorization();

            return app;
        }
    }
}
