using FluentValidation;
using Web.Hubs.Core.Dtos.Messages;

namespace Web.Hubs.Core.Validators;

public sealed class CreateMessageValidator : AbstractValidator<CreateMessageDto>
{
    public CreateMessageValidator()
    {
        RuleFor(p => p.ChatId)
            .NotEmpty();

        RuleFor(p => p.Content)
            .NotEmpty()
            .MaximumLength(5000);
    }
}
