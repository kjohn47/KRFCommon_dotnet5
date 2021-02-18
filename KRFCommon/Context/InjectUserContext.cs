namespace KRFCommon.Context
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    using KRFCommon.Constants;
    using KRFCommon.CQRS.Common;
    using KRFCommon.JSON;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;

    public static class InjectUserContextHelper
    {
        public static IServiceCollection InjectUserContext( this IServiceCollection services, string tokenIdentifier, string key )
        {
            if ( string.IsNullOrEmpty( tokenIdentifier ) )
            {
                throw new Exception( "Missing token identifier setting" );
            }

            if ( string.IsNullOrEmpty( key ) )
            {
                throw new Exception( "Missing token key setting" );
            }

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ITokenProvider, TokenProvider>( s => new TokenProvider( s.GetService<IHttpContextAccessor>(), tokenIdentifier ) );
            services.AddScoped<IUserContext, UserContext>( s => new UserContext( s.GetService<ITokenProvider>(), key ) );
            services.AddAuthentication( o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            } )
            .AddJwtBearer( x =>
             {
                 x.IncludeErrorDetails = true;
                 x.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( key ) ),
                     ValidateIssuer = false,
                     ValidateAudience = false,
                     ValidateLifetime = false
                 };
                 x.Events = new JwtBearerEvents
                 {
                     OnMessageReceived = ctx =>
                     {
                         if ( ctx.Request.Headers.ContainsKey( tokenIdentifier ) )
                         {
                             var bearerToken = ctx.Request.Headers[ tokenIdentifier ].ElementAt( 0 );
                             var token = bearerToken.StartsWith( KRFJwtConstants.Bearer, StringComparison.OrdinalIgnoreCase ) ? bearerToken.Substring( 7 ) : bearerToken;
                             ctx.Token = token;
                         }
                         else
                         {
                             ctx.NoResult();
                         }
                         return Task.CompletedTask;
                     },
                     OnTokenValidated = ctx =>
                     {
                         var jwtToken = ( System.IdentityModel.Tokens.Jwt.JwtSecurityToken ) ctx.SecurityToken;
                         var isAdmin = jwtToken.Claims.FirstOrDefault( x => x.Type.Equals( KRFJwtConstants.IsAdmin, StringComparison.OrdinalIgnoreCase ) )?.Value.Equals( "true", StringComparison.OrdinalIgnoreCase );
                         var claims = new List<System.Security.Claims.Claim>();
                         if ( isAdmin != null && isAdmin == true )
                         {
                             claims.Add( new System.Security.Claims.Claim( KRFConstants.UserRoleClaim, Claims.Admin.ToString() ) );
                         }
                         else
                         {
                             claims.Add( new System.Security.Claims.Claim( KRFConstants.UserRoleClaim, Claims.User.ToString() ) );
                         }
                         var appIdentity = new System.Security.Claims.ClaimsIdentity( claims );
                         ctx.Principal.AddIdentity( appIdentity );
                         return Task.CompletedTask;
                     }
                 };
             } );
            services.AddAuthorization( options =>
             {
                 options.AddPolicy( Policies.Admin, policy =>
                 {
                     policy.RequireClaim( KRFConstants.UserRoleClaim, Claims.Admin.ToString() );
                 } );
                 options.AddPolicy( Policies.User, policy =>
                 {
                     policy.RequireClaim( KRFConstants.UserRoleClaim, Claims.User.ToString(), Claims.Admin.ToString() );
                 } );
             } );

            return services;
        }
    }
}
