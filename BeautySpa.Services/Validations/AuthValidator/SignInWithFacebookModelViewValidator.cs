using BeautySpa.ModelViews.AuthModelViews;
using FluentValidation;

public class SignInWithFacebookModelViewValidator : AbstractValidator<SignInWithFacebookModelView>
{
    public SignInWithFacebookModelViewValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Facebook AccessToken is required.");
    }
}
