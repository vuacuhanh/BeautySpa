using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.ModelViews.UserModelViews
{
    public class PUTuserforcustomer
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public GETUserModelViews UserInfor { get; set; } = new GETUserModelViews();
    }
}
