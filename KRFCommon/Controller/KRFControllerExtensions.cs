namespace KRFCommon.Controller
{
    using KRFCommon.JSON;

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
    }
}
