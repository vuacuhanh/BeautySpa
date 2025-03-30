﻿using BeautySpa.Core.Utils;
using Microsoft.AspNetCore.Identity;


namespace BeautySpa.Contract.Repositories.Entity
{
    public class ApplicationUsers : IdentityUser<Guid>
    {
        public string Password { get; set; } = string.Empty;
        public virtual UserInfor? UserInfor { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }


        public ApplicationUsers()
        {
            CreatedTime = CoreHelper.SystemTimeNow;
            LastUpdatedTime = CreatedTime;
        }
    }
}
