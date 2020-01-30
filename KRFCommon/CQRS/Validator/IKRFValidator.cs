using KRFCommon.CQRS.Command;
using System.Threading.Tasks;

namespace KRFCommon.CQRS.Validator
{
    public interface IKRFValidator<TInput>
    {
        public Task<CommandValidationError> CheckValidationAsync(TInput request);
    }
}
