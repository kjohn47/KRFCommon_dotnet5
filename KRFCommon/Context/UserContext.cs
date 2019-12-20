using System;

namespace KRFCommon.Context
{
    public class UserContext: IUserContext
    {
        public UserContext( ITokenProvider tokenProvider )
        {
            if( !string.IsNullOrEmpty( tokenProvider.Token ) )
            {
                this.Name = tokenProvider.Token;
            }
        }

        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public Claims Claim { get; set; }
    }
}
