using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Localizer
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LogLevel : byte
    {
        Fatal = 0,
        Error = 1,
        Warn = 2,
        Info = 3,
        Debug = 4,
    }
}
