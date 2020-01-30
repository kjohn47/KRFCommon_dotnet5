using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace KRFCommon.CQRS.Common
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ResponseErrorType
    {
        [EnumMember(Value = "Unknown")]
        Unknown,
        [EnumMember(Value = "Database")]
        Database,
        [EnumMember(Value = "Proxy")]
        Proxy,
        [EnumMember(Value = "Application")]
        Application,
        [EnumMember(Value = "Validation")]
        Validation,
        [EnumMember(Value = "Exception")]
        Exception
    }
}
