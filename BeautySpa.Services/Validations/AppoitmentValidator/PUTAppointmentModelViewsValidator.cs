using BeautySpa.ModelViews.AppointmentModelViews;
using FluentValidation;


namespace BeautySpa.Services.Validations.AppoitmentValidator
{
    public class PUTAppointmentModelViewsValidator : AbstractValidator<PUTAppointmentModelViews>
    {
        public PUTAppointmentModelViewsValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

}
