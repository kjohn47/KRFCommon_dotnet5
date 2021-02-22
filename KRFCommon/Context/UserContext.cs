namespace KRFCommon.Context
{
    using System;
    using System.Linq;

    using KRFCommon.Constants;

    using Microsoft.AspNetCore.Http;

    public class UserContext : IUserContext
    {
        public UserContext( IHttpContextAccessor httpContextAccessor )
        {
            var user = httpContextAccessor?.HttpContext?.User;
            this.Claim = Claims.NotLogged;

            if ( user.Identity.IsAuthenticated && user.Claims != null && user.Claims.Count() > 0 )
            {
                try { this.UserId = new Guid( user.FindFirst( KRFJwtConstants.UserId )?.Value ); } catch { throw new Exception( "Invalid User ID" ); }
                try { this.SessionId = new Guid( user.FindFirst( KRFJwtConstants.SessionId )?.Value ); } catch { throw new Exception( "Invalid User Session Id" ); }
                this.Name = user.FindFirst( KRFJwtConstants.Name )?.Value;
                this.Surname = user.FindFirst( KRFJwtConstants.Surname )?.Value;
                this.UserName = user.FindFirst( KRFJwtConstants.UserName )?.Value ?? throw new Exception( "UserName cannot be empty" );
                if( Enum.TryParse<Claims>( user.FindFirst( KRFConstants.UserRoleClaim )?.Value, true, out var claim ) )
                {
                    this.Claim = claim;
                }
            }
        }

        public Guid UserId { get; set; }
        public Guid SessionId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public Claims Claim { get; set; }
    }
}
