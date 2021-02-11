namespace KRFCommon.Database
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Data.SqlClient;
    using System.IO;

    public static class KRFDbContextInjectHelper
    {
        public static IServiceCollection InjectDBContext<TDBContext>( this IServiceCollection services, KRFDatabases dbSettings )
            where TDBContext : DbContext
        {
            if ( dbSettings == null || dbSettings.Databases == null )
            {
                throw new Exception( "Missing database settings" );
            }

            KRFDatabaseConfig settings = null;
            var contextName = typeof( TDBContext ).Name;

            if ( !dbSettings.Databases.TryGetValue( contextName, out settings ) )
            {
                throw new Exception( string.Format("Missing settings for {0}", contextName ) );
            }

            if ( settings == null || string.IsNullOrEmpty( settings.ConnectionString ) )
            {
                throw new Exception( "Missing connection string or database settings" );
            }

            var connectionString = settings.ConnectionString;

            if ( settings.UseLocalDB.HasValue || !string.IsNullOrEmpty( settings.ApiDBFolder ) )
            {
                var builder = new SqlConnectionStringBuilder( settings.ConnectionString );
                var currentPath = builder.AttachDBFilename;

                //validate settings
                if ( string.IsNullOrEmpty( currentPath ) )
                {
                    throw new Exception( "Missing AttachDBFilename relative path to db file .mdf on connection string" );
                }

                if ( !currentPath.EndsWith( ".mdf" ) )
                {
                    throw new Exception( "Wrong file type selected on AttachDBFilename path. use .mdf extension" );
                }

                if ( Path.IsPathRooted( currentPath ) )
                {
                    throw new Exception( "Path selected to AttachDBFilename is not a relative path" );
                }

                if ( settings.UseLocalDB.HasValue && settings.UseLocalDB.Value )
                {
                    //add Startup Project location to connection string relative path
                    builder.AttachDBFilename = string.Format( "{0}\\{1}", Environment.CurrentDirectory, currentPath );
                }
                else if ( !string.IsNullOrEmpty( settings.ApiDBFolder ) )
                {
                    //add apiDB path to connection string relative path
                    if ( !Path.IsPathRooted( settings.ApiDBFolder ) )
                    {
                        throw new Exception( "Path selected to ApiDBFolder setting is not a valid absolute path" );
                    }
                    builder.AttachDBFilename = string.Format( "{0}\\{1}", settings.ApiDBFolder, currentPath );
                }

                connectionString = builder.ConnectionString;
            }

            services.AddDbContext<TDBContext>( opt =>
             {
                 opt.UseSqlServer( connectionString, x =>
                     {
                         if ( !string.IsNullOrEmpty( dbSettings.MigrationAssembly ) )
                             x.MigrationsAssembly( dbSettings.MigrationAssembly );
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
