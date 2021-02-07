namespace KRFCommon.CQRS.Command
{
    using KRFCommon.CQRS.Common;

    using System.Threading.Tasks;

    public interface ICommand<TReq, TResp>
        where TReq : ICommandRequest
        where TResp : ICommandResponse
    {
        Task<ICommandValidationError> ExecuteValidationAsync( TReq request );
        Task<IResponseOut<TResp>> ExecuteCommandAsync( TReq request );
    }
}
