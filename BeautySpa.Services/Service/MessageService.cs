using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.MessageModelViews;
using BeautySpa.Services.Validations.MessageValidator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using BeautySpa.Core.SignalR;

namespace BeautySpa.Services.Service
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHubContext<MessageHub> _hubContext;

        public MessageService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor, IHubContext<MessageHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _hubContext = hubContext;
        }

        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public async Task<BaseResponseModel<List<ConversationItem>>> GetConversationsAsync(Guid userId)
        {
            IQueryable<Message> query = _unitOfWork.GetRepository<Message>().Entities
                .Where(m => m.DeletedTime == null && (m.SenderId == userId || m.ReceiverId == userId));

            var groupedQuery = query
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g => new
                {
                    UserId = g.Key,
                    LastMessage = g.OrderByDescending(x => x.CreatedTime).FirstOrDefault(),
                    UnreadCount = g.Count(x => x.ReceiverId == userId && !x.IsRead)
                });

            var grouped = await groupedQuery.ToListAsync();

            var userIds = grouped.Select(x => x.UserId).ToList();

            IQueryable<ApplicationUsers> userQuery = _unitOfWork.GetRepository<ApplicationUsers>().Entities
                .Include(u => u.UserInfor)
                .Where(u => userIds.Contains(u.Id));

            var userInfos = await userQuery.ToListAsync();
            var userNameMap = userInfos.ToDictionary(
                u => u.Id,
                u => u.UserInfor != null ? u.UserInfor.FullName : "Unknown");

            var result = grouped.Select(item => new ConversationItem
            {
                UserId = item.UserId,
                LastMessage = item.LastMessage?.Content ?? string.Empty,
                LastTime = item.LastMessage?.CreatedTime ?? DateTimeOffset.MinValue,
                UnreadCount = item.UnreadCount,
                UserName = userNameMap.TryGetValue(item.UserId, out string? value) ? value : "Unknown"
            }).ToList();

            return BaseResponseModel<List<ConversationItem>>.Success(result);
        }

        public async Task<BaseResponseModel<List<GETMessageModelViews>>> GetThreadAsync(Guid user1Id, Guid user2Id)
        {
            IQueryable<Message> query = _unitOfWork.GetRepository<Message>().Entities
                .Where(m =>
                    m.DeletedTime == null &&
                    ((m.SenderId == user1Id && m.ReceiverId == user2Id) ||
                     (m.SenderId == user2Id && m.ReceiverId == user1Id)))
                .OrderBy(m => m.CreatedTime);

            var list = await query.ToListAsync();
            var result = _mapper.Map<List<GETMessageModelViews>>(list);

            return BaseResponseModel<List<GETMessageModelViews>>.Success(result);
        }
        public async Task<BaseResponseModel<string>> SendMessageAsync(CreateMessageRequest model)
        {
            await new CreateMessageRequestValidator().ValidateAndThrowAsync(model);

            if (_contextAccessor.HttpContext == null)
            {
                throw new ErrorException(StatusCodes.Status401Unauthorized, ErrorCode.UnAuthenticated, "HttpContext is not available.");
            }

            var httpContext = _contextAccessor.HttpContext;

            Guid senderId = Guid.Parse(Authentication.GetUserIdFromHttpContext(httpContext));
            string senderType = Authentication.GetUserRoleFromHttpContext(httpContext);

            var message = new Message
            {
                Id = Guid.NewGuid(),
                Content = model.Content,
                SenderId = senderId,
                ReceiverId = model.ReceiverId,
                SenderType = senderType,
                ReceiverType = model.ReceiverType,
                IsRead = false,
                CreatedTime = CoreHelper.SystemTimeNow,
                CreatedBy = CurrentUserId
            };

            await _unitOfWork.GetRepository<Message>().InsertAsync(message);
            await _unitOfWork.SaveAsync();

            var messageView = _mapper.Map<GETMessageModelViews>(message);
            await _hubContext.Clients.User(model.ReceiverId.ToString()).SendAsync("ReceiveMessage", messageView);

            return BaseResponseModel<string>.Success("Message Sent");
        }

        public async Task<BaseResponseModel<string>> MarkAsReadAsync(Guid messageId)
        {
            IQueryable<Message> query = _unitOfWork.GetRepository<Message>().Entities
                .Where(m => m.Id == messageId && m.DeletedTime == null);

            var message = await query.FirstOrDefaultAsync();

            if (message == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Message not found.");

            message.IsRead = true;
            message.LastUpdatedTime = CoreHelper.SystemTimeNow;
            message.LastUpdatedBy = CurrentUserId;

            await _unitOfWork.GetRepository<Message>().UpdateAsync(message);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Message marked as read");
        }

        public async Task<BaseResponseModel<string>> DeleteMessageAsync(Guid messageId)
        {
            if (messageId == Guid.Empty)
                throw new ErrorException(400, ErrorCode.InvalidInput, "Invalid messageId");

            var repo = _unitOfWork.GetRepository<Message>();
            var message = await repo.Entities.FirstOrDefaultAsync(m => m.Id == messageId && m.DeletedTime == null);

            if (message == null)
                throw new ErrorException(404, ErrorCode.NotFound, "Message not found");

            message.DeletedTime = CoreHelper.SystemTimeNow;
            message.DeletedBy = CurrentUserId;

            await repo.UpdateAsync(message);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Message deleted");
        }
    }
}
