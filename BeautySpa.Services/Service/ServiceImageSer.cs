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

        public async Task<BaseResponseModel<Guid>> CreateAsync(POSTServiceImageModelViews model)
        {
            var validator = new POSTServiceImageModelViewsValidator();
            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

            var provider = await _unitOfWork.GetRepository<ServiceProvider>().GetByIdAsync(model.ServiceProviderId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service provider not found.");

            var entity = _mapper.Map<ServiceImage>(model);
            entity.Id = Guid.NewGuid();
            entity.CreatedBy = CurrentUserId;
            entity.CreatedTime = CoreHelper.SystemTimeNow;
            entity.LastUpdatedBy = CurrentUserId;
            entity.LastUpdatedTime = entity.CreatedTime;

            await _unitOfWork.GetRepository<ServiceImage>().InsertAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<Guid>.Success(entity.Id);
        }

        public async Task<BaseResponseModel<BasePaginatedList<GETServiceImageModelViews>>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");

            IQueryable<ServiceImage> query = _unitOfWork.GetRepository<ServiceImage>()
                .Entities.Where(img => img.DeletedTime == null)
                .OrderByDescending(img => img.CreatedTime);

            var pagedImages = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var result = new BasePaginatedList<GETServiceImageModelViews>(
                _mapper.Map<List<GETServiceImageModelViews>>(pagedImages),
                await query.CountAsync(),
                pageNumber,
                pageSize
            );

            return BaseResponseModel<BasePaginatedList<GETServiceImageModelViews>>.Success(result);
        }

        public async Task<BaseResponseModel<GETServiceImageModelViews>> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid service image ID.");

            var entity = await _unitOfWork.GetRepository<ServiceImage>().Entities
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service image not found.");

            return BaseResponseModel<GETServiceImageModelViews>.Success(_mapper.Map<GETServiceImageModelViews>(entity));
        }

        public async Task<BaseResponseModel<string>> UpdateAsync(PUTServiceImageModelViews model)
        {
            var validator = new PUTServiceImageModelViewsValidator();
            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

            var entity = await _unitOfWork.GetRepository<ServiceImage>().Entities
                .FirstOrDefaultAsync(x => x.Id == model.Id && x.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service image not found.");

            _mapper.Map(model, entity);
            entity.LastUpdatedBy = CurrentUserId;
            entity.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<ServiceImage>().UpdateAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Service image updated successfully.");
        }

        public async Task<BaseResponseModel<string>> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid service image ID.");

            var entity = await _unitOfWork.GetRepository<ServiceImage>().Entities
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service image not found.");

            entity.DeletedTime = CoreHelper.SystemTimeNow;
            entity.DeletedBy = CurrentUserId;

            await _unitOfWork.GetRepository<ServiceImage>().UpdateAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Service image deleted successfully.");
        }

        public async Task<BaseResponseModel<string>> SetPrimaryImageAsync(Guid imageId)
        {
            throw new NotImplementedException("Primary image is now handled by ImageUrl in ServiceProvider.");
        }
    }
}
