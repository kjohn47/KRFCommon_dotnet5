namespace KRFCommon.Context
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;

    using KRFCommon.Constants;

    using Microsoft.IdentityModel.Tokens;

    public static class KRFJwt
    {
        public static string GetSignedBearerTokenFromContext( IUserContext context, string signKey, DateTime? expiration = null )
        {
            var handler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( signKey ) );
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity( new[]
                {
                    new Claim( KRFJwtConstants.Name, context.Name?? string.Empty ),
                    new Claim( KRFJwtConstants.Surname, context.Surname?? string.Empty ),
                    new Claim( KRFJwtConstants.UserName, context.UserName ),
                    new Claim( KRFJwtConstants.UserId, context.UserId.ToString() ),
                    new Claim( KRFJwtConstants.SessionId, context.SessionId.ToString() ),
                    new Claim( KRFJwtConstants.IsAdmin, context.Claim.Equals( Claims.Admin ) ? bool.TrueString : bool.FalseString )
                } ),
                Expires = expiration,
                SigningCredentials = new SigningCredentials( key, SecurityAlgorithms.HmacSha512Signature )
            };

            return string.Concat( KRFJwtConstants.BearerSpaced, handler.WriteToken( handler.CreateToken( descriptor ) ) );
        }

        public static Claim GetUserRoleClaim( IEnumerable<Claim> claims )
        {
            if ( claims == null || !claims.Any() )
            {
                return new Claim( KRFConstants.UserRoleClaim, nameof( Claims.NotLogged ) );
            }

            var isAdmin = claims.FirstOrDefault( x => x.Type.Equals( KRFJwtConstants.IsAdmin, StringComparison.OrdinalIgnoreCase ) )?.Value.Equals( bool.TrueString, StringComparison.OrdinalIgnoreCase );
            if ( isAdmin ?? false )
            {
                return new Claim( KRFConstants.UserRoleClaim, nameof( Claims.Admin ) );
            }

            return new Claim( KRFConstants.UserRoleClaim, nameof( Claims.User ) );
        }
    }
}
