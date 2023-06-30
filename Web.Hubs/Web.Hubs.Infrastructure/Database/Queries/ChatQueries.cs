using Web.Hubs.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Web.Hubs.Infrastructure.Database.Queries;

public static class ChatQueries
{
    public static readonly Func<DatabaseContext, long, Task<Guid>> GetMonologId =
        EF.CompileAsyncQuery((DatabaseContext context, long userId) =>
            context.ChatsUsers
                .Where(cu => cu.UserId == userId && cu.Chat!.Type == ChatType.Monolog)
                .Select(cu => cu.ChatId)
                .FirstOrDefault()
        );

    public static readonly Func<DatabaseContext, long, long, Task<Guid>> GetDialogId =
        EF.CompileAsyncQuery((DatabaseContext context, long firstCollocutor, long secondCollocutor) =>
            context.ChatsUsers
                .Where(cu => cu.Chat!.Type == ChatType.Dialog && cu.UserId == firstCollocutor)
                .Select(cu => cu.ChatId)
                      .Intersect(
                        context.ChatsUsers
                            .Where(cu => cu.Chat!.Type == ChatType.Dialog && cu.UserId == secondCollocutor)
                            .Select(cu => cu.ChatId)
                        )
                .FirstOrDefault()
        );

}