﻿using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Notification : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string NotificationType { get; set; } = string.Empty; // System, Booking, Chat, Reward, Alert
        public bool IsRead { get; set; } = false;
        // Khóa ngoại
        public Guid UserId { get; set; }
        public virtual ApplicationUsers? User { get; set; }
    }
}