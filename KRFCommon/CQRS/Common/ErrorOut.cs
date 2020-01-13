using System.Net;

namespace KRFCommon.CQRS.Common
{
    public class ErrorOut
    {

        public ErrorOut(HttpStatusCode code, string message, string property = "")
        {
            this.ErrorMessage = message;
            this.ErrorStatusCode = (int)code;
            this.Property = property;
        }

        public ErrorOut(HttpStatusCode code, string message, bool isValidationError, string property = "")
        {
            this.ErrorMessage = message;
            this.ErrorStatusCode = (int)code;
            this.Property = property;
            this.IsValidationError = isValidationError;
        }
        public int ErrorStatusCode { get; }
        public string ErrorMessage { get; }
        public string Property { get; }
        public bool IsValidationError { get; }
    }
}
