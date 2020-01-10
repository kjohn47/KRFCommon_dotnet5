using KRFCommon.CQRS.Common;
namespace KRFCommon.CQRS.Command
{
    public class CommandValidationError
    {
        public CommandValidationError()
        {
            this.HasError = false;
        }

        public CommandValidationError(ErrorOut error ) 
        {
            this.HasError = true;
            this.Error = error;
        }

        public ErrorOut Error { get; set; }
        public bool HasError { get; private set; }
}
}
