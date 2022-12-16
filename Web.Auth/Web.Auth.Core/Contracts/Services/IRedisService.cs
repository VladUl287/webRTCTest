namespace Web.Auth.Core.Contracts.Services;

public interface IRedisService
{
    Task SetItem<T>(string key, T value);

    Task GetItem(string key);

    Task<T> GetItem<T>(string key, T value);

    Task DeleteItem(string key);

    Task DeleteValue<T>(string key, T value);
}