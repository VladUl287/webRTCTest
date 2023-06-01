using Web.Hubs.Core.Dtos.Chats;
using Web.Hubs.Core.Entities;

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
                Name = chat.Name,
                Image = chat.Image,

                Message = chat.Messages
                    .OrderByDescending(m => m.Date)
                    .Select(m => m.Content)
                    .FirstOrDefault(),

                Unread = chat.Messages
                    .Count(msg => msg.UserId != userId && msg.Date >= chat.ChatUsers
                        .Where(ch => ch.UserId == userId)
                        .Select(m => m.LastRead)
                        .FirstOrDefault()
                    )
            });
    }
}
