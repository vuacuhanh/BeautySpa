using BeautySpa.ModelViews.LocationModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.LocationValidator
{
    public class POSTLocationSpaValidator : AbstractValidator<POSTLocationSpaModelView>
    {
        public POSTLocationSpaValidator()
        {
            RuleFor(x => x.Street).NotEmpty().MaximumLength(150);
            RuleFor(x => x.City).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
            RuleFor(x => x.BranchId).NotEmpty();
        }
    }
}
