using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace MlHostWeb.Client.Services
{
    public class StateCacheService
    {
        private readonly ConcurrentDictionary<string, ICacheKey> _apiCache = new ConcurrentDictionary<string, ICacheKey>(StringComparer.OrdinalIgnoreCase);

        public StateCacheService() { }

        public T GetOrCreate<T>(string id, Func<T> createNew) where T : ICacheKey => (T)_apiCache.GetOrAdd(CreateKeyFromType<T>(id), k => createNew());

        public void Set<T>(T value) where T : ICacheKey => _apiCache[CreateKeyFromType<T>(value.GetId())] = value;

        public bool TryGetValue<T>(string id, out T value) where T : ICacheKey
        {
            value = default;
            if (!_apiCache.TryGetValue(CreateKeyFromType<T>(id), out ICacheKey cacheValue)) return false;

            value = (T)cacheValue;
            return true;
        }

        public void Clear() => _apiCache.Clear();

        public static string CreateKeyFromType<T>(string id) => $"{typeof(T).FullName}:{id.VerifyNotEmpty(nameof(id))}";
    }
}
