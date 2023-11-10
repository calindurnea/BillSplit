namespace BillSplit.Persistence.Caching;

public interface ICacheManger
{
    void SetData<T>(string key, T value, TimeSpan? lifetime = null);
    bool RemoveData(string key);
    bool Exists(string key);
}