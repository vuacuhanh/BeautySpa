using BeautySpa.ModelViews.ServicePromotionModelView;
using FluentValidation;

namespace BeautySpa.Services.Validations.ServicePromotionValidator
{
    public class POSTServicePromotionValidator : AbstractValidator<POSTServicePromotionModelView>
    {
        public POSTServicePromotionValidator()
        {
            RuleFor(x => x.ServiceId).NotEmpty();
            RuleFor(x => x.StartDate).LessThan(x => x.EndDate);
            RuleFor(x => x).Must(x => x.DiscountAmount > 0 || x.DiscountPercent > 0)
                .WithMessage("Either DiscountAmount or DiscountPercent must be greater than 0");
        }
    }
}