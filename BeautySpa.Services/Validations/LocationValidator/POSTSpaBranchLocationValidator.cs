using BeautySpa.ModelViews.LocationModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.LocationValidator
{
    public class POSTSpaBranchLocationValidator : AbstractValidator<POSTSpaBranchLocationModelView>
    {
        public POSTSpaBranchLocationValidator()
        {
            RuleFor(x => x.ServiceProviderId).NotEmpty();
            RuleFor(x => x.BranchName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Street).NotEmpty().MaximumLength(200);
            RuleFor(x => x.District).NotEmpty().MaximumLength(100);
            RuleFor(x => x.City).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
            RuleFor(x => x.ProvinceId).NotEmpty();
            RuleFor(x => x.DistrictId).NotEmpty();
        }
    }
}