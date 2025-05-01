using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.SignalR;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.NotificationModelViews;
using BeautySpa.Services.Validations.NotificationValidator;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Services.Service
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHubContext<MessageHub> _hubContext;

        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor, IHubContext<MessageHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _hubContext = hubContext;
        }

        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public async Task<BaseResponseModel<List<GETNotificationModelView>>> GetAllByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ErrorException(400, ErrorCode.InvalidInput, "Invalid userId");

            IQueryable<Notification> query = _unitOfWork.GetRepository<Notification>().Entities
                .Where(n => n.UserId == userId && n.DeletedTime == null)
                .OrderByDescending(n => n.CreatedTime);

            List<Notification> data = await query.ToListAsync();
            var result = _mapper.Map<List<GETNotificationModelView>>(data);

            return BaseResponseModel<List<GETNotificationModelView>>.Success(result);
        }

        public async Task<BaseResponseModel<int>> GetUnreadCountAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ErrorException(400, ErrorCode.InvalidInput, "Invalid userId");

            IQueryable<Notification> query = _unitOfWork.GetRepository<Notification>().Entities
                .Where(n => n.UserId == userId && !n.IsRead && n.DeletedTime == null);

            int count = await query.CountAsync();
            return BaseResponseModel<int>.Success(count);
        }

        public async Task<BaseResponseModel<string>> CreateAsync(POSTNotificationModelView model)
        {
            await new POSTNotificationValidator().ValidateAndThrowAsync(model);

            var notification = _mapper.Map<Notification>(model);
            notification.Id = Guid.NewGuid();
            notification.IsRead = false;
            notification.CreatedTime = CoreHelper.SystemTimeNow;
            notification.CreatedBy = CurrentUserId;

            await _unitOfWork.GetRepository<Notification>().InsertAsync(notification);
            await _unitOfWork.SaveAsync();

            var result = _mapper.Map<GETNotificationModelView>(notification);
            await _hubContext.Clients.User(model.UserId.ToString()).SendAsync("ReceiveNotification", result);

            return BaseResponseModel<string>.Success("Notification sent");
        }

        public async Task<BaseResponseModel<string>> MarkAsReadAsync(Guid notificationId)
        {
            if (notificationId == Guid.Empty)
                throw new ErrorException(400, ErrorCode.InvalidInput, "Invalid notificationId");

            var repo = _unitOfWork.GetRepository<Notification>();
            IQueryable<Notification> query = repo.Entities
                .Where(n => n.Id == notificationId && n.DeletedTime == null);

            Notification? noti = await query.FirstOrDefaultAsync();
            if (noti == null)
                throw new ErrorException(404, ErrorCode.NotFound, "Notification not found.");

            noti.IsRead = true;
            noti.LastUpdatedBy = CurrentUserId;
            noti.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await repo.UpdateAsync(noti);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Notification marked as read");
        }

        public async Task<BaseResponseModel<string>> DeleteAsync(Guid notificationId)
        {
            if (notificationId == Guid.Empty)
                throw new ErrorException(400, ErrorCode.InvalidInput, "Invalid notificationId");

            var repo = _unitOfWork.GetRepository<Notification>();
            var notification = await repo.Entities.FirstOrDefaultAsync(n => n.Id == notificationId && n.DeletedTime == null);

            if (notification == null)
                throw new ErrorException(404, ErrorCode.NotFound, "Notification not found");

            notification.DeletedTime = CoreHelper.SystemTimeNow;
            notification.DeletedBy = CurrentUserId;

            await repo.UpdateAsync(notification);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Notification deleted");
        }

    }
}
