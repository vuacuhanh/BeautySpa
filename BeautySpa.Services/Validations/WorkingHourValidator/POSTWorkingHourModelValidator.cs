using BeautySpa.ModelViews.WorkingHourModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.WorkingHourValidator
{
    public class POSTWorkingHourModelValidator : AbstractValidator<POSTWorkingHourModelViews>
    {
        public POSTWorkingHourModelValidator()
        {
            RuleFor(x => x.DayOfWeek)
                .InclusiveBetween(0, 6).WithMessage("DayOfWeek must be between 0 (Sunday) and 6 (Saturday).");

            RuleFor(x => x.OpeningTime)
                .LessThan(x => x.ClosingTime).WithMessage("Opening time must be earlier than closing time.");
        }
    }
}