namespace KRFCommon.Controller
{
    using KRFCommon.Context;

    using Microsoft.AspNetCore.Authorization;

    public class KRFUserAuthorizeAttribute : AuthorizeAttribute
    {
        public KRFUserAuthorizeAttribute(): base( Policies.User )
        { }
    }
}
