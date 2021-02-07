﻿namespace KRFCommon.Swagger
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;

    public static class SwaggerInitHelper
    {
        public static void SwaggerInit( this IServiceCollection services, string AppName, string tokenIdentifier )
        {
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
             } );
        }

        public static void SwaggerConfigure( this IApplicationBuilder app, string AppName )
        {

            app.UseSwagger();

            app.UseSwaggerUI( option => option.SwaggerEndpoint( string.Format( "/swagger/{0}/swagger.json", AppName ), AppName ) );
        }
    }
}
