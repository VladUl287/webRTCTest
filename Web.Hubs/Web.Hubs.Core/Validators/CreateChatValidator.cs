using FluentValidation;
using FluentValidation.Results;
using Web.Hubs.Core.Dtos.Chats;

namespace Web.Hubs.Core.Validators;

public sealed class CreateChatValidator : AbstractValidator<CreateChatDto>
{
    public CreateChatValidator()
    {
        RuleFor(chat => chat)
            .Custom((chatDto, context) =>
            {
                if (chatDto.Type is Core.Enums.ChatType.Monolog)
                {
                    if (chatDto is { Users.Length: not 1 })
                    {
                        context.AddFailure("The monolog cannot contain more or less than one user");
                        return;
                    }

                    var user = chatDto.Users[0];

                    if (user is null || user.Id != chatDto.UserId)
                    {
                        context.AddFailure("The monolog creator user not equal member user");
                        return;
                    }
                }

                if (chatDto.Type is Core.Enums.ChatType.Dialog)
                {
                    if (chatDto.Users is { Length: not 2 })
                    {
                        context.AddFailure("The dialog cannot contain more or less than two users");
                        return;
                    }

                    var firstCollocutor = chatDto.Users[0];
                    var secondCollocutor = chatDto.Users[1];

                    if (firstCollocutor is null || secondCollocutor is null)
                    {
                        context.AddFailure("Some of users not correct");
                        return;
                    }

                    if (firstCollocutor.Id == secondCollocutor.Id)
                    {
                        context.AddFailure("Some of users not correct");
                        return;
                    }
                }
            });

        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(p => p.Image)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(p => p.Type)
            .NotEmpty()
            .IsInEnum();

        RuleFor(p => p.UserId)
            .NotEmpty()
            .GreaterThanOrEqualTo(1)
            .WithMessage("Not correct user identity");
    }

    protected override bool PreValidate(ValidationContext<CreateChatDto> context, ValidationResult result)
    {
        if (context.InstanceToValidate is null)
        {
            result.Errors.Add(new ValidationFailure(string.Empty, "Chat data was not supplied"));

            return false;
        }

        if (context.InstanceToValidate.Users is null or { Length: 0 })
        {
            result.Errors.Add(new ValidationFailure(string.Empty, "Chat data was not correct"));

            return false;
        }

        return true;
    }
}
