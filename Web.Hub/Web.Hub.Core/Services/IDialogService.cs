using OneOf;
using OneOf.Types;
using Web.Hub.Core.Dtos.Dialogs;
using Web.Hub.Core.Results;

namespace Web.Hub.Core.Services;

public interface IDialogService
{
    Task<DialogData> CreateDialog(DialogCreate dialogCreate);

    Task<OneOf<Guid, AlreadyExists>> AddUser(Guid dialogId, long userId, long newUserId);

    Task<OneOf<Success, NotFound>> UpdateDialog(DialogUpdate dialog);

    Task RemoveDialog(Guid dialogId, long userId);

    Task RemoveUsers(Guid dialogId, long userId, long[] removeUsersIds);
}
