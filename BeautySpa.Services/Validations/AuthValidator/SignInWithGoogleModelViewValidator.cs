using BeautySpa.ModelViews.AuthModelViews;
using FluentValidation;

public class SignInWithGoogleModelViewValidator : AbstractValidator<SignInWithGoogleModelView>
{
    public SignInWithGoogleModelViewValidator()
    {
        RuleFor(x => x.IdToken)
            .NotEmpty().WithMessage("Google IdToken is required.");
    }
}
