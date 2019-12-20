using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Linq;

namespace KRFCommon.Context
{
    public class TokenProvider : ITokenProvider
    {
        public TokenProvider( IHttpContextAccessor httpContextAccessor)
        {
            this.Token = "";
            var httpContext = httpContextAccessor.HttpContext;
            StringValues tokenData;
            httpContext.Request.Headers.TryGetValue("AccessToken", out tokenData);
            if( tokenData.Count > 0 )
            {
                this.Token = tokenData.First();
            }
        }
        public string Token { get; set; }
    }
}
