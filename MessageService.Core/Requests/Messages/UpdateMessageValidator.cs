using FluentValidation;

namespace MessageService.Core.Requests.Messages;

public class UpdateMessageValidator : AbstractValidator<UpdateMessage>
{
    public UpdateMessageValidator()
    {
        RuleFor(x => x.CreatedOn).NotEmpty();
        RuleFor(x => x.ModifiedOn).NotEmpty();
        RuleFor(x => x.CreatedBy);
        RuleFor(x => x.ModifiedBy);
        RuleFor(x => x.Text).MaximumLength(128).NotEmpty();
        RuleFor(x => x.Number).NotEmpty();
    }
}
