using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ServiceImageModelViews;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.Services.Validations.ServiceImageValidator;

namespace BeautySpa.Services.Service
{
    public class ServiceImageSer : IServiceImages
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public ServiceImageSer(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<BaseResponseModel<string>> CreateMultipleAsync(POSTServiceImageModelViews model)
        {
            var validator = new POSTServiceImageModelViewsValidator();
            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

            if (model.ImageUrls == null || !model.ImageUrls.Any())
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "No image URLs provided.");

            var provider = await _unitOfWork.GetRepository<ServiceProvider>().GetByIdAsync(model.ServiceProviderId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Provider not found");

            if (provider.CreatedBy != CurrentUserId)
                throw new ErrorException(StatusCodes.Status403Forbidden, ErrorCode.UnAuthorized, "You can't modify this provider's images.");
            foreach (var imageUrl in model.ImageUrls)
            {
                var entity = new ServiceImage
                {
                    Id = Guid.NewGuid(),
                    ServiceProviderId = model.ServiceProviderId,
                    ImageUrl = imageUrl,
                    CreatedTime = CoreHelper.SystemTimeNow,
                    CreatedBy = CurrentUserId,
                    LastUpdatedTime = CoreHelper.SystemTimeNow,
                    LastUpdatedBy = CurrentUserId
                };
                await _unitOfWork.GetRepository<ServiceImage>().InsertAsync(entity);
            }

            await _unitOfWork.SaveAsync();
            return BaseResponseModel<string>.Success("Images added successfully.");
        }

        public async Task<BaseResponseModel<BasePaginatedList<GETServiceImageModelViews>>> GetPagedByProviderIdAsync(Guid providerId, int page, int size)
        {
            if (page <= 0 || size <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page and size must be greater than 0.");

            var query = _unitOfWork.GetRepository<ServiceImage>().Entities
                .AsNoTracking()
                .Where(x => x.ServiceProviderId == providerId && x.DeletedTime == null)
                .OrderByDescending(x => x.CreatedTime);

            var totalCount = await query.CountAsync();

            var pagedList = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            var result = _mapper.Map<List<GETServiceImageModelViews>>(pagedList);

            return BaseResponseModel<BasePaginatedList<GETServiceImageModelViews>>.Success(
                new BasePaginatedList<GETServiceImageModelViews>(result, totalCount, page, size));
        }

        public async Task<BaseResponseModel<List<GETServiceImageModelViews>>> GetByProviderIdAsync(Guid providerId)
        {
            var query = _unitOfWork.GetRepository<ServiceImage>().Entities
                .AsNoTracking()
                .Where(x => x.ServiceProviderId == providerId && x.DeletedTime == null)
                .OrderByDescending(x => x.CreatedTime);

            var list = await query.ToListAsync();
            var result = _mapper.Map<List<GETServiceImageModelViews>>(list);

            return BaseResponseModel<List<GETServiceImageModelViews>>.Success(result);
        }

        public async Task<BaseResponseModel<string>> DeleteAsync(Guid imageId)
        {
            var repo = _unitOfWork.GetRepository<ServiceImage>();
            var entity = await repo.Entities.FirstOrDefaultAsync(x => x.Id == imageId && x.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Image not found");
            var provider = await _unitOfWork.GetRepository<ServiceProvider>()

            .GetByIdAsync(entity.ServiceProviderId)
            ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Provider not found");

            if (provider.CreatedBy != CurrentUserId)
                throw new ErrorException(StatusCodes.Status403Forbidden, ErrorCode.UnAuthorized, "You can't delete this provider's images.");

            entity.DeletedTime = CoreHelper.SystemTimeNow;
            entity.DeletedBy = CurrentUserId;

            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Image deleted successfully.");
        }
    }
}