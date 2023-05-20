using Web.Hub.Core.Dtos.Dialogs;

namespace Web.Hub.Core.Services;

public interface IDialogService
{
    Task<DialogData> CreateDialog(DialogCreate dialogCreate);
}
