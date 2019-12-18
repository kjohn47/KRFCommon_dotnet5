using System.Net;

namespace KRFCommon.CQRS.Common
{
    public class ErrorOut
    {
        public ErrorOut(HttpStatusCode code, string message)
        {
            this.ErrorMessage = message;
            this.ErrorStatusCode = (int)code;
        }
        public int ErrorStatusCode { get; }
        public string ErrorMessage { get; }
    }
}
