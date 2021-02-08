KRFCommon

Important during service dev:
	install swagger
	Install-Package Swashbuckle.AspNetCore -Version 5.0.0-rc4
	install jwtBearer
	Install-Package Microsoft.AspNetCore.Authentication.JwtBearer -Version 3.1.0
	
This nuget has all common parts to be used on microservices for KJohn React Framework


- user context + user authorization and authentcation config
services.InjectUserContext( TokenIdentifier, TokenKey );
app.AuthConfigure();


- query and command abstractions
IQuery<TReq, TResp>
ICommand<TReq, TResp>


- sql EF query error and dependency injection and migration configuration helper
services.InjectDBContext<DBContextType>( KRFDatabaseConfig config, "MigrationAssembly" );
serviceScope.ConfigureAutomaticMigrations<DBContextType>();


- exception handler middleware
app.KRFExceptionHandlerMiddlewareConfigure( loggerFactory, enableLogs, ApiName, TokenIdentifier );


** when using middleware - KRFBodyRewindMiddleware
app.KRFExceptionHandlerMiddlewareConfigure( loggerFactory, enableLogs, ApiName, TokenIdentifier, BufferSize );


- middlewares, 
app.UseMiddleware<KRFBodyRewindMiddleware>( BufferSize, MemBufferOnly );


- swagger
services.SwaggerInit( ApiName, TokenKey );
app.SwaggerConfigure( ApiName );


- in-memory cache
 -> Dependency: services.InjectMemoryCache( configuration ) - using KRFCommon.MemoryCache
 -> configuration will be injected as singleton
 -> you can pass any configuration that extends MemoryCacheSettingsBase
 -> if no configuration is passed, default MemoryCacheSettingsBase is created with cleanup interval of 10 minutes
