using BeautySpa.ModelViews.StaffModelViews;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.Services.Validations.StaffValidator
{
    public class PUTStaffModelViewValidator : AbstractValidator<PUTStaffModelView>
    {
        public PUTStaffModelViewValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.StaffRole).NotEmpty().MaximumLength(50);
        }
    }
}
