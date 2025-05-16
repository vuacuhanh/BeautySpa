using BeautySpa.ModelViews.MoMoModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.MomoValidator
{
    public class RefundValidator : AbstractValidator<RefundRequest>
    {
        public RefundValidator()
        {
            RuleFor(x => x.RequestId).NotEmpty();
            RuleFor(x => x.OrderId).NotEmpty();
            RuleFor(x => x.TransId).GreaterThan(0);
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}