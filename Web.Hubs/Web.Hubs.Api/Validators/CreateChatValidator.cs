using FluentValidation;
using Web.Hubs.Core.Dtos.Chats;

namespace Web.Hubs.Api.Validators;

public sealed class CreateChatValidator : AbstractValidator<CreateChatDto>
{
    public CreateChatValidator()
    {
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
            .GreaterThanOrEqualTo(1);
    }
}
