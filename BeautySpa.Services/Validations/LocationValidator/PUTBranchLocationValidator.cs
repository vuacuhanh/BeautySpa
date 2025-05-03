using BeautySpa.ModelViews.LocationModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.LocationValidator
{
    public class PUTBranchLocationValidator : AbstractValidator<PUTBranchLocationModelView>
    {
        public PUTBranchLocationValidator()
        {
            Include(new POSTBranchLocationValidator());
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
