KRFCommon package - common parts of KRFApi

Important during service dev:
	install swagger
	Install-Package Swashbuckle.AspNetCore -Version 5.0.0-rc4
	install jwtBearer
	Install-Package Microsoft.AspNetCore.Authentication.JwtBearer -Version 3.1.0
	
This nuget has all common parts to be used on microservices for KJohn React Framework


- user context + user authorization and authentcation config
```
services.InjectUserContext( TokenIdentifier, TokenKey );
app.AuthConfigure();
```

- query and command abstractions
```
YourQuery IQuery<TReq, TResp>
where TReq is IQueryRequest
where TResp is IQueryResponse -> ICQRSResponse

Query method:
-> Task<IResponseOut<Toutput>> QueryAsync( TReq request )

IResponseOut<T>
- T Result
- ErrorOut Error

ErrorOut - error type that is handled by api

YourCommand ICommand<TReq, TResp>
where TReq is ICommandRequest
where TResp is ICommandResponse -> ICQRSResponse

Command methods:
-> Task<ICommandValidationError> ExecuteValidationAsync( TReq request );
-> Task<IResponseOut<TResp>> ExecuteCommandAsync( TReq request );

Validator (fluentValidator)
YourValidator : KRFValidator<TReq>, IKRFValidator<TReq>

result = IKRFValidator<TReq> validator -> await validator.CheckValidationAsync( TReq request );
Will retun output for command ExecuteValidationAsync
Will return output of type: ICommandValidationError -> ErrorOut GetError();

```

- Common controller:
```
 YourController : KRFController

 Implements:
 async Task<IActionResult> ExecuteAsyncQuery<Tinput, Toutput>( Tinput request, IQuery<Tinput, Toutput> query )
 async Task<IActionResult> ExecuteAsyncCommand<Tinput, Toutput>( Tinput request, ICommand<Tinput, Toutput> command, Func<Toutput, IActionResult> changeAction = null )

```


- sql EF query error and dependency injection and migration configuration helper
```
On KRFApi from template, use App -> Injection -> AppDBContextInjection
services.InjectDBContext<DBContextType>( KRFDatabases config );

serviceScope.ConfigureAutomaticMigrations<DBContextType>();

KRFDatabases object includes settings for configuring the context

"KRFDatabases": {
    "Databases": {
      "DBContextType": {
        "ConnectionString": "Server=(localdb)\\mssqllocaldb;Database=DBContextDB;AttachDbFilename=DBFiles\\DBContextDB.mdf;Trusted_Connection=True;MultipleActiveResultSets=true",
        "UseLocalDB": true,
        "ApiDBFolder": ""
      }
    },
    "EnableAutomaticMigration": true,
    "MigrationAssembly": "KRFTemplateApi.Infrastructure"
  }

Databases -> dictionary of settings for context
"DBContextType" -> Key with same name as context class
"ConnectionString" -> MS SQL connection string
"UseLocalDB": set to true to auto complete AttachDbFilename with api (WebApi) path (connection string must have valid relative path). Send as null to use your own connection string
"ApiDBFolder": set absolute path for database file (UseLocalDB = false to enable this) - (connection string must have valid relative path or filename)

"EnableAutomaticMigration" -> flag can be used on KRFApi to prevent Migration -> ConfigureAutomaticMigrations
"MigrationAssembly" -> Project namespace where migrations will be generated -> recomended Infrastructure


DatabaseQueryHandlerMethod : QueryCommand
When the return result is just to express success or error, like additions or updates to database
```

- middlewares, 
```
- Allow request to be readed again by api for logs
* app.UseMiddleware<KRFBodyRewindMiddleware>( BufferSize, MemBufferOnly );
```

- exception handler middleware
```
KRFExceptionHandlerMiddleware:

This will activate midleware app.UseMiddleware<KRFBodyRewindMiddleware>( BufferSize, MemBufferOnly ) on enableReadRequest = true

Log user request and exception to system events:

app.KRFExceptionHandlerMiddlewareConfigure( 
                    ILoggerFactory loggerFactory, 
                    bool logErrors, 
                    string apiName, 
                    string tokenIdentifier, 
                    bool enableReadRequest, 
                    bool reqBufferOnly, 
                    int? reqBufferSize = null )

Log exception to system events:

app.KRFExceptionHandlerMiddlewareConfigure( ILoggerFactory loggerFactory, bool logErrors, string apiName, string tokenIdentifier );

```

- swagger
```
services.SwaggerInit( ApiName, TokenKey );
app.SwaggerConfigure( ApiName );
```


- In-memory cache
```
 -> using KRFCommon.MemoryCache
```

 -> configuration sample: (constant key for appsettings.json - KRFApiSettings.MemoryCacheSettings_Key)
 
```
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
```
```
 configuration Class -> KRFMemoryCacheSettings
```
```
 add to services: -> services.AddKRFMemoryCache( KRFMemoryCacheSettings object/null ); 
```
 if no settings are passed, default values are used: 10 minutes remove refresh and 60 minutes for each cached item.

 * Implementations:
```
 add dependency to constructor -> IKRFMemoryCache _memoryCache;
```

 Get cached item or update from query lambda -> 
```
this._memoryCache...

--Base
 T value = GetCachedItem<T>( string key, Func<T> queryFunc )
 T value = GetCachedItem<T>( string key, Func<T> queryFunc, string settingsKey )
 T value = GetCachedItem<T>( string key, Func<T> queryFunc, bool preventCacheUpdate )
 T value = GetCachedItem<T>( string key, Func<T> queryFunc, string settingsKey, bool preventCacheUpdate )

--Return Value with cache miss flag KRFCacheResult<T>

 KRFCacheResult<T> value = GetCachedItemWithMissReturn<T>( string key, Func<T> queryFunc )
 KRFCacheResult<T> value = GetCachedItemWithMissReturn<T>( string key, Func<T> queryFunc, string settingsKey )
 KRFCacheResult<T> value = GetCachedItemWithMissReturn<T>( string key, Func<T> queryFunc, bool preventCacheUpdate )
 KRFCacheResult<T> value = GetCachedItemWithMissReturn<T>( string key, Func<T> queryFunc, string settingsKey, bool preventCacheUpdate )

 --Pass handler on method
 T value = GetCachedItemWithHandler<T>( string key, Func<T> queryFunc, Func<KRFCacheResult<T>, T> handler )
 T value = GetCachedItemWithHandler<T>( string key, Func<T> queryFunc, string settingsKey, Func<KRFCacheResult<T>, T> handler )
 T value = GetCachedItemWithHandler<T>( string key, Func<T> queryFunc, bool preventCacheUpdate, Func<KRFCacheResult<T>, T> handler )
 T value = GetCachedItemWithHandler<T>( string key, Func<T> queryFunc, string settingsKey, bool preventCacheUpdate, Func<KRFCacheResult<T>, T> handler )

```

All of the above have async Task<T> version, that terminates with Async -> 
```
 T value = await this._memoryCache.GetCachedItemAsync<T>(Key: string, () => Task<T> GetValues() : Func)
```

 KRFMemoryCache is an extension of MemoryCache