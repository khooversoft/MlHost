using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MlHost.Services
{
    public class Json : IJson
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public string Serialize<T>(T subject) => JsonSerializer.Serialize(subject, _options);

        public T Deserialize<T>(string subject) => JsonSerializer.Deserialize<T>(subject, _options);
    }
}
