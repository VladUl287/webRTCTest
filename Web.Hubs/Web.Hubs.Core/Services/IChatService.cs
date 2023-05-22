using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Dtos.Chats;
using Web.Hubs.Core.Results;

namespace Web.Hubs.Core.Services;

public interface IDialogService
{
    Task<ChatData> CreateDialog(ChatCreate chatCreate);

    Task<OneOf<Guid, AlreadyExists>> AddUser(Guid dialogId, long userId, long newUserId);

    Task<OneOf<Success, NotFound>> UpdateDialog(ChatUpdate dialog);

    Task RemoveDialog(Guid chatId, long userId);

    Task RemoveUsers(Guid chatId, long creatorId, long[] usersIds);
}
