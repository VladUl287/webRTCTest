using OneOf;
using OneOf.Types;
using Web.Hub.Core.Results;
using Web.Hub.Core.Dtos.Dialogs;

namespace Web.Hub.Core.Repositories;

public interface IDialogManagerRepository : IBaseRepository
{
    Task<Guid> AddDialog(DialogCreate dialog);

    Task<OneOf<Guid, AlreadyExists>> AddUser(Guid dialogId, long userId);

    Task<OneOf<Success, NotFound>> UpdateDialog(DialogUpdate dialog);

    Task RemoveDialog(Guid dialogId, long userId);

    Task RemoveDialogUser(Guid dialogId, long userId);
}