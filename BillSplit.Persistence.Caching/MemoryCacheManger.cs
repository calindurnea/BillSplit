using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace BillSplit.Persistence.Caching;

internal sealed class MemoryCacheManger : ICacheManger
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheManger(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<bool> Exists(string key)
    {
        return Task.FromResult(_memoryCache.TryGetValue(key, out _));
    }

    public Task SetData<T>(string key, T value, TimeSpan? lifetime = null)
    {
        return Task.FromResult(lifetime is not null
            ? _memoryCache.Set(key, JsonConvert.SerializeObject(value), absoluteExpirationRelativeToNow: lifetime.Value)
            : _memoryCache.Set(key, JsonConvert.SerializeObject(value)));
    }

    public Task PrePendData<T>(string key, T value)
    {
        var dataToCache = new List<string>();
        if (_memoryCache.TryGetValue(key, out string? cachedData))
        {
            dataToCache = JsonConvert.DeserializeObject<List<string>>(cachedData!)!;
        }
        
        dataToCache.Add(JsonConvert.SerializeObject(value));
        return Task.FromResult(_memoryCache.Set(key, JsonConvert.SerializeObject(dataToCache)));
    }

    public Task<T[]> GetData<T>(string key, long indexStart = 0, long indexEnd = -1)
    {
        if (!_memoryCache.TryGetValue(key, out string? cachedData))
        {
            return Task.FromResult(Array.Empty<T>());
        }
        
        var parsedCachedData = JsonConvert.DeserializeObject<List<T>>(cachedData!)!
            .Where(data=>data is not null)
            .ToList()
            .GetRange((int)indexStart, (int)(indexEnd - indexStart + 1))
            .ToArray();

        return Task.FromResult(parsedCachedData);
    }

    public Task<bool> RemoveData(string key)
    {
        _memoryCache.Remove(key);
        var keyExist = _memoryCache.TryGetValue(key, out _);
        return Task.FromResult(keyExist);
    }
}