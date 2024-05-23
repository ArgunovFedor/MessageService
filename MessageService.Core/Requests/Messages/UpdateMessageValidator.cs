using FluentValidation;

namespace MessageService.Core.Requests.Messages;

public class UpdateMessageValidator : AbstractValidator<UpdateMessage>
{
    public UpdateMessageValidator()
    {
        RuleFor(x => x.Text).MaximumLength(128).NotEmpty();
        RuleFor(x => x.Number).NotEmpty();
    }
}
