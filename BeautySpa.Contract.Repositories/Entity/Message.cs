using BeautySpa.Core.Base;

namespace BeautySpa.Contract.Repositories.Entity
{
    public class Message : BaseEntity
    {
        public required string Content { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public required string SenderType { get; set; } = string.Empty;
        public required string ReceiverType { get; set; } = string.Empty;

        // Khóa ngoại
        public Guid SenderId { get; set; }
        public virtual ApplicationUsers? Sender { get; set; }

        public Guid ReceiverId { get; set; }
        public virtual ApplicationUsers? Receiver { get; set; }
    }
}