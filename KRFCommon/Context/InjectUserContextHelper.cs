namespace KRFCommon.Context
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Net;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    using KRFCommon.Api;
    using KRFCommon.Constants;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.IdentityModel.Tokens;

    public static class InjectUserContextHelper
    {
        public static IServiceCollection AddUserBearerContext( this IServiceCollection services, IConfiguration configuration )
        {
            var appConfiguration = configuration.GetSection( KRFApiSettings.AppConfiguration_Key ).Get<AppConfiguration>() ?? null;
            return services.AddUserBearerContext( appConfiguration );
        }

        public static IServiceCollection AddUserBearerContext( this IServiceCollection services, AppConfiguration configuration )
        {
            if ( configuration == null )
            {
                throw new Exception( "Missing configuration" );
            }

            if ( string.IsNullOrEmpty( configuration.TokenIdentifier ) )
            {
                throw new Exception( "Missing token identifier setting" );
            }

            if ( string.IsNullOrEmpty( configuration.TokenKey ) )
            {
                throw new Exception( "Missing token key setting" );
            }

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAuthentication( o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            } )
            .AddJwtBearer( x =>
             {
                 x.IncludeErrorDetails = true;
                 x.SaveToken = true;
                 x.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( configuration.TokenKey ) ),
                     ValidateIssuer = configuration.HasIssuer,
                     ValidateAudience = configuration.HasAudience,
                     ValidateLifetime = configuration.TokenValidateLife,
                     ValidIssuers = configuration.GetMultipleIssuers,
                     ValidIssuer = configuration.GetSingleIssuer,
                     ValidAudiences = configuration.GetMultipleAudiences,
                     ValidAudience = configuration.GetSingleAudience,
                 };
                 x.Events = new JwtBearerEvents
                 {
                     OnMessageReceived = ctx =>
                     {
                         if ( ctx.Request.Headers.ContainsKey( configuration.TokenIdentifier ) )
                         {
                             var bearerToken = ctx.Request.Headers[ configuration.TokenIdentifier ].ElementAt( 0 );
                             var token = bearerToken.StartsWith( KRFJwtConstants.BearerSpaced, StringComparison.OrdinalIgnoreCase ) ? bearerToken.Substring( 7 ) : bearerToken;
                             ctx.Token = token;
                         }
                         else if ( configuration.AllowAnonymousOnAuthorizeWithoutPolicy )
                         {
                             ctx.Principal = new ClaimsPrincipal();
                             ctx.Principal.AddIdentity( new ClaimsIdentity( KRFJwtConstants.Bearer, KRFConstants.AnonymousIdentity, KRFConstants.AnonymousIdentity ) );
                             ctx.Success();
                         }
                         else
                         {
                             ctx.NoResult();
                         }
                         return Task.CompletedTask;
                     },
                     OnChallenge = ctx =>
                     {
                         if ( ctx.AuthenticateFailure != null )
                         {
                             ctx.HandleResponse();

                             ctx.Response.StatusCode = ( int ) HttpStatusCode.Unauthorized;
                             if ( ctx.AuthenticateFailure is SecurityTokenInvalidSignatureException )
                             {
                                 ctx.Response.Headers.Append( KRFConstants.AuthenticateHeader, KRFTokenErrors.TokenSignatureErrorCode );
                             }
                             else if ( ctx.AuthenticateFailure is SecurityTokenExpiredException || ctx.AuthenticateFailure is SecurityTokenNoExpirationException )
                             {
                                 ctx.Response.Headers.Append( KRFConstants.AuthenticateHeader, KRFTokenErrors.TokenExpiredErrorCode );
                             }
                             else
                             {
                                 ctx.Response.Headers.Append( KRFConstants.AuthenticateHeader, KRFTokenErrors.TokenDefaultErrorCode );
                             }
                         }
                         return Task.CompletedTask;
                     },
                     OnTokenValidated = ctx =>
                     {
                         var currentIdentity = ctx.Principal.Identities.FirstOrDefault();
                         if ( currentIdentity != null )
                         {
                             currentIdentity.AddClaim( KRFJwt.GetUserRoleClaim( ( ctx.SecurityToken as JwtSecurityToken ).Claims ) );
                         }
                         return Task.CompletedTask;
                     }
                 };
             } );
            services.AddAuthorization( options =>
            {
                options.AddPolicy( Policies.Admin, policy =>
                {
                    policy.RequireClaim( KRFConstants.UserRoleClaim, nameof( Claims.Admin ) );
                } );
                options.AddPolicy( Policies.User, policy =>
                {
                    policy.RequireClaim( KRFConstants.UserRoleClaim, nameof( Claims.User ), nameof( Claims.Admin ) );
                } );
            } );

            services.AddScoped<IUserContext, UserContext>();

            return services;
        }
    }
}
