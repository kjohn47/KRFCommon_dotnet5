namespace KRFCommon.CQRS.Command
{
    using KRFCommon.CQRS.Common;
    using KRFCommon.CQRS.Validator;

    using System.Threading.Tasks;

    public interface ICommand<TReq, TResp>
        where TReq : ICommandRequest
        where TResp : ICommandResponse
    {
        Task<IValidationResult> ExecuteValidationAsync( TReq request );
        Task<IResponseOut<TResp>> ExecuteCommandAsync( TReq request );
    }
}
