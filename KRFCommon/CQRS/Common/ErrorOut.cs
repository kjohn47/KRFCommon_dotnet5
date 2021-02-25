namespace KRFCommon.CQRS.Common
{
    using System.Net;

    using KRFCommon.Constants;

    public class ErrorOut
    {

        public ErrorOut()
        { }

        public ErrorOut( HttpStatusCode code, string message )
        {
            this.ErrorMessage = message;
            this.ErrorStatusCode = ( int ) code;
            this.WithErrors = true;
            this.ErrorType = ResponseErrorType.Unknown;
            this.ErrorCode = KRFConstants.DefaultErrorCode;
        }

        public ErrorOut( HttpStatusCode code, string message, string errorCode )
        {
            this.ErrorMessage = message;
            this.ErrorStatusCode = ( int ) code;
            this.WithErrors = true;
            this.ErrorType = ResponseErrorType.Unknown;
            this.ErrorCode = string.IsNullOrEmpty( errorCode ) ? KRFConstants.DefaultErrorCode : errorCode;
        }

        public ErrorOut( HttpStatusCode code, string message, string errorProperty, string errorCode )
        {
            this.ErrorMessage = message;
            this.ErrorStatusCode = ( int ) code;
            this.ErrorProperty = errorProperty;
            this.WithErrors = true;
            this.ErrorType = ResponseErrorType.Unknown;
            this.ErrorCode = string.IsNullOrEmpty( errorCode ) ? KRFConstants.DefaultErrorCode : errorCode;
        }

        public ErrorOut( HttpStatusCode code, string message, bool validationError, string errorProperty, string errorCode )
        {
            this.ErrorMessage = message;
            this.ErrorStatusCode = ( int ) code;
            this.ErrorProperty = errorProperty;
            this.WithErrors = true;
            this.ErrorType = validationError ? ResponseErrorType.Validation : ResponseErrorType.Unknown;
            this.ErrorCode = string.IsNullOrEmpty( errorCode ) ? KRFConstants.DefaultErrorCode : errorCode;
        }

        public ErrorOut( HttpStatusCode code, string message, ResponseErrorType errorType )
        {
            this.ErrorMessage = message;
            this.ErrorStatusCode = ( int ) code;
            this.WithErrors = true;
            this.ErrorType = errorType;
            this.ErrorCode = KRFConstants.DefaultErrorCode;
        }

        public ErrorOut( HttpStatusCode code, string message, ResponseErrorType errorType, string errorCode )
        {
            this.ErrorMessage = message;
            this.ErrorStatusCode = ( int ) code;
            this.WithErrors = true;
            this.ErrorType = errorType;
            this.ErrorCode = string.IsNullOrEmpty( errorCode ) ? KRFConstants.DefaultErrorCode : errorCode;
        }

        public ErrorOut( HttpStatusCode code, string message, ResponseErrorType errorType, string errorProperty, string errorCode )
        {
            this.ErrorMessage = message;
            this.ErrorStatusCode = ( int ) code;
            this.ErrorProperty = errorProperty;
            this.WithErrors = true;
            this.ErrorType = errorType;
            this.ErrorCode = string.IsNullOrEmpty( errorCode ) ? KRFConstants.DefaultErrorCode : errorCode;
        }

        public bool WithErrors { get; set; }
        public int ErrorStatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorProperty { get; set; }
        public ResponseErrorType ErrorType { get; set; }
        public string ErrorCode { get; set; }

        public bool ValidationError => this.ErrorType.Equals( ResponseErrorType.Validation );
    }
}
