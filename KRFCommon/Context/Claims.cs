namespace KRFCommon.Context
{
    using System.ComponentModel;

    public enum Claims
    {
        [Description( Policies.Admin )]
        Admin,
        [Description( Policies.User )]
        User,
        [Description( Policies.NotLogged )]
        NotLogged
    }
    public static class Policies
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string NotLogged = "NotLogged";
    }
}
