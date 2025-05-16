using BeautySpa.ModelViews.VnPayModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.VnPayValidator
{
    public class RefundVnPayValidator : AbstractValidator<RefundVnPayRequest>
    {
        public RefundVnPayValidator()
        {
            RuleFor(x => x.TransactionId).NotEmpty().WithMessage("TransactionId is required");
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.Reason).NotEmpty().WithMessage("Refund reason is required");
        }
    }
}