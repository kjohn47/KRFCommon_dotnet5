namespace KRFCommon.Api
{
    public class AppConfiguration
    {
        private bool _tokenValidateLife = true;
        public string ApiName { get; set; }
        public string TokenIdentifier { get; set; }
        public string TokenKey { get; set; }
        public bool TokenValidateLife {
            get
            {
                return this._tokenValidateLife;
            }

            set 
            {
                this._tokenValidateLife = value;
            }
        }
        public string[] TokenValidIssuers { get; set; }
        public string[] TokenValidAudiences { get; set; }
        public bool AllowAnonymousOnAuthorizeWithoutPolicy { get; set; }
        public bool EnableReqLogs { get; set; }
        public int? RequestBufferSize { get; set; }

        public bool HasIssuer => this.TokenValidIssuers != null && this.TokenValidIssuers.Length > 0;
        public bool HasAudience => this.TokenValidAudiences != null && this.TokenValidAudiences.Length > 0;

        private bool HasMultipleIssuers => this.HasIssuer && this.TokenValidIssuers.Length > 1;
        private bool HasMultipleAudiences => this.HasAudience && this.TokenValidAudiences.Length > 1;        

        public string GetSingleIssuer => this.HasIssuer && !this.HasMultipleIssuers ? this.TokenValidIssuers[ 0 ] : null;
        public string GetSingleAudience => this.HasAudience && !this.HasMultipleAudiences ? this.TokenValidAudiences[ 0 ] : null;
        public string[] GetMultipleIssuers => this.HasMultipleIssuers ? this.TokenValidIssuers : null;
        public string[] GetMultipleAudiences => this.HasMultipleAudiences ? this.TokenValidAudiences : null;
    }
}
