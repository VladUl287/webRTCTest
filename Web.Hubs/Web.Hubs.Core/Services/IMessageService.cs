using OneOf;
using OneOf.Types;
using Web.Hubs.Core.Dtos.Messages;
using System.ComponentModel.DataAnnotations;

namespace Web.Hubs.Core.Services;

public interface IMessageService
{
    Task<OneOf<MessageDto, ValidationResult, Error<string>>> Create(CreateMessageDto message, long userId);
}
