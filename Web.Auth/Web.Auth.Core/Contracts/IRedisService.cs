namespace Web.Auth.Core.Contracts;

public interface IRedisService
{
    Task AddOrCreateValue<T>(string key, T value);

    Task<T> GetAndDeleteValue<T>(string key, T value);

    Task DeleteValue<T>(string key, T value);

    Task DeleteItem(string key);
}