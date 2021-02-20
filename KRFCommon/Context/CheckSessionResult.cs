namespace KRFCommon.Context
{
    using KRFCommon.CQRS.Command;
    using KRFCommon.CQRS.Common;

    public class CheckSessionResult: ICommandResponse
    {
        public bool? HasError { get; set; }
        public ErrorOut Error { get; set; }
    }
}
