namespace KRFCommon.Swagger
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.Extensions.DependencyInjection;

    using Swashbuckle.AspNetCore.SwaggerGen;

    public static class SwaggerInitHelper
    {
        public static IServiceCollection SwaggerInit( this IServiceCollection services, string AppName, string tokenIdentifier )
        {
            if ( string.IsNullOrEmpty( tokenIdentifier ) )
            {
                throw new Exception( "Missing token identifier setting" );
            }

            if ( string.IsNullOrEmpty( AppName ) )
            {
                throw new Exception( "Missing App Name setting" );
            }

            services.AddSwaggerGen( option =>
             {
                 option.AddSecurityDefinition( "Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                 {
                     In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                     Description = "Place Access Bearer JWT Token",
                     Name = tokenIdentifier,
                     Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
                 } );

                 option.AddSecurityRequirement( new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                   {
                       new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                       {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                       },
                       new List<string>()
                   }
                } );

                 option.SwaggerDoc( AppName, new Microsoft.OpenApi.Models.OpenApiInfo
                 {
                     Version = "v1",
                     Title = AppName,
                     Description = string.Format( "{0} API Swagger", AppName )
                 } );

                 option.CustomOperationIds( apiDesc =>
                 {
                     var controllerDesc = apiDesc.ActionDescriptor as ControllerActionDescriptor;
                     var method = string.Format("{0}{1}", apiDesc.HttpMethod.Substring(0, 1), apiDesc.HttpMethod.Substring(1).ToLowerInvariant());

                     if (controllerDesc != null)
                     {
                         return $"{controllerDesc.ControllerName}{controllerDesc.ActionName}_{method}";
                     }

                     return apiDesc.TryGetMethodInfo( out MethodInfo methodInfo ) 
                        ? $"{apiDesc.ActionDescriptor.RouteValues[ "controller" ]}{methodInfo.Name}_{method}" 
                        : $"{apiDesc.ActionDescriptor.RouteValues[ "controller" ]}_{method}";
                 } );
             } );

            return services;
        }

        public static IApplicationBuilder SwaggerConfigure( this IApplicationBuilder app, string AppName )
        {

            app.UseSwagger();

            app.UseSwaggerUI( option => option.SwaggerEndpoint( string.Format( "/swagger/{0}/swagger.json", AppName ), AppName ) );

            return app;
        }
    }
}
