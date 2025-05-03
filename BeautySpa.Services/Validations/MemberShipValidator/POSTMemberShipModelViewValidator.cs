using BeautySpa.ModelViews.MemberShipModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.MemberShipValidator
{
    public class POSTMemberShipModelViewValidator : AbstractValidator<POSTMemberShipModelView>
    {
        public POSTMemberShipModelViewValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.InitialPoints).GreaterThanOrEqualTo(0);
        }
    }
}
