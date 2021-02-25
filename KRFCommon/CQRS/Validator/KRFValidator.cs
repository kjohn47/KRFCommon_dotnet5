namespace KRFCommon.CQRS.Validator
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using FluentValidation;

    using KRFCommon.Constants;
    using KRFCommon.CQRS.Command;

    public class KRFValidator<TInput> : AbstractValidator<TInput> where TInput : class
    {
        public KRFValidator()
        {
            this.CascadeMode = CascadeMode.Stop;
        }

        public async Task<ICommandValidationError> CheckValidationAsync( TInput request )
        {
            var validationResult = await this.ValidateAsync( request );

            if ( validationResult.IsValid )
            {
                return new CommandValidationError();
            }

            var error = validationResult.Errors.FirstOrDefault();
            HttpStatusCode? httpStatus = null;
            var errorCode = KRFConstants.DefaultErrorCode;
            if ( !string.IsNullOrEmpty( error.ErrorCode ) )
            {
                var splitError = error.ErrorCode.Split( '|' );
                if ( splitError.Length == 2 )
                {
                    if ( Enum.TryParse<HttpStatusCode>( splitError[ 0 ], out var status ) )
                    {
                        httpStatus = status;
                    }
                }
                errorCode = splitError[ 1 ];
            }
            return new CommandValidationError( ValidationHelper.GenerateError( error.ErrorMessage, error.PropertyName, httpStatus, errorCode ) );
        }

        public static string GenerateErrorCodeWithHttpStatus( HttpStatusCode httpStatus, string errorCode )
        {
            return string.Format( "{0}|{1}", httpStatus.ToString(), errorCode );
        }
    }
}
