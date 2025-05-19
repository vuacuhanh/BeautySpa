using BeautySpa.ModelViews.AppointmentModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.AppoitmentValidator
{
    public class PUTAppointmentModelViewValidator : AbstractValidator<PUTAppointmentModelView>
    {
        public PUTAppointmentModelViewValidator()
        {
            Include(new POSTAppointmentModelViewValidator());
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
