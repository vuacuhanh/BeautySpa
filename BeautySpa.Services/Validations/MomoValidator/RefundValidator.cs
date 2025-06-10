using BeautySpa.ModelViews.MoMoModelViews;
using FluentValidation;
public class RefundRequestValidator : AbstractValidator<RefundRequest>
{
    public RefundRequestValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.TransId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
