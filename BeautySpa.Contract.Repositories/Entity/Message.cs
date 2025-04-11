using BeautySpa.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Message : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public string SenderType { get; set; } = string.Empty;
        public string ReceiverType { get; set; } = string.Empty;

        // Khóa ngoại
        public Guid SenderId { get; set; }
        public virtual ApplicationUsers Sender { get; set; }

        public Guid ReceiverId { get; set; }
        public virtual ApplicationUsers Receiver { get; set; }
    }
}