using BeautySpa.ModelViews.LocationModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.LocationValidator
{
    public class POSTBranchLocationValidator : AbstractValidator<POSTBranchLocationModelView>
    {
        public POSTBranchLocationValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}
