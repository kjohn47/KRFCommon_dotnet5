namespace KRFCommon.CQRS.Query
{
    using System.Threading.Tasks;

    using KRFCommon.CQRS.Common;

    public interface IQuery<in Tinput, Toutput>
        where Tinput : IQueryRequest
        where Toutput : IQueryResponse
    {
        Task<IResponseOut<Toutput>> QueryAsync( Tinput request );
    }
}
