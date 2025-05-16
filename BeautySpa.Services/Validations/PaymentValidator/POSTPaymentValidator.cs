using BeautySpa.ModelViews.PaymentModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.PaymentValidator
{
    public class POSTPaymentValidator : AbstractValidator<POSTPaymentModelView>
    {
        public POSTPaymentValidator()
        {
            RuleFor(x => x.AppointmentId).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Deposit amount must be greater than 0");
            RuleFor(x => x.PaymentMethod).NotEmpty();
        }
    }
}