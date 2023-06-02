namespace Web.Hubs.Core.Services;

public interface IConnectionService
{
    Task<string[]> Get(long[] userIds);

    Task Add(long userId, string value);

    Task Delete(long userId, string value);

    Task Flush();
}
