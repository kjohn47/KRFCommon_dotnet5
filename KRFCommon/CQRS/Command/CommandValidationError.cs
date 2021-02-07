namespace KRFCommon.CQRS.Command
{
    using KRFCommon.CQRS.Common;

    public class CommandValidationError : ICommandValidationError
    {
        public CommandValidationError()
        {
            this.hasError = false;
        }

        public CommandValidationError( ErrorOut error )
        {
            this.hasError = true;
            this.error = error;
        }

        private ErrorOut error;

        private bool hasError;

        public ErrorOut GetError()
        {
            if ( this.hasError )
            {
                return error;
            }

            return null;
        }
    }
}
