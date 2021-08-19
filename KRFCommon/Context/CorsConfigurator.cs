namespace KRFCommon.Context
{
    using KRFCommon.Api;

    using Microsoft.AspNetCore.Builder;

    static class CorsConfigurator
    {
        public static IApplicationBuilder CorsConfigure( this IApplicationBuilder app, CorsConfiguration configuration, bool isDev )
        {
            if ( configuration == null )
            {
                if ( isDev )
                {
                    app.UseCors( x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader() );
                }

                return app;
            }

            app.UseCors( x =>
             {
                 if ( configuration.AllowAnyOrigin )
                 {
                     x.AllowAnyOrigin();
                 }
                 else
                 {
                     x.WithOrigins( configuration.AllowedOrigins );
                 }

                 if ( configuration.AllowAnyHeader )
                 {
                     x.AllowAnyHeader();
                 }
                 else
                 {
                     x.WithHeaders( configuration.AllowedHeaders );
                 }

                 if ( configuration.AllowAnyMethod )
                 {
                     x.AllowAnyMethod();
                 }
                 else
                 {
                     x.WithMethods( configuration.AllowedMethods );
                 }
             } );

            return app;
        }
    }
}
