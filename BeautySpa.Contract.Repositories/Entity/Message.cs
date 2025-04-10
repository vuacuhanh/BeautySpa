using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Message : BaseEntity
    {
        [Required]
        public string SenderId { get; set; }

        [Required]
        [StringLength(10)]
        public string SenderType { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        [Required]
        [StringLength(10)]
        public string ReceiverType { get; set; }

        [Required]
        public string Content { get; set; }

        public bool IsRead { get; set; } = false;
    }
}