using Web.Hubs.Core.Services;

namespace Web.Hubs.Infrastructure.Services;

public sealed class StoreService<TKey, TValue> : IStoreService<TKey, TValue>
{
    public Task Add(TKey key, TValue value)
    {
        throw new NotImplementedException();
    }

    public Task Delete(TKey key, TValue value)
    {
        throw new NotImplementedException();
    }

    public Task<TValue[]> Get(TKey key)
    {
        throw new NotImplementedException();
    }

    public Task<TValue[]> Get(TKey[] key)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Has(TKey key)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Has(TKey key, TValue value)
    {
        throw new NotImplementedException();
    }
}
