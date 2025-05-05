using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.RequestBecomeProviderModelView;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Services.Service
{
    public class RequestBecomeProviderService : IRequestBecomeProvider
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public RequestBecomeProviderService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<BaseResponseModel<Guid>> CreateRequestAsync(POSTRequestBecomeProviderModelView model)
        {
            var userId = Guid.Parse(CurrentUserId);
            var user = await _unitOfWork.GetRepository<ApplicationUsers>().GetByIdAsync(userId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User not found.");

            foreach (var categoryId in model.ServiceCategoryIds)
            {
                _ = await _unitOfWork.GetRepository<ServiceCategory>().GetByIdAsync(categoryId)
                    ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, $"Service category {categoryId} not found.");
            }

            var existing = await _unitOfWork.GetRepository<RequestBecomeProvider>()
                .Entities.FirstOrDefaultAsync(x => x.UserId == userId && x.RequestStatus == "pending");

            if (existing != null)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Already have pending request.");

            var request = _mapper.Map<RequestBecomeProvider>(model);
            request.Id = Guid.NewGuid();
            request.UserId = userId;
            request.RequestStatus = "pending";
            request.CreatedBy = CurrentUserId;
            request.CreatedTime = CoreHelper.SystemTimeNow;
            request.LastUpdatedBy = CurrentUserId;
            request.LastUpdatedTime = request.CreatedTime;

            await _unitOfWork.GetRepository<RequestBecomeProvider>().InsertAsync(request);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<Guid>.Success(request.Id);
        }

        public async Task<BaseResponseModel<BasePaginatedList<GETRequestBecomeProviderModelView>>> GetAllAsync(string? requestStatus, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid pagination.");

            var query = _unitOfWork.GetRepository<RequestBecomeProvider>()
                .Entities
                .Where(r => r.DeletedTime == null);

            if (!string.IsNullOrEmpty(requestStatus))
            {
                requestStatus = requestStatus.ToLower();
                query = query.Where(r => r.RequestStatus.ToLower() == requestStatus);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(r => r.CreatedTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mapped = _mapper.Map<List<GETRequestBecomeProviderModelView>>(items);
            return BaseResponseModel<BasePaginatedList<GETRequestBecomeProviderModelView>>.Success(
                new BasePaginatedList<GETRequestBecomeProviderModelView>(mapped, total, pageNumber, pageSize));
        }

        public async Task<BaseResponseModel<string>> ApproveRequestAsync(Guid requestId)
        {
            var requestRepo = _unitOfWork.GetRepository<RequestBecomeProvider>();
            var userRepo = _unitOfWork.GetRepository<ApplicationUsers>();
            var providerRepo = _unitOfWork.GetRepository<ServiceProvider>();
            var providerCategoryRepo = _unitOfWork.GetRepository<ServiceProviderCategory>();
            var workingHourRepo = _unitOfWork.GetRepository<WorkingHour>();
            var imageRepo = _unitOfWork.GetRepository<ServiceImage>();

            var request = await requestRepo.Entities
                .FirstOrDefaultAsync(r => r.Id == requestId && r.RequestStatus == "pending" && r.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Yêu cầu không tồn tại.");

            var user = await userRepo.GetByIdAsync(request.UserId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Người dùng không tồn tại.");

            if (user.ServiceProvider != null)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Người dùng đã là nhà cung cấp.");

            var provider = new ServiceProvider
            {
                Id = Guid.NewGuid(),
                BusinessName = request.BusinessName,
                PhoneNumber = request.PhoneNumber,
                WebsiteOrSocialLink = request.WebsiteOrSocialLink ?? "",
                Description = request.Description,
                ImageUrl = request.ImageUrl ?? "",
                ProviderId = user.Id,
                ContactFullName = user.UserName ?? "",
                ContactPosition = "Chủ cơ sở",
                Status = "approved",
                IsApproved = true,
                AverageRating = 0,
                TotalReviews = 0,
            };
            await providerRepo.InsertAsync(provider);

            var categoryIds = request.ServiceCategoryIds?.Split('|', StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList() ?? new();
            foreach (var catId in categoryIds)
            {
                await providerCategoryRepo.InsertAsync(new ServiceProviderCategory
                {
                    Id = Guid.NewGuid(),
                    ServiceProviderId = provider.Id,
                    ServiceCategoryId = catId
                });
            }

            if (request.OpenTime.HasValue && request.CloseTime.HasValue)
            {
                await workingHourRepo.InsertAsync(new WorkingHour
                {
                    Id = Guid.NewGuid(),
                    ServiceProviderId = provider.Id,
                    DayOfWeek = 1,
                    OpeningTime = request.OpenTime.Value,
                    ClosingTime = request.CloseTime.Value,
                    IsWorking = true
                });
            }

            var descImages = request.DescriptionImages?.Split('|', StringSplitOptions.RemoveEmptyEntries);
            if (descImages != null)
            {
                foreach (var img in descImages)
                {
                    await imageRepo.InsertAsync(new ServiceImage
                    {
                        Id = Guid.NewGuid(),
                        ServiceProviderId = provider.Id,
                        ImageUrl = img
                    });
                }
            }

            request.RequestStatus = "approved";
            request.LastUpdatedBy = CurrentUserId;
            request.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await requestRepo.UpdateAsync(request);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Đã duyệt yêu cầu thành công.");
        }

        public async Task<BaseResponseModel<string>> RejectRequestAsync(Guid requestId, string reason)
        {
            var request = await _unitOfWork.GetRepository<RequestBecomeProvider>()
                .Entities
                .FirstOrDefaultAsync(r => r.Id == requestId && r.RequestStatus == "pending" && r.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Request not found.");

            request.RequestStatus = "rejected";
            request.RejectedReason = reason;
            request.LastUpdatedBy = CurrentUserId;
            request.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<RequestBecomeProvider>().UpdateAsync(request);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Request rejected with reason.");
        }
    }
}