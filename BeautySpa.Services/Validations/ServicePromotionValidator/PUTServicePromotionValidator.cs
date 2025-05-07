using BeautySpa.ModelViews.ServicePromotionModelView;
using FluentValidation;

namespace BeautySpa.Services.Validations.ServicePromotionValidator
{
    public class PUTServicePromotionValidator : AbstractValidator<PUTServicePromotionModelView>
    {
        public PUTServicePromotionValidator()
        {
            Include(new POSTServicePromotionValidator());
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}