namespace KRFCommon.JSON
{
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public static class KRFJsonSerializerOptions
    {
        private static IList<JsonConverter> GetKRFConverterOptions()
        {
            return new List<JsonConverter>
            {
                new JsonStringEnumConverter( JsonNamingPolicy.CamelCase )
            };
        }

        public static JsonSerializerOptions GetJsonSerializerOptions( JsonSerializerOptions options = null )
        {
            var opt = options?? new JsonSerializerOptions();
            foreach ( var converter in GetKRFConverterOptions() )
            {
                opt.Converters.Add( converter );
            }
            opt.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            opt.PropertyNameCaseInsensitive = true;
            opt.AllowTrailingCommas = true;
            return opt;
        }
    }
}
