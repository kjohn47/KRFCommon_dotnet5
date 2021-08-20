namespace KRFCommon.Context
{
    using System;

    public class UserContext : IUserContext
    {
        public Guid UserId { get; set; }
        public Guid SessionId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public Claims Claim { get; set; }
        public DateTime? TokenExpiration { get; set; }
    }
}
