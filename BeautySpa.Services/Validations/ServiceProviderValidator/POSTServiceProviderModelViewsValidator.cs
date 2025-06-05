using BeautySpa.ModelViews.ServiceProviderModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.ServiceProviderValidator
{
    public class POSTServiceProviderModelViewsValidator : AbstractValidator<POSTServiceProviderModelViews>
    {
        public POSTServiceProviderModelViewsValidator()
        {
            RuleFor(x => x.BusinessName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^[0-9]{10,11}$");
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}