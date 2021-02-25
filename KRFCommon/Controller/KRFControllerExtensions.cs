namespace KRFCommon.Controller
{
    using System;

    using KRFCommon.JSON;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;

    public static class KRFControllerExtensions
    {
        public static IServiceCollection AddKRFController( this IServiceCollection services )
        {
            services.AddControllers( o =>
            {
                o.AllowEmptyInputInBodyModelBinding = true;
                o.RespectBrowserAcceptHeader = true;
                o.ReturnHttpNotAcceptable = false;
            } )
            .AddJsonOptions( o => KRFJsonSerializerOptions.GetJsonSerializerOptions( o.JsonSerializerOptions ) );

            return services;
        }

        public static IServiceCollection AddKRFController( this IServiceCollection services, Action<MvcOptions> options )
        {
            services.AddControllers( options )
            .AddJsonOptions( o => KRFJsonSerializerOptions.GetJsonSerializerOptions( o.JsonSerializerOptions ) );

            return services;
        }
    }
}
