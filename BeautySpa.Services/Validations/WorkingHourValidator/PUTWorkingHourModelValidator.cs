using BeautySpa.ModelViews.WorkingHourModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.WorkingHourValidator
{
    public class PUTWorkingHourModelValidator : AbstractValidator<PUTWorkingHourModelViews>
    {
        public PUTWorkingHourModelValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");

            Include(new POSTWorkingHourModelValidator());
        }
    }
}  