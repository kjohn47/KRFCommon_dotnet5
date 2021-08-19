namespace KRFCommon.CQRS.Validator
{
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using FluentValidation;

    using KRFCommon.Constants;
    using KRFCommon.CQRS.Common;

    public class KRFValidator<TInput> : AbstractValidator<TInput> where TInput : ICQRSRequest
    {
        private readonly HttpStatusCode HttpStatus = HttpStatusCode.BadRequest;

        public KRFValidator( HttpStatusCode httpStatus, CascadeMode? validationMode = null ) : this( validationMode )
        {
            this.HttpStatus = httpStatus;
        }

        public KRFValidator( CascadeMode? validationMode = null )
        {
            this.CascadeMode = validationMode.HasValue ? validationMode.Value : CascadeMode.Stop;
        }

        public async Task<IValidationResult> CheckValidationAsync( TInput request )
        {
            var validationResult = await this.ValidateAsync( request );

            if ( validationResult.IsValid )
            {
                return ValidationResult.ReturnValid();
            }

            if ( this.CascadeMode.Equals( CascadeMode.Continue ) )
            {
                return ValidationResult.ReturnInvalid(
                    ValidationHelper.GenerateError(
                        KRFConstants.ValidationErrorMessage,
                        string.Empty,
                        this.HttpStatus,
                        KRFConstants.ValidationErrorCode,
                        validationResult.Errors.Select( x => new ValidationError( x.ErrorCode, x.ErrorMessage, x.PropertyName ) ) ) );
            }

            var error = validationResult.Errors.First();
            return ValidationResult.ReturnInvalid(
                ValidationHelper.GenerateError(
                    error.ErrorMessage,
                    error.PropertyName,
                    this.HttpStatus,
                    error.ErrorCode ) );
        }
    }
}
