using BeautySpa.ModelViews.PromotionModelViews;
using FluentValidation;


namespace BeautySpa.Services.Validations.PromotionValidator
{
    public class PUTPromotionModelViewValidator : AbstractValidator<PUTPromotionModelViews>
    {
        public PUTPromotionModelViewValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id không được để trống");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Tiêu đề không được để trống")
                .MaximumLength(200).WithMessage("Tiêu đề không được vượt quá 200 ký tự");

            RuleFor(x => x.ProviderId)
                .NotEmpty().WithMessage("ProviderId không được để trống");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Ngày bắt đầu không được để trống");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("Ngày kết thúc không được để trống")
                .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu");

            RuleFor(x => x.DiscountPercent)
              .InclusiveBetween(0, 100).When(x => x.DiscountPercent.HasValue)
              .WithMessage("Phần trăm giảm giá phải từ 0 đến 100");

            RuleFor(x => x.DiscountAmount)
                .GreaterThan(0).When(x => x.DiscountAmount.HasValue)
                .WithMessage("Giá trị giảm phải lớn hơn 0");

            RuleFor(x => x)
                .Must(x => x.DiscountPercent.HasValue ^ x.DiscountAmount.HasValue)
                .WithMessage("Chỉ được nhập MỘT trong hai: phần trăm giảm giá HOẶC số tiền giảm giá.");
        }
    }
}
