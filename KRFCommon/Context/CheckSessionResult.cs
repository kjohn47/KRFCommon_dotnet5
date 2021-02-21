namespace KRFCommon.Context
{
    using KRFCommon.CQRS.Command;
    using KRFCommon.CQRS.Common;

    public class CheckSessionResult : ICommandResponse
    {
        public ErrorOut Error { get; set; }
        public bool Success => this.Error == null;
    }
}
