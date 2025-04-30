using BeautySpa.ModelViews.UserModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.UserValidator
{
    public class PUTuserforcustomerValidator : AbstractValidator<PUTuserforcustomer>
    {
        public PUTuserforcustomerValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.UserInfor.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");
        }
    }
}
