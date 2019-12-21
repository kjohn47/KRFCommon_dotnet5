using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KRFCommon.Context
{
    public enum Claims
    {
        [Description(Policies.Admin)]
        Admin,
        [Description(Policies.User)]
        User        
    }
    public static class Policies
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }
}
