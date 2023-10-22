using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Dtos.Chats;
using System.ComponentModel.DataAnnotations;

namespace Web.Hubs.Core.Contracts.Services;

public interface IChatService
{
    Task<OneOf<Guid, ValidationResult, Error<string>>> Create(CreateChatDto chatCreate, long userId);
}
