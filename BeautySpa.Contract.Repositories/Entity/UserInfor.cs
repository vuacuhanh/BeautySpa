﻿using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class UserInfor : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime DayofBirth { get; set; }

        public string? ProvinceId { get; set; }
        public string? ProvinceName { get; set; }
        public string? DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public string? AddressDetail { get; set; }

        public string? AvatarUrl { get; set; }
        public string Gender { get; set; } = string.Empty;
        public decimal? Salary { get; set; }
        //public string? BankAccount { get; set; }
        //public string? BankAccountName { get; set; }
        //public string? Bank { get; set; }

        // Khóa ngoại
        public Guid UserId { get; set; }
        public virtual ApplicationUsers? User { get; set; }
    }
}
