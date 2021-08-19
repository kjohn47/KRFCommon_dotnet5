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
                return this.StatusCode( (int) queryResult.Error.Value.ErrorStatusCode, queryResult.Error.Value );
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
                return this.StatusCode( ( int ) commandValid.Error.Value.ErrorStatusCode, commandValid.Error.Value );
            }

            var commandResult = await command.ExecuteCommandAsync( request );
            if ( commandResult.Error.HasValue )
            {
                return this.StatusCode( ( int ) commandResult.Error.Value.ErrorStatusCode, commandResult.Error.Value );
            }

            return changeAction != null ? changeAction( commandResult.Result ) : this.Ok( commandResult.Result );
        }

        public async Task<IActionResult> ExecuteFileQueryAsync<Tinput, Toutput>
            ( 
                Tinput request, 
                IQuery<Tinput, Toutput> query 
            )
        where Tinput : IQueryRequest
        where Toutput : IFileQueryResponse
        {
            try
            {
                var queryResult = await query.QueryAsync( request );

                if ( queryResult.Error.HasValue )
                {
                    return this.NotFound();
                }

                if ( queryResult.Result.HasFileName )
                {
                    return this.File( queryResult.Result.FileBytes, queryResult.Result.MimeType, queryResult.Result.FileName );
                }

                return this.File( queryResult.Result.FileBytes, queryResult.Result.MimeType );
            }
            catch
            {
                return this.NotFound();
            }
        }
    }
}
