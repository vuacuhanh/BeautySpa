using BeautySpa.ModelViews.LocationModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.LocationValidator
{
    public class POSTSpaBranchLocationValidator : AbstractValidator<POSTSpaBranchLocationModelView>
    {
        public POSTSpaBranchLocationValidator()
        {
            RuleFor(x => x.BranchName).NotEmpty();
            RuleFor(x => x.Street).NotEmpty();
            RuleFor(x => x.City).NotEmpty();
            RuleFor(x => x.District).NotEmpty();
        }
    }
}