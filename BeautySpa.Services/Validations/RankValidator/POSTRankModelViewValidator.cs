using BeautySpa.ModelViews.RankModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.RankValidator
{
    public class POSTRankModelViewValidator : AbstractValidator<POSTRankModelView>
    {
        public POSTRankModelViewValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Rank name is required.");
            RuleFor(x => x.MinPoints).GreaterThanOrEqualTo(0).WithMessage("Minimum points must be non-negative.");
            RuleFor(x => x.DiscountPercent).GreaterThanOrEqualTo(0).When(x => x.DiscountPercent.HasValue);
        }
    }
}
