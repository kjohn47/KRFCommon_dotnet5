using System;

namespace KRFCommon.Context
{
    public interface IUserContext
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public Claims Claim { get; set; }       
    }
}
