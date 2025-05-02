using BeautySpa.ModelViews.AppointmentModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.AppoitmentValidator
{
    public class POSTAppointmentModelViewsValidator : AbstractValidator<POSTAppointmentModelViews>
    {
        public POSTAppointmentModelViewsValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.ProviderId).NotEmpty();
            RuleFor(x => x.ServiceId).NotEmpty();
        }
    }

}
