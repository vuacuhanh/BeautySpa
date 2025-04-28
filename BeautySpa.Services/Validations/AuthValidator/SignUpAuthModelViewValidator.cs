using BeautySpa.ModelViews.AuthModelViews;
using FluentValidation;

namespace BeautySpa.Validations.Auth
{
    public class SignUpAuthModelViewValidator : AbstractValidator<SignUpAuthModelView>
    {
        public SignUpAuthModelViewValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

            RuleFor(x => x.ConfirmOtp)
                .NotEmpty().WithMessage("OTP is required.");
        }
    }
}
