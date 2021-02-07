namespace KRFCommon.CQRS.Validator
{
    using System.Linq;
    using System.Threading.Tasks;

    using FluentValidation;

    using KRFCommon.CQRS.Command;

    public class KRFValidator<TInput> : AbstractValidator<TInput> where TInput : class
    {
        public KRFValidator()
        {
            this.CascadeMode = CascadeMode.Stop;
        }

        public async Task<ICommandValidationError> CheckValidationAsync( TInput request )
        {
            var validationResult = await this.ValidateAsync( request );

            if ( validationResult.IsValid )
            {
                return new CommandValidationError();
            }

            var error = validationResult.Errors.FirstOrDefault();
            return new CommandValidationError( ValidationHelper.GenerateError( error.ErrorMessage, error.PropertyName, error.ErrorCode ) );
        }
    }
}
