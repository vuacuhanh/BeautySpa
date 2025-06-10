using BeautySpa.ModelViews.PaymentModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.PaymentValidator
{
    public class POSTPaymentValidator : AbstractValidator<POSTPaymentModelView>
    {
        public POSTPaymentValidator()
        {
            RuleFor(x => x.AppointmentId).NotEmpty();
            RuleFor(x => x.PaymentMethod).NotEmpty().Must(m => m == "momo" || m == "vnpay"|| m == "paypal")
                .WithMessage("Phương thức thanh toán không hợp lệ");
        }
    }
}