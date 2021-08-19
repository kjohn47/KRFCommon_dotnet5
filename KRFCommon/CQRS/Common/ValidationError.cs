namespace KRFCommon.CQRS.Common
{
    public class ValidationError
    {
        public ValidationError( string errorCode, string errorMessage, string propertyName )
        {
            this.ErrorCode = errorCode;
            this.ErrorMessage = errorMessage;
            this.PropertyName = propertyName;
        }

        public string ErrorCode { get; private set; }
        public string ErrorMessage { get; private set; }
        public string PropertyName { get; private set; }
    }
}
