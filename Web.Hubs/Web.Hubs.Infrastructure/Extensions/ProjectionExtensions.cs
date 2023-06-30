using Web.Hubs.Core.Dtos.Chats;
using Web.Hubs.Core.Dtos.Messages;
using Web.Hubs.Core.Entities;
using Web.Hubs.Core.Enums;

namespace Web.Hubs.Infrastructure.Extensions;

internal static class ProjectionExtensions
{
    public static IQueryable<ChatDto> ProjectToDto(this IQueryable<Chat> queryable, long userId)
    {
        ArgumentNullException.ThrowIfNull(queryable);

        return queryable.Select(
            chat => new ChatDto
            {
                Id = chat.Id,
                Date = chat.Date,
                UserId = chat.UserId,

                Name = chat.Type == ChatType.Dialog ?
                    chat.ChatUsers!.First(x => x.UserId == userId).Name :
                    chat.Name,
                Image = chat.Type == ChatType.Dialog ?
                    chat.ChatUsers!.First(x => x.UserId == userId).Image :
                    chat.Image,

                LastRead = chat.ChatUsers!
                    .Where(cu => cu.UserId == userId)
                    .Select(cu => cu.LastRead)
                    .FirstOrDefault(),

                LastMessage = chat.Messages!
                    .OrderByDescending(m => m.Date)
                    .Select(m => new LastMessageDto
                    {
                        Content = m.Content,
                        Date = m.Date
                    })
                    .FirstOrDefault(),

                Unread = chat.Messages!
                    .Count(m =>
                        m.UserId != userId &&
                        m.Date > chat.ChatUsers!
                            .Where(ch => ch.UserId == userId)
                            .Select(m => m.LastRead)
                            .FirstOrDefault()
                    )
            });
    }
}
