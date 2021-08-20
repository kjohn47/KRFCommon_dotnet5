using System;
using System.Linq;

using KRFCommon.Constants;

using Microsoft.AspNetCore.Http;

namespace KRFCommon.Context
{
    public class UserContextBuilder : IUserContextBuilder
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserContextBuilder( IHttpContextAccessor httpContextAccessor )
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public IUserContext GetContext()
        {
            var context = new UserContext();

            var user = httpContextAccessor?.HttpContext?.User;
            context.Claim = Claims.NotLogged;

            if ( user.Identity != null && user.Identity.IsAuthenticated && user.Claims != null && user.Claims.Any() )
            {
                if ( Enum.TryParse<Claims>( user.FindFirst( KRFConstants.UserRoleClaim )?.Value, true, out var claim ) )
                {
                    if ( !claim.Equals( Claims.NotLogged ) )
                    {
                        context.Claim = claim;
                        try { context.UserId = new Guid( user.FindFirst( KRFJwtConstants.UserId )?.Value ); } catch { throw new Exception( "Invalid User ID" ); }
                        try { context.SessionId = new Guid( user.FindFirst( KRFJwtConstants.SessionId )?.Value ); } catch { throw new Exception( "Invalid User Session Id" ); }
                        context.Name = user.FindFirst( KRFJwtConstants.Name )?.Value;
                        context.Surname = user.FindFirst( KRFJwtConstants.Surname )?.Value;
                        context.UserName = user.FindFirst( KRFJwtConstants.UserName )?.Value ?? throw new Exception( "UserName cannot be empty" );
                        if ( user.HasClaim( x => x.Type.Equals( KRFJwtConstants.ExpireTicks, StringComparison.InvariantCultureIgnoreCase ) ) )
                        {
                            var expireValue = user.FindFirst( KRFJwtConstants.ExpireTicks ).Value;
                            if ( long.TryParse( expireValue, out var seconds ) )
                            {
                                context.TokenExpiration = DateTimeOffset.UnixEpoch.DateTime.AddSeconds( seconds );
                            }
                        }
                    }
                }
            }

            return context;
        }
    }
}
