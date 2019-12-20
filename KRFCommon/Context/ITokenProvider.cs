using Microsoft.AspNetCore.Http;

namespace KRFCommon.Context
{
    public interface ITokenProvider
    {
        public string Token { get; set; }
    }
}
