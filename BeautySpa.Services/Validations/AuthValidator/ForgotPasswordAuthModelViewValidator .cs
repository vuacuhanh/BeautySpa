using BeautySpa.ModelViews.AuthModelViews;
using FluentValidation;

public class ForgotPasswordAuthModelViewValidator : AbstractValidator<ForgotPasswordAuthModelView>
{
    public ForgotPasswordAuthModelViewValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
    }
}
