using BeautySpa.ModelViews.AuthModelViews;
using FluentValidation;

namespace BeautySpa.Validations.Auth
{
    public class OtpVerifyModelViewValidator : AbstractValidator<OtpVerifyModelView>
    {
        public OtpVerifyModelViewValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.OtpCode)
                .NotEmpty().WithMessage("OTP code is required.");

            RuleFor(x => x.IpAddress)
                .NotEmpty().WithMessage("IP address is required.");

            RuleFor(x => x.DeviceInfo)
                .NotEmpty().WithMessage("Device info is required.");
        }
    }
}
