namespace KRFCommon.Context
{
    using System.Net;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;

    using KRFCommon.CQRS.Common;
    using KRFCommon.Constants;
    using KRFCommon.JSON;

    public static class AuthConfigureHelper
    {
        public static IApplicationBuilder AuthConfigure( this IApplicationBuilder app, bool hideErrorMessage = true )
        {
            app.UseStatusCodePages( async ctx =>
             {
                 var serializerOpt = KRFJsonSerializerOptions.GetJsonSerializerOptions();

                 switch ( ctx.HttpContext.Response.StatusCode )
                 {
                     case ( ( int ) HttpStatusCode.Unauthorized ):
                     {
                         string message = "Selected token is invalid or user is not authenticated";
                         string code = null;
                         if ( ctx.HttpContext.Response.Headers.TryGetValue( KRFConstants.AuthenticateHeader, out var authHeader ) )
                         {
                             code = authHeader[ 0 ];
                             switch ( code )
                             {
                                 case KRFTokenErrors.TokenSignatureErrorCode:
                                 {
                                     message = "Invalid token signature, please logout and login again";
                                     break;
                                 }
                                 case KRFTokenErrors.TokenExpiredErrorCode:
                                 {
                                     message = "Your token has expired, request new token from auth server and send new request";
                                     break;
                                 }
                                 default:
                                 {
                                     code = KRFTokenErrors.TokenDefaultErrorCode;
                                     break;
                                 }
                             }
                         }

                         if ( hideErrorMessage )
                         {
                             message = "User is not authenticated";
                         }

                         await ctx.HttpContext.Response.WriteAsJsonAsync( new ErrorOut( HttpStatusCode.Unauthorized, message, ResponseErrorType.Application, "Authentication", code ), serializerOpt );
                         break;
                     }
                     case ( ( int ) HttpStatusCode.Forbidden ):
                     {
                         await ctx.HttpContext.Response.WriteAsJsonAsync( new ErrorOut( HttpStatusCode.Forbidden, "User is not allowed to resource", ResponseErrorType.Application, "Authorization", KRFConstants.AuthorizationErrorCode ), serializerOpt );
                         break;
                     }
                     case ( ( int ) HttpStatusCode.NotFound ):
                     {
                         await ctx.HttpContext.Response.WriteAsJsonAsync( new ErrorOut( HttpStatusCode.NotFound, "Could not find desired resource", ResponseErrorType.Application, KRFConstants.DefaultErrorCode ), serializerOpt );
                         break;
                     }
                     default:
                     {
                         await ctx.HttpContext.Response.WriteAsJsonAsync( new ErrorOut( ( HttpStatusCode ) ctx.HttpContext.Response.StatusCode, "Unknown error occurred", ResponseErrorType.Unknown, KRFConstants.DefaultErrorCode ), serializerOpt );
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
