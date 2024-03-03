namespace BillSplit.Persistence.Caching;

public interface ICacheManger
{
    Task SetData<T>(string key, T value, TimeSpan? lifetime = null);
    Task<bool> RemoveData(string key);
    Task<bool> Exists(string key);
    Task PrePendData<T>(string key, T value);
    Task<T[]> GetData<T>(string key, long indexStart = 0, long indexEnd = -1);
}