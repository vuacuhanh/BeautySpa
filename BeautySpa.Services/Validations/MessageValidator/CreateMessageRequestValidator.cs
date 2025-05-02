using FluentValidation;
using BeautySpa.ModelViews.MessageModelViews;

namespace BeautySpa.Services.Validations.MessageValidator
{
    public class CreateMessageRequestValidator : AbstractValidator<CreateMessageRequest>
    {
        public CreateMessageRequestValidator()
        {
            RuleFor(x => x.SenderId)
                .NotEmpty().WithMessage("SenderId is required.");

            RuleFor(x => x.ReceiverId)
                .NotEmpty().WithMessage("ReceiverId is required.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Message content cannot be empty.")
                .MaximumLength(1000).WithMessage("Content is too long.");

            RuleFor(x => x.SenderType)
                .NotEmpty().WithMessage("SenderType is required.");

            RuleFor(x => x.ReceiverType)
                .NotEmpty().WithMessage("ReceiverType is required.");
        }
    }
}
