using BeautySpa.ModelViews.PayPalModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.PayPalPaymentValidator
{
    public class CreatePayPalPaymentValidator : AbstractValidator<CreatePayPalPaymentRequest>
    {
        public CreatePayPalPaymentValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than 0.");
            RuleFor(x => x.Currency).NotEmpty().WithMessage("Currency is required.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
            RuleFor(x => x.ReturnUrl).NotEmpty().WithMessage("Return URL is required.");
            RuleFor(x => x.CancelUrl).NotEmpty().WithMessage("Cancel URL is required.");
        }
    }
}
