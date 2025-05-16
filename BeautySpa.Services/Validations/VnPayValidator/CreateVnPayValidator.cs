using BeautySpa.ModelViews.VnPayModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.VnPayValidator
{
    public class CreateVnPayValidator : AbstractValidator<CreateVnPayRequest>
    {
        public CreateVnPayValidator()
        {
            RuleFor(x => x.AppointmentId).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero");
            RuleFor(x => x.ReturnUrl).NotEmpty().WithMessage("Return URL is required");
            RuleFor(x => x.OrderInfo).NotEmpty().WithMessage("Order info is required");
        }
    }
}