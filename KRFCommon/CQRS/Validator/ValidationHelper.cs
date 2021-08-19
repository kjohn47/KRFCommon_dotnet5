namespace KRFCommon.CQRS.Validator
{
    using System.Collections.Generic;
    using System.Net;

    using KRFCommon.CQRS.Common;

    public static class ValidationHelper
    {
        public static ErrorOut GenerateError( string errorMessage, string errorProperty, HttpStatusCode httpStatus, string errorCode, IEnumerable<ValidationError> validationErrors = null )
        {
            return new ErrorOut(
                httpStatus,
                errorMessage, 
                true, 
                errorProperty, 
                errorCode,
                validationErrors );
        }
    }
}
