using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRFCommon.Context
{
    public static class InjectUserContext
    {
        public static void InjectContext( IServiceCollection services, string tokenIdentifier, string key )
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ITokenProvider, TokenProvider>(s => new TokenProvider( s.GetService<IHttpContextAccessor>(), tokenIdentifier ) );
            services.AddScoped<IUserContext, UserContext>( s => new UserContext( s.GetService<ITokenProvider>(), key ));
            services.AddAuthentication(o => {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.IncludeErrorDetails = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes( key )),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false
                };
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = ctx =>
                    {
                        if (ctx.Request.Headers.ContainsKey(tokenIdentifier))
                        {
                            var bearerToken = ctx.Request.Headers[tokenIdentifier].ElementAt(0);
                            var token = bearerToken.StartsWith("Bearer ", System.StringComparison.OrdinalIgnoreCase) ? bearerToken.Substring(7) : bearerToken;
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
                        var jwtToken = (System.IdentityModel.Tokens.Jwt.JwtSecurityToken)ctx.SecurityToken;
                        var isAdmin = jwtToken.Claims.FirstOrDefault(x => x.Type.Equals("isAdmin", System.StringComparison.OrdinalIgnoreCase))?.Value.Equals("true", System.StringComparison.OrdinalIgnoreCase);
                        var claims = new List<System.Security.Claims.Claim>();
                        if (isAdmin != null && isAdmin == true)
                        {
                            claims.Add(new System.Security.Claims.Claim("UserRole", Claims.Admin.ToString()));
                        }
                        else
                        {
                            claims.Add(new System.Security.Claims.Claim("UserRole", Claims.User.ToString()));
                        }
                        var appIdentity = new System.Security.Claims.ClaimsIdentity(claims);
                        ctx.Principal.AddIdentity(appIdentity);
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.Admin, policy =>
                {
                    policy.RequireClaim("UserRole", Claims.Admin.ToString());
                });
                options.AddPolicy(Policies.User, policy =>
                {
                    policy.RequireClaim("UserRole", Claims.User.ToString(), Claims.Admin.ToString());
                });
            });
        }
    }
}
