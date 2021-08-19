namespace KRFCommon.Constants
{
    public static class KRFConstants
    {
        //String Constants
        public const string JsonContentType = "application/json";
        public const string JsonContentUtf8Type = "application/json; charset=utf-8;";
        public const string AuthenticateHeader = "www-authenticate";
        public const string UserRoleClaim = "UserRole";
        public const string GetMethod = "GET";
        public const string PostMethod = "POST";
        public const string PutMethod = "PUT";
        public const string DeleteMethod = "DELETE";
        public const string TimeStampFormat = "yyyy-MM-ddTHH:mm:ss.fff";
        public const string DateWithoutSeparatorFormat = "yyyyMMdd";
        public const string DefaultLogsFolder = "Logs";
        public const string SQLLogFolder = "SQL";
        public const string APILogFolder = "Api";
        public const string LogReqRespEvtName = "KRFRequestResponseLogger";
        public const string LogExceptionEvtName = "KRFExceptionLogger";
        public const string DefaultErrorCode = "KRFERROR";
        public const string AuthorizationErrorCode = "KRFNOTAUTHORIZED";
        public const string NotAvailableErrorCode = "KRFNOTAVAILABLE";
        public const string HttpExErrorCode = "KRFHTTPEXCEPTION";
        public const string AnonymousIdentity = "IsAnonymous";
        public const string ValidationErrorCode = "KRFVALIDATIONERROR";
        public const string ValidationErrorMessage = "One or more fields have invalid data";

        //Int Constants
        public const int EntityFrameworkEventId = 20101;
        public const int ApiEventId = 474747;
    }
}
