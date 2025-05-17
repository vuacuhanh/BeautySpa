using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.Core.Settings
{
    public class GoogleGeocodeResponse
    {
        public List<Result> Results { get; set; } = new();
        public string Status { get; set; } = string.Empty;
    }

    public class Result
    {
        public Geometry? Geometry { get; set; }
    }

    public class Geometry
    {
        public Location? Location { get; set; }
    }

    public class Location
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
