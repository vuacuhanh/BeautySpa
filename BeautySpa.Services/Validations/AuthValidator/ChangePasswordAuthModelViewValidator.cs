using BeautySpa.ModelViews.AuthModelViews;
using FluentValidation;

public class ChangePasswordAuthModelViewValidator : AbstractValidator<ChangePasswordAuthModelView>
{
    public ChangePasswordAuthModelViewValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(6).WithMessage("New password must be at least 6 characters.");

        RuleFor(x => x.OtpCode)
            .NotEmpty().WithMessage("OTP Code is required.");
    }
}
