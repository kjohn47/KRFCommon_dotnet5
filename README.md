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
Dependency: 
AddKRFController -> configures json serializer and controller on services.AddController()
```
  services.AddKRFController()
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
- Allow request to be readed/ enable request seek
* app.UseMiddleware<KRFBodyRewindMiddleware>( BufferSize? or null );

- Log Request and response on json communication: (already includes logic to enable seek of request so no need for KRFBodyRewindMiddleware when using)
* app.UseMiddleware<KRFLogRequestResponseMiddleware>( loggerFactory, apiName, tokenIdentifier, reqBufferSize? or null );
```

- exception handler middleware
```
Log Request/Response and Exceptions: -> This will enable app.UseMiddleware<KRFLogRequestResponseMiddleware>( loggerFactory, apiName, tokenIdentifier, reqBufferSize? or null ) on enableReadRequest = true

app.KRFLogAndExceptionHandlerConfigure(
                    ILoggerFactory loggerFactory, 
                    bool logErrors, 
                    string apiName, 
                    string tokenIdentifier, 
                    bool enableReadRequest, 
                    int? reqBufferSize = null )


Log user request and exception to system events: -> This will activate midleware app.UseMiddleware<KRFBodyRewindMiddleware>( BufferSize ) on enableReadRequest = true

app.KRFExceptionHandlerMiddlewareConfigure( 
                    ILoggerFactory loggerFactory, 
                    bool logErrors, 
                    string apiName, 
                    string tokenIdentifier, 
                    bool enableReadRequest, 
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
 add to services: -> services.AddKRFMemoryCache( IConfiguration GetSection(KRFMemoryCacheSettings) ); 
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

 - KRF Loggers
 * Log To File Provider
  ```
  services.AddLogging( l =>
            {
              a) l.AddKRFLogToFileLogger( IConfiguration GetSection() );
              b) l.AddKRFLogToFileLogger( IConfiguration GetSection( KRFApiSettings.KRFLoggerConfig_Key ) );
              c) l.AddKRFLogToFileLogger( IConfiguration GetSection( x => {
                x is Action<KRFLogToFileLoggerOptions>
              } ) );
            }
  Settings:
    "KRFFileLogger": {
    "Path": null, -> Path to log files, null to log on api/Logs, Relative path will log on api\Path, absolute path will log on Path\
    "FilterLogLevelArray": [ "Information", "Warning", "Trace", "Critical", "Error" ], -> Array of loglevels to be logged that are not from middleware. (None and Debug are not available)
    "DisableApiLogs": false, -> false or null will log api specific logs on Api folder, true to skip. Logs from Middleware.
    "DisableSQLLogs": false -> false or null will log EF specific logs on SQL folder, true to skip. Logs from Entity Framework.
  }

  Each log origin has it's own folder and are created at start of app. There is a log folder for each loglevel, one for Api and one for SQL.
  Files are created for each day and named after current date.
  Api logs will have one file for each middleware used and current date
  ```


 * LogToEventLogApi
 NOT IMPLEMENTED



* Service Proxy - External Services
TODO: Common service handler for swagger generated proxy

KRFRestHandler handler for calling rest api
```
Task<KRFHttpResponse<TResp>> RequestHttp<TResp>( KRFHttpRequest request ) -> use this for when no body needs to be sent

Task<KRFHttpResponse<TResp>> RequestHttp<TBody, TResp>( KRFHttpRequestWithBody<TBody> request ) -> use this for when body needs to be sent (POST/PUT only)

```

Configuration -> KRFExternalServices
```
  "KRFExternalServices": {
    "ServerList": {
      "ServerImplTypeName" : {
        "ServerUrl" : "https://server.url:port",
        "CertificatePath" : "string", (to decide implementation)
        "CertificateKey" : "SomeStringKey",
        "TokenIdentifier" : "string - for sending krf token on header, use on krf api's",
        "TokenKey" : "string - signature key expected by krf api token",
        "Timeout" : null or number,
        "ForceDisableSSL" : null or boolean
      }
    }
  }
```

Session api / auth api
** CheckSessionResult -> class to be used as response from CheckSessionAlive Command (implements ICommandResponse)

JSON
KRFJsonSerializerOptions -> common json serializer settings used on all api's
```
  KRFJsonSerializerOptions.GetJsonSerializerOptions()
```