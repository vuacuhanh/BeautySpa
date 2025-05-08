using BeautySpa.ModelViews.ServicePromotionModelView;
using FluentValidation;

namespace BeautySpa.Services.Validations.ServicePromotionValidator
{
    public class POSTServicePromotionValidator : AbstractValidator<POSTServicePromotionModelView>
    {
        public POSTServicePromotionValidator()
        {
            RuleFor(x => x.ServiceId)
                .NotEmpty()
                .WithMessage("ServiceId is required.");

            RuleFor(x => x.StartDate)
                .LessThan(x => x.EndDate)
                .WithMessage("StartDate must be earlier than EndDate.");


            RuleFor(x => x)
                .Must(x =>
                {
                    bool hasPercent = x.DiscountPercent > 0;
                    bool hasAmount = x.DiscountAmount > 0;
                    return hasPercent ^ hasAmount; // XOR: chỉ một trong hai
                })
                .WithMessage("Only one of DiscountPercent or DiscountAmount should be provided and must be greater than 0.");
        }
    }
}