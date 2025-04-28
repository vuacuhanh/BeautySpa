using BeautySpa.ModelViews.AuthModelViews;
using FluentValidation;

public class ResetPasswordAuthModelViewValidator : AbstractValidator<ResetPasswordAuthModelView>
{
    public ResetPasswordAuthModelViewValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.OtpCode)
            .NotEmpty().WithMessage("OTP Code is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(6).WithMessage("New password must be at least 6 characters.");
    }
}
