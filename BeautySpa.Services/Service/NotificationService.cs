using AutoMapper;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.NotificationModelViews;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Repositories.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using BeautySpa.Core.Utils;
using BeautySpa.Core.Infrastructure;

namespace BeautySpa.Services.Service
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        private string currentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<BasePaginatedList<GETNotificationModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");
            }

            IQueryable<Notification> notifications = _unitOfWork.GetRepository<Notification>()
                .Entities.Where(n => !n.DeletedTime.HasValue)
                .OrderByDescending(n => n.CreatedTime)
                .AsQueryable();

            var paginatedNotifications = await notifications
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new BasePaginatedList<GETNotificationModelViews>(
                _mapper.Map<List<GETNotificationModelViews>>(paginatedNotifications),
                await notifications.CountAsync(),
                pageNumber,
                pageSize
            );
        }

        public async Task<GETNotificationModelViews> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid notification ID.");
            }

            var notification = await _unitOfWork.GetRepository<Notification>()
                .Entities.FirstOrDefaultAsync(n => n.Id == id && !n.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Notification not found.");

            return _mapper.Map<GETNotificationModelViews>(notification);
        }

        public async Task<Guid> CreateAsync(POSTNotificationModelViews model)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Notification title cannot be empty.");
            }

            var notification = _mapper.Map<Notification>(model);
            notification.Id = Guid.NewGuid();
            notification.CreatedBy = currentUserId;
            notification.CreatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Notification>().InsertAsync(notification);
            await _unitOfWork.SaveAsync();

            return notification.Id;
        }

        public async Task UpdateAsync(PUTNotificationModelViews model)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Notification title cannot be empty.");
            }

            var genericRepository = _unitOfWork.GetRepository<Notification>();

            var notification = await genericRepository.Entities
                .FirstOrDefaultAsync(n => n.Id == model.Id && !n.DeletedTime.HasValue);

            if (notification == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, $"Not found Notification with id = {model.Id}");
            }

            _mapper.Map(model, notification);
            notification.LastUpdatedBy = currentUserId;
            notification.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await genericRepository.UpdateAsync(notification);
            await genericRepository.SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid notification ID.");
            }

            var genericRepository = _unitOfWork.GetRepository<Notification>();

            var notification = await genericRepository.Entities
                .FirstOrDefaultAsync(n => n.Id == id && !n.DeletedTime.HasValue);

            if (notification == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, $"Not found Notification with id = {id}");
            }

            notification.DeletedTime = CoreHelper.SystemTimeNow;
            notification.DeletedBy = currentUserId;

            await genericRepository.UpdateAsync(notification);
            await genericRepository.SaveAsync();
        }
    }
}
