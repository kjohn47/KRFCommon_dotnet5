namespace KRFCommon.CQRS.Validator
{
    using System;
    using System.Net;

    using KRFCommon.CQRS.Common;

    public static class ValidationHelper
    {
        public static ErrorOut GenerateError( string errorMessage, string errorProperty, string errorCode = "" )
        {
            HttpStatusCode code = HttpStatusCode.BadRequest;
            if ( !string.IsNullOrEmpty( errorCode ) )
            {
                object errorParseOut;
                Enum.TryParse( typeof( HttpStatusCode ), errorCode, out errorParseOut );
                if ( errorParseOut != null )
                {
                    code = ( HttpStatusCode ) errorParseOut;
                }
            }

            return new ErrorOut( code, errorMessage, ResponseErrorType.Validation, errorProperty );
        }
    }
}
