using BeautySpa.ModelViews.MoMoModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.MomoValidator
{
    public class RefundValidator : AbstractValidator<RefundRequest>
    {
        public RefundValidator()
        {
            RuleFor(x => x.RequestId)
                .NotEmpty().WithMessage("RequestId không được để trống");

            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("OrderId không được để trống");

            RuleFor(x => x.TransId)
                .GreaterThan(0).WithMessage("TransId phải lớn hơn 0");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Số tiền hoàn phải lớn hơn 0");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Mô tả hoàn tiền không được để trống");
        }
    }
}
