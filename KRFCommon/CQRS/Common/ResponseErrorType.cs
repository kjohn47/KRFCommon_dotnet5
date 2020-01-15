using System.Text.Json.Serialization;

namespace KRFCommon.CQRS.Common
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ResponseErrorType
    {
        Unknown,
        Database,
        Proxy,
        Application
    }
}
