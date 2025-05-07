using BeautySpa.ModelViews.PromotionModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.PromotionValidator
{
    public class POSTPromotionValidator : AbstractValidator<POSTPromotionModelView>
    {
        public POSTPromotionValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.StartDate).LessThan(x => x.EndDate).WithMessage("Start date must be before end date");
            RuleFor(x => x).Must(x => x.DiscountAmount > 0 || x.DiscountPercent > 0)
                .WithMessage("Either DiscountAmount or DiscountPercent must be greater than 0");
        }
    }
}
