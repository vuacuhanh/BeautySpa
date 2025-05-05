using BeautySpa.ModelViews.PromotionAdminModelView;
using FluentValidation;

public class POSTPromotionAdminValidator : AbstractValidator<POSTPromotionAdminModelView>
{
    public POSTPromotionAdminValidator()
    {
        RuleFor(x => x.PromotionName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.StartDate).LessThan(x => x.EndDate);
        RuleFor(x => x.RankIds).NotEmpty();
        RuleFor(x => x.DiscountAmount).GreaterThan(0).When(x => x.DiscountPercent == null);
        RuleFor(x => x.DiscountPercent).InclusiveBetween(0.01m, 100).When(x => x.DiscountAmount == null);
    }
}
