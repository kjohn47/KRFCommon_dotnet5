namespace KRFCommon.CQRS.Validator
{
    using System.Net;

    using KRFCommon.CQRS.Common;

    public static class ValidationHelper
    {
        public static ErrorOut GenerateError( string errorMessage, string errorProperty, HttpStatusCode? httpStatus = null, string errorCode = null )
        {
            return new ErrorOut( 
                httpStatus.HasValue ? httpStatus.Value : HttpStatusCode.BadRequest,
                errorMessage, 
                ResponseErrorType.Validation, 
                errorProperty, errorCode );
        }
    }
}
