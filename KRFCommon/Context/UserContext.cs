using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KRFCommon.Context
{
    public class UserContext: IUserContext
    {
        public UserContext( ITokenProvider tokenProvider, string key )
        {
            this.Claim = Claims.NotLogged;
            if ( !string.IsNullOrEmpty( tokenProvider.Token ) )
            {
                var token = tokenProvider.Token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ? tokenProvider.Token.Substring(7) : tokenProvider.Token;
                var jsonToken = Jose.JWT.Decode(token, Encoding.UTF8.GetBytes(key));
                var context = JsonConvert.DeserializeObject<Dictionary<string,string>>(jsonToken);
                if (context != null)
                {
                    this.UserId = new Guid(context.GetValueOrDefault("UserId"));
                    this.Name = context.GetValueOrDefault("Name") ?? string.Empty;
                    this.Surname = context.GetValueOrDefault("Surname") ?? string.Empty;
                    this.UserName = context.GetValueOrDefault("UserName") ?? throw new Exception("UserName cannot be empty");
                    this.Claim = context.GetValueOrDefault("isAdmin").Equals("true", StringComparison.OrdinalIgnoreCase) ? Claims.Admin : Claims.User;
                }
            }
        }

        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public Claims Claim { get; set; }
    }
}
