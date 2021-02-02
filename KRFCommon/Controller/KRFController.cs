namespace KRFCommon.Controller
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    using KRFCommon.CQRS.Command;
    using KRFCommon.CQRS.Query;

    public class KRFController : ControllerBase
    {
        public async Task<IActionResult> ExecuteAsyncQuery<Tinput, Toutput>(Tinput request, IQuery<Tinput, Toutput> query)
            where Tinput : IQueryRequest
            where Toutput : IQueryResponse
        {
            var queryResult = await query.QueryAsync( request );

            if( queryResult.Error != null )
            {
                return this.StatusCode(queryResult.Error.ErrorStatusCode, queryResult.Error );
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
            var commandValid = await command.ExecuteValidationAsync(request);

            if (commandValid.GetError() != null)
            {
                return this.StatusCode(commandValid.GetError().ErrorStatusCode, commandValid.GetError() );
            }

            var commandResult = await command.ExecuteCommandAsync(request);
            if( commandResult.Error != null )
            {
                return this.StatusCode(commandResult.Error.ErrorStatusCode, commandResult.Error);
            }

            return changeAction != null ? changeAction(commandResult.Result) : this.Ok(commandResult.Result);
        }
    }
}
