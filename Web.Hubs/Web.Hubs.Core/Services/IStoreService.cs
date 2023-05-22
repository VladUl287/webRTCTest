namespace Web.Hubs.Core.Services;

public interface IStoreService<TKey>
{
    Task Add(TKey key, string content);

    Task Delete(TKey key);
}
