namespace KRFCommon.Controller
{
    using KRFCommon.Context;

    using Microsoft.AspNetCore.Authorization;

    public class KRFAdminAuthorizeAttribute : AuthorizeAttribute
    {
        public KRFAdminAuthorizeAttribute(): base( Policies.Admin )
        { }
    }
}
