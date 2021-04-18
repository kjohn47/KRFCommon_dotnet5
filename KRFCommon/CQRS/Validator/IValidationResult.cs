namespace KRFCommon.CQRS.Validator
{
    using KRFCommon.Common;
    using KRFCommon.CQRS.Common;

    public interface IValidationResult
    {
        NullableObject<ErrorOut> Error { get; }
    }
}
