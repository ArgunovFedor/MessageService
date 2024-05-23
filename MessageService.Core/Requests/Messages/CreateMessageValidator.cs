using FluentValidation;

namespace MessageService.Core.Requests.Messages;

public class CreateMessageValidator : AbstractValidator<CreateMessage>
{
    public CreateMessageValidator()
    {
        RuleFor(x => x.Text).MaximumLength(128).NotEmpty();
        RuleFor(x => x.Number).NotEmpty();
    }
}
