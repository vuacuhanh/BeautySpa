using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.ModelViews.AuthModelViews
{
    public class ChangePasswordAuthModelView
    {
        public required string CurrentPassword { get; set; }

        public required string NewPassword { get; set; }

        public required string ConfirmPassword { get; set; }
    }
}
