using BeautySpa.ModelViews.AuthModelViews;
using FluentValidation;

namespace BeautySpa.Validations.Auth
{
    public class SignInAuthModelViewValidator : AbstractValidator<SignInAuthModelView>
    {
        public SignInAuthModelViewValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}
