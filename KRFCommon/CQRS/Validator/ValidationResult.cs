namespace KRFCommon.CQRS.Validator
{
    using KRFCommon.Common;
    using KRFCommon.CQRS.Common;

    public class ValidationResult : IValidationResult
    {
        protected ValidationResult()
        {
            this.Error = NullableObject<ErrorOut>.EmptyResult();
        }
        protected ValidationResult( ErrorOut error )
        {
            this.Error = NullableObject<ErrorOut>.FromResult( error );
        }

        public NullableObject<ErrorOut> Error { get; }

        public static ValidationResult ReturnInvalid( ErrorOut error )
        {
            return new ValidationResult( error );
        }

        public static ValidationResult ReturnValid()
        {
            return new ValidationResult();
        }
    }
}
