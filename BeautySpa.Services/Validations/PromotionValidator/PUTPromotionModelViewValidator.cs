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

            RuleFor(x => x.DiscountPercent)
                .InclusiveBetween(0, 100).When(x => x.DiscountPercent.HasValue)
                .WithMessage("Discount percent must be between 0 and 100");

            RuleFor(x => x.DiscountAmount)
                .GreaterThan(0).When(x => x.DiscountAmount.HasValue)
                .WithMessage("Discount amount must be greater than 0");

            RuleFor(x => x)
                .Must(x => x.DiscountPercent.HasValue ^ x.DiscountAmount.HasValue)
                .WithMessage("Only one of DiscountPercent or DiscountAmount must be provided, not both");
        }
    }
}
