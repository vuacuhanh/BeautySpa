using BeautySpa.ModelViews.MoMoModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.MomoValidator
{
    public class CreatePaymentValidator : AbstractValidator<CreatePaymentRequest>
    {
        public CreatePaymentValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("Mã đơn hàng không được để trống");

            RuleFor(x => x.OrderInfo)
                .NotEmpty().WithMessage("Thông tin đơn hàng không được để trống");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Số tiền phải lớn hơn 0");
        }
    }
}
