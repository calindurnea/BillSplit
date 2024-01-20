namespace BillSplit.Persistence.Caching;

public interface ICacheManger
{
    Task SetData<T>(string key, T value, TimeSpan? lifetime = null);
    Task<bool> RemoveData(string key);
    Task<bool> Exists(string key);
}