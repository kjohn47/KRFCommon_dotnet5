using Microsoft.AspNetCore.Builder;

namespace KRFCommon.Context
{
    public static class AuthConfigure
    {
        public static void Configure(IApplicationBuilder app )
        {
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthentication();

            app.UseAuthorization();
        }
    }
}
