using BeautySpa.ModelViews.AuthModelViews;
using FluentValidation;

namespace BeautySpa.Validations.Auth
{
    public class RefreshTokenRequestModelViewValidator : AbstractValidator<RefreshTokenRequestModelView>
    {
        public RefreshTokenRequestModelViewValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.");
        }
    }
}
