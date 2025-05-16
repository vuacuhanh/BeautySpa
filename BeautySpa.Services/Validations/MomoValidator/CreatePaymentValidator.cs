using BeautySpa.ModelViews.MoMoModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.MomoValidator
{
    public class CreatePaymentValidator : AbstractValidator<CreatePaymentRequest>
    {
        public CreatePaymentValidator()
        {
            RuleFor(x => x.OrderId).NotEmpty();
            RuleFor(x => x.OrderInfo).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
        }
    }
}