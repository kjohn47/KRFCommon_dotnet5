namespace KRFCommon.CQRS.Validator
{
    using System.Threading.Tasks;

    using KRFCommon.CQRS.Command;

    public interface IKRFValidator<TInput>
    {
        public Task<IValidationResult> CheckValidationAsync( TInput request );
    }
}
