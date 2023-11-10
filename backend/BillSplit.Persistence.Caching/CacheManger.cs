using Newtonsoft.Json;
using StackExchange.Redis;

namespace BillSplit.Persistence.Caching;

internal sealed class CacheManger : ICacheManger
{
    private readonly IDatabase _database;

    public CacheManger(IDatabase database)
    {
        _database = database;
    }

    public bool Exists(string key)
    {
        return _database.KeyExists(key);
    }

    public T? GetData<T>(string key)
    {
        var value = _database.StringGet(key);
        return value.HasValue ? JsonConvert.DeserializeObject<T>(value!) : default;
    }

    public void SetData<T>(string key, T value, TimeSpan? lifetime = null)
    {
        _database.StringSet(key, JsonConvert.SerializeObject(value), lifetime);
    }

    public bool RemoveData(string key)
    {
        var keyExist = _database.KeyExists(key);
        return keyExist && _database.KeyDelete(key);
    }
}