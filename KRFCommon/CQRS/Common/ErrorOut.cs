using System.Net;

namespace KRFCommon.CQRS.Common
{
    public class ErrorOut
    {

        public ErrorOut(HttpStatusCode code, string message, string errorProperty = "")
        {
            this.ErrorMessage = message;
            this.ErrorStatusCode = (int)code;
            this.ErrorProperty = errorProperty;
            this.WithErrors = true;
            this.ValidationError = true;
            this.ErrorType = ResponseErrorType.Unknown;
        }

        public ErrorOut(HttpStatusCode code, string message, bool validationError, string errorProperty = "")
        {
            this.ErrorMessage = message;
            this.ErrorStatusCode = (int)code;
            this.ErrorProperty = errorProperty;
            this.ValidationError = validationError;
            this.WithErrors = true;
            this.ErrorType = ResponseErrorType.Unknown;
        }

        public ErrorOut(HttpStatusCode code, string message, ResponseErrorType errorType, string errorProperty = "")
        {
            this.ErrorMessage = message;
            this.ErrorStatusCode = (int)code;
            this.ErrorProperty = errorProperty;
            this.WithErrors = true;
            this.ValidationError = true;
            this.ErrorType = errorType;
        }

        public ErrorOut(HttpStatusCode code, string message, bool validationError, ResponseErrorType errorType, string errorProperty = "")
        {
            this.ErrorMessage = message;
            this.ErrorStatusCode = (int)code;
            this.ErrorProperty = errorProperty;
            this.ValidationError = validationError;
            this.WithErrors = true;
            this.ErrorType = errorType;
        }

        public bool WithErrors { get; }
        public int ErrorStatusCode { get; }
        public string ErrorMessage { get; }
        public string ErrorProperty { get; }
        public bool ValidationError { get; }
        public ResponseErrorType ErrorType { get; }
    }
}
