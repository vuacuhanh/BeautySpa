﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.ModelViews.UserModelViews
{
    public class PUTUserModelViews
    {
        public Guid Id { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? AddressDetail { get; set; }
        public string? ProvinceId { get; set; }
        public string? DistrictId { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
    }
}