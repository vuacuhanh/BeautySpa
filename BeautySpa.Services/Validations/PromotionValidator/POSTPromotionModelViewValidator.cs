using BeautySpa.ModelViews.PromotionModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.PromotionValidator
{
    public class POSTPromotionValidator : AbstractValidator<POSTPromotionModelView>
    {
        public POSTPromotionValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title must not be empty")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date must not be empty");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date must not be empty")
                .GreaterThan(x => x.StartDate)
                .WithMessage("End date must be after start date");

            RuleFor(x => x)
                .Must(x =>
                    (x.DiscountPercent > 0 && (x.DiscountAmount == null || x.DiscountAmount == 0)) ||
                    (x.DiscountAmount > 0 && (x.DiscountPercent == null || x.DiscountPercent == 0)))
                .WithMessage("Only one of DiscountPercent or DiscountAmount must be greater than 0");
        }
    }
}
