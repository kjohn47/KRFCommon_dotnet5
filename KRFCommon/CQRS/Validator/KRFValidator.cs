using FluentValidation;
using KRFCommon.CQRS.Command;
using System.Linq;
using System.Threading.Tasks;

namespace KRFCommon.CQRS.Validator
{
    public class KRFValidator<TInput>: AbstractValidator<TInput> where TInput: class
    {
        public KRFValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
        }

        public async Task<CommandValidationError> CheckValidationAsync(TInput request)
        {           
            var validationResult = await this.ValidateAsync(request);

            if( validationResult.IsValid )
            {
                return new CommandValidationError();
            }

            var error = validationResult.Errors.FirstOrDefault();
            return new CommandValidationError(ValidationHelper.GenerateError( error.ErrorMessage, error.PropertyName, error.ErrorCode ));
        }
    }
}
