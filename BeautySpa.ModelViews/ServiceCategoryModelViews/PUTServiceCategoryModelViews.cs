using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.ModelViews.ServiceCategoryModelViews
{
    public class PUTServiceCategoryModelViews
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
