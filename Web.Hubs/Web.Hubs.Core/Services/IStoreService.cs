namespace Web.Hubs.Core.Services;

public interface IStoreService<TKey> where TKey : notnull
{
    Task<string[]> Get(TKey key);

    Task<string[]> Get(TKey[] key);

    Task Add(TKey key, string content);

    Task Delete(TKey key, string content);

    Task Delete(TKey key);
}
