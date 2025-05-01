using BeautySpa.Core.Base;
using BeautySpa.ModelViews.MessageModelViews;

namespace BeautySpa.Contract.Services.Interface
{
    public interface IMessageService
    {
        Task<BaseResponseModel<List<ConversationItem>>> GetConversationsAsync(Guid userId);
        Task<BaseResponseModel<List<GETMessageModelViews>>> GetThreadAsync(Guid user1Id, Guid user2Id);
        Task<BaseResponseModel<string>> SendMessageAsync(CreateMessageRequest model);
        Task<BaseResponseModel<string>> MarkAsReadAsync(Guid messageId);
        Task<BaseResponseModel<string>> DeleteMessageAsync(Guid messageId);
    }
}
