using KRFCommon.CQRS.Query;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace KRFCommon.Controller
{
    public class KFRController : ControllerBase
    {
        public async Task<IActionResult> ExecuteAsyncQuery<Tinput, Toutput>(Tinput request, IQuery<Tinput, Toutput> query)
            where Tinput : class
            where Toutput : class
        {
            var queryResult = await query.QueryAsync( request );

            if( queryResult.HasError )
            {
                this.StatusCode(queryResult.Error.ErrorStatusCode, queryResult.Error.ErrorMessage);
            }

            return this.Ok( queryResult.Result );
        }
    }
}
