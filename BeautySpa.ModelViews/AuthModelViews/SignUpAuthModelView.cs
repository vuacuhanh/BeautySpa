using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.ModelViews.AuthModelViews
{
    public class SignUpAuthModelView
    {
        public required string FullName { get; set; }

        public required string Email { get; set; }

        public required string Password { get; set; }

        public Guid RoleId { get; set; }
    }
}
