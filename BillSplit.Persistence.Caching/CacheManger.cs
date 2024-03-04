using Newtonsoft.Json;
using StackExchange.Redis;

namespace BillSplit.Persistence.Caching;

internal sealed class CacheManger : ICacheManger
{
    private readonly IDatabaseAsync _database;

    public CacheManger(IDatabaseAsync database)
    {
        _database = database;
    }

    public async Task<bool> Exists(string key)
    {
        return await _database.KeyExistsAsync(key);
    }

    public async Task SetData<T>(string key, T value, TimeSpan? lifetime = null)
    {
        await _database.StringSetAsync(key, JsonConvert.SerializeObject(value), lifetime);
    }

    public async Task PrePendData<T>(string key, T value)
    {
        await _database.ListLeftPushAsync(key, JsonConvert.SerializeObject(value));
    }

    public async Task<T[]> GetData<T>(string key, long indexStart = 0, long indexEnd = -1)
    {
        var entries = await _database.ListRangeAsync(key, indexStart, indexEnd);

        return entries
            .Where(redisValue => redisValue.HasValue)
            .Select(redisValue => JsonConvert.DeserializeObject<T>(redisValue.ToString()))
            .OfType<T>()
            .ToArray();
    }

    public async Task<bool> RemoveData(string key)
    {
        var keyExist = await _database.KeyExistsAsync(key);
        return keyExist && await _database.KeyDeleteAsync(key);
    }
}