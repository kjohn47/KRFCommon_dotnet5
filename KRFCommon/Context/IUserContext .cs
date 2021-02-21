namespace KRFCommon.Context
{
    using System;

    public interface IUserContext
    {
        Guid UserId { get; }
        Guid SessionId { get; }
        string UserName { get; }
        string Name { get; }
        string Surname { get; }
        Claims Claim { get; }
    }
}
