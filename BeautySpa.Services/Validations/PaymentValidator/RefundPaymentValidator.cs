using BeautySpa.ModelViews.PaymentModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.PaymentValidator
{
    public class RefundPaymentValidator : AbstractValidator<RefundPaymentModelView>
    {
        public RefundPaymentValidator()
        {
            RuleFor(x => x.AppointmentId).NotEmpty();
            RuleFor(x => x.Reason).NotEmpty().WithMessage("Refund reason must be provided");
        }
    }
}