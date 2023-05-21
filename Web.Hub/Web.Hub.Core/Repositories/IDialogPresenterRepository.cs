using Web.Hub.Core.Dtos.Dialogs;

namespace Web.Hub.Core.Repositories;

public interface IDialogPresenterRepository
{
    Task<DialogData[]> GetDialogs(long userId);

    Task<DialogInfo> GetDialogInfo(Guid dialogId, long userId);
}