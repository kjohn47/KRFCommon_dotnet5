namespace KRFCommon.Database
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.EntityFrameworkCore;

    public static class KRFDbContextInjectHelper
    {
        public static IServiceCollection InjectDBContext<TDBContext>( this IServiceCollection services, KRFDatabaseConfig settings, string migrationAssembly )
            where TDBContext : DbContext
        {
            var connectionString = settings.ConnectionString;
            services.AddDbContext<TDBContext>( opt =>
             {
                 opt.UseSqlServer( settings.UseLocalDB.HasValue && settings.UseLocalDB.Value && !string.IsNullOrEmpty( settings.ApiDBFolder )
                     ? connectionString.Replace( settings.ApiDBFolder, string.Concat( Environment.CurrentDirectory, "\\", settings.ApiDBFolder ) )
                     : connectionString, x =>
                     {
                         if ( !string.IsNullOrEmpty( migrationAssembly ) )
                             x.MigrationsAssembly( migrationAssembly );
                     } );
             } );

            return services;
        }

        public static IServiceScope ConfigureAutomaticMigrations<TDBContext>( this IServiceScope serviceScope )
            where TDBContext : DbContext
        {
            serviceScope.ServiceProvider.GetService<TDBContext>().Database.Migrate();

            return serviceScope;
        }
    }
}
