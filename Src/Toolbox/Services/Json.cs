using System.Text.Json;

namespace Toolbox.Services
{
    public class Json : IJson
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        private static readonly JsonSerializerOptions _serializeOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        };

        public string Serialize<T>(T subject) => JsonSerializer.Serialize(subject, _options);

        public string SerializeWithIndent<T>(T subject) => JsonSerializer.Serialize(subject, _serializeOptions);

        public T Deserialize<T>(string subject) => JsonSerializer.Deserialize<T>(subject, _options);
    }
}
