namespace Web.Hubs.Core.Contracts.Services;

public interface IStorage<T>
    where T : struct
{
    Task<string[]> Get(T[] keys);

    Task Add(T key, string value);

    Task Delete(T key, string value);

    Task Flush();
}
