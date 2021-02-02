namespace KRFCommon.CQRS.Command
{
    using KRFCommon.CQRS.Common;

    public interface ICommandValidationError
    {
        ErrorOut GetError();
    }
}
