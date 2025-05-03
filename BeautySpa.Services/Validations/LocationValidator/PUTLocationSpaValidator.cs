using BeautySpa.ModelViews.LocationModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.LocationValidator
{
    public class PUTLocationSpaValidator : AbstractValidator<PUTLocationSpaModelView>
    {
        public PUTLocationSpaValidator()
        {
            Include(new POSTLocationSpaValidator());
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
