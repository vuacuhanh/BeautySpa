using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.ModelViews.StatisticModelViews
{
    public class StatisticResultModelView
    {
        public int TotalAppointments { get; set; }
        public decimal TotalRevenue { get; set; }
        public int CompletedCount { get; set; }
        public int CancelledCount { get; set; }
        public int NoShowCount { get; set; }
        public List<TopServiceModel> TopServices { get; set; }
    }

    public class TopServiceModel
    {
        public string ServiceName { get; set; }
        public int TotalBooked { get; set; }
    }
}
