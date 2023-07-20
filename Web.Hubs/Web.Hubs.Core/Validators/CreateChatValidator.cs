using FluentValidation;
using Web.Hubs.Core.Enums;
using FluentValidation.Results;
using Web.Hubs.Core.Dtos.Chats;

namespace Web.Hubs.Core.Validators;

public sealed class CreateChatValidator : AbstractValidator<CreateChatDto>
{
    public CreateChatValidator()
    {
        RuleFor(p => p.Name)
            .MaximumLength(255)
            .WithMessage("Incorrect chat name");

        RuleFor(p => p.Image)
            .MaximumLength(255)
            .WithMessage("Incorrect chat image");

        RuleFor(p => p.Type)
            .NotEmpty()
            .IsInEnum()
            .WithMessage("Incorrect chat type");

        RuleFor(p => p.UserId)
            .NotEmpty()
            .WithMessage("Incorrect user identity");
    }

    protected override bool PreValidate(ValidationContext<CreateChatDto> context, ValidationResult result)
    {
        var chatDto = context?.InstanceToValidate;

        if (chatDto is null)
        {
            result.Errors.Add(new(string.Empty, "Chat data was not provided"));

            return false;
        }

        if (chatDto is { Users: null or { Length: 0 } })
        {
            result.Errors.Add(new(string.Empty, "Chat users is incorrect"));

            return false;
        }

        if (chatDto.Type is ChatType.Monolog)
        {
            if (chatDto is { Users.Length: not 1 })
            {
                result.Errors.Add(new(string.Empty, "The monolog cannot contain more or less than one user"));

                return false;
            }

            var user = chatDto.Users[0];

            if (user is null || user.Id != chatDto.UserId)
            {
                result.Errors.Add(new(string.Empty, "The monolog creator user not equal member user"));

                return false;
            }
        }

        if (chatDto.Type is ChatType.Dialog)
        {
            if (chatDto.Users is { Length: not 2 })
            {
                result.Errors.Add(new(string.Empty, "The dialog cannot contain more or less than two users"));

                return false;
            }

            var firstUser = chatDto.Users[0];
            var secondUser = chatDto.Users[1];

            if (firstUser is null || secondUser is null)
            {
                result.Errors.Add(new(string.Empty, "Some of users not correct"));

                return false;
            }

            if (firstUser.Id == secondUser.Id)
            {
                result.Errors.Add(new(string.Empty, "The dialog contains the same user twice"));

                return false;
            }
        }

        return true;
    }
}
