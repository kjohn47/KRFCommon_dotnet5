using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace KRFCommon.Context
{
    public static class InjectUserContext
    {
        public static void InjectContext( IServiceCollection services )
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ITokenProvider, TokenProvider>();
            services.AddScoped<IUserContext, UserContext>();
        }
    }
}
