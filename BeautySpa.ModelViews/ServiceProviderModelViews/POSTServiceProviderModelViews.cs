﻿namespace BeautySpa.ModelViews.ServiceProviderModelViews
{
    public class POSTServiceProviderModelViews
    {
        public string BusinessName { get; set; } = string.Empty;
        public string BusinessAddress { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ContactFullName { get; set; } = string.Empty;
        public string ContactPosition { get; set; } = string.Empty;
        public int? YearsOfExperience { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public int MaxAppointmentsPerSlot { get; set; } = 5;
        public string? Email { get; set; }
        public Guid UserId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

    }
}