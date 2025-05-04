using BeautySpa.ModelViews.ServiceImageModelViews;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.Services.Validations.ServiceImageValidator
{
    public class POSTServiceImageModelViewsValidator : AbstractValidator<POSTServiceImageModelViews>
    {
        public POSTServiceImageModelViewsValidator()
        {
            RuleFor(x => x.ServiceProviderId)
                .NotEmpty().WithMessage("ServiceProviderId is required.");

            RuleFor(x => x.ImageUrls)
                .NotEmpty().WithMessage("At least one image URL must be provided.");

            RuleForEach(x => x.ImageUrls)
                .NotEmpty().WithMessage("Image URL cannot be empty.")
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("Invalid image URL format.");
        }
    }
}
