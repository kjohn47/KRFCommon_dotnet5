namespace KRFCommon.Context
{
    using System.Linq;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;

    public class TokenProvider : ITokenProvider
    {
        public TokenProvider( IHttpContextAccessor httpContextAccessor, string tokenIdentifier )
        {
            this.Token = "";
            var httpContext = httpContextAccessor.HttpContext;
            StringValues tokenData;
            httpContext.Request.Headers.TryGetValue( tokenIdentifier, out tokenData );
            if ( tokenData.Count > 0 )
            {
                this.Token = tokenData.First();
            }
        }
        public string Token { get; set; }
    }
}
