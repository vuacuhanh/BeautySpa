using BeautySpa.ModelViews.PromotionModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.PromotionValidator
{
    public class PUTPromotionValidator : AbstractValidator<PUTPromotionModelView>
    {
        public PUTPromotionValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Promotion ID must not be empty");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title must not be empty")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date must not be empty");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date must not be empty")
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("End date must be greater than or equal to start date");

            RuleFor(x => x)
                .Must(x =>
                    (x.DiscountPercent > 0 && (x.DiscountAmount == null || x.DiscountAmount == 0)) ||
                    (x.DiscountAmount > 0 && (x.DiscountPercent == null || x.DiscountPercent == 0)))
                .WithMessage("Only one of DiscountPercent or DiscountAmount must be greater than 0");
        }
    }

}
