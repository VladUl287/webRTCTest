namespace Web.Hubs.Core.Services;

public interface IStoreService<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{    
    Task<bool> Has(TKey key);

    Task<bool> Has(TKey key, TValue value);

    Task<TValue[]> Get(TKey key);

    Task<TValue[]> Get(TKey[] key);

    Task Add(TKey key, TValue value);

    Task Delete(TKey key, TValue value);
}
