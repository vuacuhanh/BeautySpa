using BeautySpa.ModelViews.RoleModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.RoleValidator
{
    public class POSTRoleModelViewsValidator : AbstractValidator<POSTRoleModelViews>
    {
        public POSTRoleModelViewsValidator()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("Role name is required.")
                .MaximumLength(50).WithMessage("Role name must not exceed 50 characters.");
        }
    }
}
