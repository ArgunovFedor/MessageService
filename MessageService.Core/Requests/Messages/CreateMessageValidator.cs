using FluentValidation;

namespace MessageService.Core.Requests.Messages;

public class CreateMessageValidator : AbstractValidator<CreateMessage>
{
    public CreateMessageValidator()
    {
        RuleFor(x => x.CreatedOn).NotEmpty();
        RuleFor(x => x.ModifiedOn).NotEmpty();
        RuleFor(x => x.CreatedBy);
        RuleFor(x => x.ModifiedBy);
        RuleFor(x => x.Text).MaximumLength(128).NotEmpty();
        RuleFor(x => x.Number).NotEmpty();
    }
}
