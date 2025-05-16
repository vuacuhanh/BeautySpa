using BeautySpa.ModelViews.AppointmentModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.AppoitmentValidator
{
    public class POSTAppointmentModelViewValidator : AbstractValidator<POSTAppointmentModelView>
    {
        public POSTAppointmentModelViewValidator()
        {
            RuleFor(x => x.AppointmentDate).NotEmpty();
            RuleFor(x => x.StartTime).NotEmpty();
            RuleFor(x => x.SpaBranchLocationId).NotEmpty();
            RuleFor(x => x.Services).NotEmpty().WithMessage("At least one service must be selected.");
        }
    }
}
