using BeautySpa.ModelViews.MessageModelViews;
using Microsoft.AspNetCore.SignalR;

namespace BeautySpa.Repositories.SignaIR
{
    public class MessageHub : Hub
    {
        // Gửi tin nhắn tới người nhận dựa trên ReceiverId
        public async Task SendMessageToUser(GETMessageModelViews message)
        {
            await Clients.User(message.ReceiverId.ToString()).SendAsync("ReceiveMessage", message);
            await Clients.User(message.SenderId.ToString()).SendAsync("ReceiveMessage", message);
        }

        // Gửi sự kiện cập nhật hội thoại tới người nhận
        public async Task UpdateConversation(string email, ConversationItem conversation)
        {
            await Clients.User(email).SendAsync("UpdateConversation", conversation);
        }
    }
}
