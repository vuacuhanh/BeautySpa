using FluentValidation;
using BeautySpa.ModelViews.ServiceCategoryModelViews;

namespace BeautySpa.Services.Validations.ServiceCategoryValidator
{
    public class POSTServiceCategoryModelViewsValidator : AbstractValidator<POSTServiceCategoryModelViews>
    {
        public POSTServiceCategoryModelViewsValidator()
        {
            RuleFor(x => x.CategoryName).NotEmpty();
        }
    }
}