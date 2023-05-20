namespace Web.Hub.Core.Services;

public interface IStoreService<TKey> where TKey : struct
{
    Task Add(TKey key, string userId);

    Task Delete(TKey key);
}
