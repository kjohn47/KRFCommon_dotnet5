namespace KRFCommon.Context
{
    using System;
    using System.Text;

    using KRFCommon.Constants;

    public class KRFJwt
    {
        public KRFJwt()
        { }

        public KRFJwt( IUserContext context )
        {
            this.Name = context.Name;
            this.Surname = context.Surname;
            this.UserName = context.UserName;
            this.IsAdmin = context.Claim.Equals( Claims.Admin ) ? "true" : null;
            this.UserId = context.UserId.ToString();
            this.SessionId = context.SessionId.ToString();
        }

        public KRFJwt( string name, string surname, string userName, bool isAdmin, Guid userId, Guid sessionId )
        {
            this.Name = name;
            this.Surname = surname;
            this.UserName = userName;
            this.IsAdmin = isAdmin ? "true" : null;
            this.UserId = userId.ToString();
            this.SessionId = sessionId.ToString();
        }

        public string UserId { get; private set; }
        public string SessionId { get; private set; }
        public string UserName { get; private set; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public string IsAdmin { get; private set; }

        public static string GetSignedBearerTokenFromContext( IUserContext context, string signKey )
        {
            return KRFJwtConstants.Bearer + Jose.JWT.Encode( new KRFJwt( context ), Encoding.UTF8.GetBytes( signKey ), Jose.JwsAlgorithm.HS512 );
        }
    }
}
