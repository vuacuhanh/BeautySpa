using BeautySpa.ModelViews.LocationModelViews;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.Services.Validations.SpaBranchLocationModelViewValidator
{
    public class POSTSpaBranchLocationModelViewValidator : AbstractValidator<POSTSpaBranchLocationModelView>
    {
        public POSTSpaBranchLocationModelViewValidator()
        {
            RuleFor(x => x.BranchName)
                .NotEmpty().WithMessage("Branch name is required.");

            RuleFor(x => x.ProvinceId)
                .NotEmpty().WithMessage("Province is required.");

            RuleFor(x => x.DistrictId)
                .NotEmpty().WithMessage("District is required.");

            RuleFor(x => x.Street)
                .MaximumLength(200).WithMessage("Street must be at most 200 characters.");
        }
    }
}
