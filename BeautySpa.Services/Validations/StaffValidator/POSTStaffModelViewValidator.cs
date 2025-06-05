using BeautySpa.ModelViews.StaffModelViews;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.Services.Validations.StaffValidator
{
    public class POSTStaffModelViewValidator : AbstractValidator<POSTStaffModelView>
    {
        public POSTStaffModelViewValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^[0-9]{10,15}$").WithMessage("Invalid phone number format");

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("Invalid email format");

            RuleFor(x => x.ServiceCategoryIds)
                .NotEmpty().WithMessage("At least one service category is required");
        }
    }
}
