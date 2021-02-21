namespace KRFCommon.CQRS.Common
{
    using System.Net;

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
        }

        public ErrorOut( HttpStatusCode code, string message, string errorProperty )
        {
            this.ErrorMessage = message;
            this.ErrorStatusCode = ( int ) code;
            this.ErrorProperty = errorProperty;
            this.WithErrors = true;
            this.ErrorType = ResponseErrorType.Unknown;
        }

        public ErrorOut( HttpStatusCode code, string message, bool validationError, string errorProperty )
        {
            this.ErrorMessage = message;
            this.ErrorStatusCode = ( int ) code;
            this.ErrorProperty = errorProperty;
            this.WithErrors = true;
            this.ErrorType = validationError ? ResponseErrorType.Validation : ResponseErrorType.Unknown;
        }

        public ErrorOut( HttpStatusCode code, string message, ResponseErrorType errorType )
        {
            this.ErrorMessage = message;
            this.ErrorStatusCode = ( int ) code;
            this.WithErrors = true;
            this.ErrorType = errorType;
        }

        public ErrorOut( HttpStatusCode code, string message, ResponseErrorType errorType, string errorProperty )
        {
            this.ErrorMessage = message;
            this.ErrorStatusCode = ( int ) code;
            this.ErrorProperty = errorProperty;
            this.WithErrors = true;
            this.ErrorType = errorType;
        }

        public bool WithErrors { get; set; }
        public int ErrorStatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorProperty { get; set; }
        public ResponseErrorType ErrorType { get; set; }

        public bool ValidationError => this.ErrorType.Equals( ResponseErrorType.Validation );
    }
}
