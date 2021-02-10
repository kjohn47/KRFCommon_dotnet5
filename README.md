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
 -> using KRFCommon.MemoryCache
 -> configuration sample: (constant key for appsettings.json - KRFApiSettings.MemoryCacheSettings_Key)

 "KRFMemoryCacheSettings": {
    "CacheCleanupInterval": {
      "Hours": 0, 
      "Minutes": 30,
      "Seconds": 0
    },
    "CachedKeySettings": {
      "SETTINGS_KEY": {
        "RemoveHours": 0,
        "RemoveMinutes": 60,
        "RemoveSeconds": 0
      }
    }
  }

 configuration Class -> KRFMemoryCacheSettings

 add to services: -> services.AddKRFMemoryCache( KRFMemoryCacheSettings object/null ); 

 if no settings are passed, default values are used: 10 minutes remove refresh and 60 minutes for each cached item.

 * Implementations:

 add dependency to constructor -> IKRFMemoryCache _memoryCache;

 Get cached item or update from async query lambda -> 
 
 T value = await this._memoryCache.GetCachedItemAsync<T>(Key: string, () => Task<T> GetValues() : Func, settings_key: string/null)

 Get cached item or update from query lambda -> 

 T value = this._memoryCache.GetCachedItem<T>(Key: string, () => Task<T> GetValues() : Func, settings_key: string/null)

 If no settings key is defined, key will be used instead to get the settings.

 If there is no settings defined, default 60 min for expiration will be set

 KRFMemoryCache is an extension of MemoryCache