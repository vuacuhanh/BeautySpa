using BeautySpa.ModelViews.ServiceProviderModelViews;
using FluentValidation;

public class PUTServiceProviderModelViewsValidator : AbstractValidator<PUTServiceProviderModelViews>
{
    public PUTServiceProviderModelViewsValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.BusinessName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^[0-9]{10,11}$");
        RuleFor(x => x.ProviderId).NotEmpty();
    }
}
