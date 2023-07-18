using FluentValidation;
using FluentValidation.Results;
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

    protected override bool PreValidate(ValidationContext<CreateMessageDto> context, ValidationResult result)
    {
        var messageDto = context?.InstanceToValidate;

        if (messageDto is null)
        {
            result.Errors.Add(new(string.Empty, "Message data was not provided"));

            return false;
        }

        return true;
    }
}
