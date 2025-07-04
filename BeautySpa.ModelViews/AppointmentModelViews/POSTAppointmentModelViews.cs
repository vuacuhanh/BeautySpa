﻿namespace BeautySpa.ModelViews.AppointmentModelViews
{
    public class POSTAppointmentModelView
    {
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public Guid SpaBranchLocationId { get; set; }
        public Guid? StaffId { get; set; } 
        public string? Notes { get; set; }
        public List<AppointmentServiceModel> Services { get; set; } = new();
        public Guid? PromotionId { get; set; }
        public Guid? PromotionAdminId { get; set; }
        public string? PaymentMethod { get; set; } = "momo";

    }
}