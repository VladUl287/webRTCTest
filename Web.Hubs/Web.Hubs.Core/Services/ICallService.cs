namespace Web.Hubs.Core.Services;

public interface ICallService
{
    Task AddUser(Guid callId, long userId);

    Task<bool> UserExists(long userId);

    Task<bool> UserExists(Guid callId, long userId);

    Task RemoveUser(Guid callId, long userId);
}
