namespace KRFCommon.Controller
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    using KRFCommon.CQRS.Command;
    using KRFCommon.CQRS.Query;

    public class KRFController : ControllerBase
    {
        public async Task<IActionResult> ExecuteAsyncQuery<Tinput, Toutput>( Tinput request, IQuery<Tinput, Toutput> query )
            where Tinput : IQueryRequest
            where Toutput : IQueryResponse
        {
            var queryResult = await query.QueryAsync( request );

            if ( queryResult.Error.HasValue )
            {
                return this.StatusCode( queryResult.Error.Value.ErrorStatusCode, queryResult.Error.Value );
            }

            return this.Ok( queryResult.Result );
        }

        public async Task<IActionResult> ExecuteAsyncCommand<Tinput, Toutput>
            (
                Tinput request,
                ICommand<Tinput, Toutput> command,
                Func<Toutput, IActionResult> changeAction = null
            )
        where Tinput : ICommandRequest
        where Toutput : ICommandResponse
        {
            var commandValid = await command.ExecuteValidationAsync( request );

            if ( commandValid.Error.HasValue )
            {
                return this.StatusCode( commandValid.Error.Value.ErrorStatusCode, commandValid.Error.Value );
            }

            var commandResult = await command.ExecuteCommandAsync( request );
            if ( commandResult.Error.HasValue )
            {
                return this.StatusCode( commandResult.Error.Value.ErrorStatusCode, commandResult.Error.Value );
            }

            return changeAction != null ? changeAction( commandResult.Result ) : this.Ok( commandResult.Result );
        }
    }
}
