using BeautySpa.ModelViews.ServiceCategoryModelViews;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.Services.Validations.ServiceCategoryValidator
{
    public class PUTServiceCategoryModelViewsValidator : AbstractValidator<PUTServiceCategoryModelViews>
    {
        public PUTServiceCategoryModelViewsValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.CategoryName).NotEmpty();
        }
    }
}
