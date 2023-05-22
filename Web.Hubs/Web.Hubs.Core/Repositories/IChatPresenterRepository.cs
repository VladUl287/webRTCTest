using Web.Hubs.Core.Dtos.Chats;

namespace Web.Hubs.Core.Repositories;

public interface IDialogPresenterRepository
{
    Task<ChatData[]> GetDialogs(long userId);

    Task<ChatInfo> GetDialogInfo(Guid dialogId, long userId);
}