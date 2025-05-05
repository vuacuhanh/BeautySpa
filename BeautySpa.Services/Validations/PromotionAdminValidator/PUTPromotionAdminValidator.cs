using BeautySpa.ModelViews.PromotionAdminModelView;
using FluentValidation;

public class PUTPromotionAdminValidator : AbstractValidator<PUTPromotionAdminModelView>
{
    public PUTPromotionAdminValidator()
    {
        Include(new POSTPromotionAdminValidator());
        RuleFor(x => x.Id).NotEmpty();
    }
}
