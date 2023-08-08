using System.Text.Json;
using System.Text.Json.Serialization;

namespace FeedR.Shared.Serialization
{
    internal sealed class SystemTextJsonSerializer : ISerializer
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public T? Deserialize<T>(string value) where T : class
        {
            return JsonSerializer.Deserialize<T>(value, Options);
        }

        public string Serialize<T>(T value) where T : class
        {
            return JsonSerializer.Serialize(value, Options);
        }
    }
}
