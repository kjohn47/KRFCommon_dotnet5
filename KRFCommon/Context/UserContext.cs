using System;

namespace KRFCommon.Context
{
    public class UserContext: IUserContext
    {
        public UserContext( ITokenProvider tokenProvider, string key )
        {
            if( !string.IsNullOrEmpty( tokenProvider.Token ) )
            {
                this.Name = tokenProvider.Token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ? tokenProvider.Token.Substring(7) : tokenProvider.Token;
                this.Surname = key;
            }
        }

        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public Claims Claim { get; set; }
    }
}
