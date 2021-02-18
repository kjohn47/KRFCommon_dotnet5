namespace KRFCommon.Context
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.Json;

    using KRFCommon.JSON;
    using KRFCommon.Constants;
    using KRFCommon.Helpers;

    public class UserContext : IUserContext
    {
        public UserContext( ITokenProvider tokenProvider, string key )
        {
            this.Claim = Claims.NotLogged;
            if ( !string.IsNullOrEmpty( tokenProvider.Token ) )
            {
                var token = tokenProvider.Token.StartsWith( KRFJwtConstants.Bearer, StringComparison.OrdinalIgnoreCase ) ? tokenProvider.Token.Substring( 7 ) : tokenProvider.Token;
                var jsonToken = Jose.JWT.Decode( token, Encoding.UTF8.GetBytes( key ) );
                CaseInsensitiveDictionary<string> context;
                try
                {
                    context = JsonSerializer.Deserialize<CaseInsensitiveDictionary<string>>( jsonToken, KRFJsonSerializerOptions.GetJsonSerializerOptions() );
                }
                catch
                {
                    throw new Exception( "An error ocurred during deserialization of token. All fields must be type string" );
                }

                if ( context != null )
                {
                    try { this.UserId = new Guid( context.GetValueOrDefault( KRFJwtConstants.UserId, string.Empty ) ); } catch { throw new Exception( "Invalid User ID" ); }
                    try { this.SessionId = new Guid( context.GetValueOrDefault( KRFJwtConstants.SessionId, string.Empty ) ); } catch { throw new Exception( "Invalid User Session Id" ); }
                    this.Name = context.GetValueOrDefault( KRFJwtConstants.Name, string.Empty );
                    this.Surname = context.GetValueOrDefault( KRFJwtConstants.Surname, string.Empty );
                    this.UserName = context.GetValueOrDefault( KRFJwtConstants.UserName ) ?? throw new Exception( "UserName cannot be empty" );
                    this.Claim = context.GetValueOrDefault( KRFJwtConstants.IsAdmin, string.Empty ).Equals( "true", StringComparison.OrdinalIgnoreCase ) ? Claims.Admin : Claims.User;
                }
            }
        }

        public Guid UserId { get; set; }
        public Guid SessionId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public Claims Claim { get; set; }
    }
}
