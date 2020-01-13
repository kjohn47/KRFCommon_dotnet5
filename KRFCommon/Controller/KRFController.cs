using KRFCommon.Context;
using KRFCommon.CQRS.Command;
using KRFCommon.CQRS.Query;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace KRFCommon.Controller
{
    public class KRFController : ControllerBase
    {
        public async Task<IActionResult> ExecuteAsyncQuery<Tinput, Toutput>(Tinput request, IQuery<Tinput, Toutput> query)
            where Tinput : class
            where Toutput : class
        {
            var queryResult = await query.QueryAsync( request );

            if( queryResult.HasError )
            {
                return this.StatusCode(queryResult.Error.ErrorStatusCode, JsonConvert.SerializeObject(queryResult.Error) );
            }

            return this.Ok( queryResult.Result );
        }

        public async Task<IActionResult> ExecuteAsyncCommand<Tinput, Toutput>
            (
                Tinput request, 
                ICommand<Tinput, Toutput> command,
                Func<Toutput, IActionResult> changeAction = null
            )
        where Tinput : class
        where Toutput : class
        {
            var commandValid = await command.ExecuteValidationAsync(request);

            if (commandValid.HasError)
            {
                return this.StatusCode(commandValid.Error.ErrorStatusCode, JsonConvert.SerializeObject(commandValid.Error) );
            }

            var commandResult = await command.ExecuteCommandAsync(request);

            return changeAction != null ? changeAction(commandResult) : this.Ok(commandResult);
        }
    }
}
