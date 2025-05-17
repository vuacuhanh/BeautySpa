using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.ModelViews.LocationModelViews
{
    public class EsgooDistrictResponse
    {
        public int error { get; set; }
        public string error_text { get; set; } = string.Empty;
        public List<DistrictModel> data { get; set; } = new();
    }
}

