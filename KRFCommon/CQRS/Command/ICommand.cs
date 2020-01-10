using System.Threading.Tasks;

namespace KRFCommon.CQRS.Command
{
    public interface ICommand<TReq, TResp>
        where TReq: class
        where TResp: class
    {
        Task<CommandValidationError> ExecuteValidationAsync(TReq request);
        Task<TResp> ExecuteCommandAsync(TReq request);
    }
}
