namespace KRFCommon.Context
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using KRFCommon.Constants;

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
                     OnAuthenticationFailed = ctx =>
                     {
                         if ( ctx.Exception is SecurityTokenInvalidSignatureException )
                         {
                             throw new SecurityTokenInvalidSignatureException( "Invalid token signature, authentication rejected" );
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
                     policy.RequireClaim( KRFConstants.UserRoleClaim, Claims.Admin.ToString() );
                 } );
                 options.AddPolicy( Policies.User, policy =>
                 {
                     policy.RequireClaim( KRFConstants.UserRoleClaim, Claims.User.ToString(), Claims.Admin.ToString() );
                 } );
             } );

            services.AddScoped<IUserContext, UserContext>( x =>
            {
                var ctx = new UserContext( x.GetService<IHttpContextAccessor>() );

                if ( ctx.Claim.Equals( Claims.NotLogged ) )
                {
                    return null;
                }

                return ctx;
            } );

            return services;
        }
    }
}
