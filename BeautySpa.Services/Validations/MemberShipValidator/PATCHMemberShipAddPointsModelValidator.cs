using BeautySpa.ModelViews.MemberShipModelViews;
using FluentValidation;

namespace BeautySpa.Services.Validations.MemberShipValidator
{
    public class PATCHMemberShipAddPointsModelValidator : AbstractValidator<PATCHMemberShipAddPointsModel>
    {
        public PATCHMemberShipAddPointsModelValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Points).GreaterThan(0).WithMessage("Points to add must be greater than 0.");
        }
    }
}
