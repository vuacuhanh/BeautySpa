using BeautySpa.ModelViews.LocationModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.LocationValidator
{
    public class PUTSpaBranchLocationValidator : AbstractValidator<PUTSpaBranchLocationModelView>
    {
        public PUTSpaBranchLocationValidator()
        {
            Include(new POSTSpaBranchLocationValidator());
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}