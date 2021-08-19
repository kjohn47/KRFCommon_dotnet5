namespace KRFCommon.CQRS.Common
{
    using System.Collections.Generic;
    using System.Net;

    using KRFCommon.Constants;

    public class ErrorOut
    {

        public ErrorOut()
        {
            this.WithErrors = true;
        }

        public ErrorOut( HttpStatusCode code, string message )
            : this(code, message, ResponseErrorType.Unknown, null)
        {
        }

        public ErrorOut( HttpStatusCode code, string message, string errorCode ) 
            : this(code, message, ResponseErrorType.Unknown, errorCode)
        {
        }

        public ErrorOut( HttpStatusCode code, string message, string errorProperty, string errorCode )
            : this( code, message, ResponseErrorType.Unknown, errorProperty, errorCode )
        {
        }

        public ErrorOut( HttpStatusCode code, string message, bool validationError, string errorProperty, string errorCode, IEnumerable<ValidationError> validationErrors )
            : this( code, message, validationError, errorProperty, errorCode )
        {
            this.ValidationErrors = validationErrors;
        }

        public ErrorOut( HttpStatusCode code, string message, bool validationError, string errorProperty, string errorCode ) 
            : this( code, message, validationError ? ResponseErrorType.Validation : ResponseErrorType.Unknown, errorProperty, errorCode )
        {
        }

        public ErrorOut( HttpStatusCode code, string message, ResponseErrorType errorType )
            : this( code, message, errorType, null )
        {
        }

        public ErrorOut( HttpStatusCode code, string message, ResponseErrorType errorType, string errorCode ) 
            : this( code, message, errorType, null, errorCode )
        {
        }

        public ErrorOut( HttpStatusCode code, string message, ResponseErrorType errorType, string errorProperty, string errorCode ) : this()
        {
            this.ErrorMessage = message;
            this.ErrorStatusCode = code;
            this.ErrorProperty = errorProperty;
            this.ErrorType = errorType;
            this.ErrorCode = string.IsNullOrEmpty( errorCode ) ? KRFConstants.DefaultErrorCode : errorCode;
        }

        public bool WithErrors { get; private set; }
        public HttpStatusCode ErrorStatusCode { get; private set; }
        public string ErrorMessage { get; private set; }
        public string ErrorProperty { get; private set; }
        public ResponseErrorType ErrorType { get; private set; }
        public string ErrorCode { get; private set; }
        public IEnumerable<ValidationError> ValidationErrors { get; private set; }

        public bool IsValidationError => this.ErrorType.Equals( ResponseErrorType.Validation );
    }
}
