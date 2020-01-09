KRFCommon

Important during service dev:
	install swagger
	Install-Package Swashbuckle.AspNetCore -Version 5.0.0-rc4
	install jwtBearer
	Install-Package Microsoft.AspNetCore.Authentication.JwtBearer -Version 3.1.0
	
This nuget has all common parts to be used on microservices for KJohn React Framework
- user context
- user authorization and authentcation
- query and command abstractions
- sql EF query error
- exception handler middleware
- auth error handler 
- middlewares, 