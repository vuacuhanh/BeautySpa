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

namespace BeautySpa.Services.Service
{
    public class ServiceImageSer : IServiceImages
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private string currentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public ServiceImageSer(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<Guid> CreateAsync(POSTServiceImageModelViews model)
        {
            // Kiểm tra ImageUrl có hợp lệ không
            if (string.IsNullOrWhiteSpace(model.ImageUrl))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Image URL cannot be empty.");
            }

            // Kiểm tra ServiceId có tồn tại không
            var serviceExists = await _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>().Entities
                .FirstOrDefaultAsync(s => s.Id == model.ServiceProviderId && !s.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service not found.");

            // Map từ model sang entity
            var serviceImage = _mapper.Map<ServiceImage>(model);
            serviceImage.Id = Guid.NewGuid();
            serviceImage.CreatedBy = currentUserId;
            serviceImage.CreatedTime = CoreHelper.SystemTimeNow;
            serviceImage.LastUpdatedBy = currentUserId;
            serviceImage.LastUpdatedTime = CoreHelper.SystemTimeNow;

            // Thêm vào DB
            await _unitOfWork.GetRepository<ServiceImage>().InsertAsync(serviceImage);
            await _unitOfWork.SaveAsync();

            return serviceImage.Id;
        }

        public async Task<BasePaginatedList<GETServiceImageModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");
            }

            IQueryable<ServiceImage> serviceImages = _unitOfWork.GetRepository<ServiceImage>()
                .Entities.Where(img => !img.DeletedTime.HasValue)
                .OrderByDescending(img => img.CreatedTime).AsQueryable();

            var paginatedServiceImages = await serviceImages
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new BasePaginatedList<GETServiceImageModelViews>(_mapper.Map<List<GETServiceImageModelViews>>(paginatedServiceImages),
                await serviceImages.CountAsync(), pageNumber, pageSize);
        }

        public async Task<GETServiceImageModelViews> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid service image ID.");
            }

            var serviceImage = await _unitOfWork.GetRepository<ServiceImage>().Entities
                .FirstOrDefaultAsync(img => img.Id == id && !img.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service image not found.");

            return _mapper.Map<GETServiceImageModelViews>(serviceImage);
        }

        public async Task UpdateAsync(PUTServiceImageModelViews model)
        {
            if (model.Id == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid service image ID.");
            }

            if (string.IsNullOrWhiteSpace(model.ImageUrl))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Image URL cannot be empty.");
            }

            // Kiểm tra ServiceId có tồn tại không
            var serviceExists = await _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>().Entities
                .FirstOrDefaultAsync(s => s.Id == model.ServiceId && !s.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service not found.");

            var genericRepository = _unitOfWork.GetRepository<ServiceImage>();

            var serviceImage = await genericRepository.Entities
                .FirstOrDefaultAsync(img => img.Id == model.Id && !img.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, $"Service image with id = {model.Id} not found.");

            // Map dữ liệu từ model sang entity
            _mapper.Map(model, serviceImage);
            serviceImage.LastUpdatedBy = currentUserId;
            serviceImage.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await genericRepository.UpdateAsync(serviceImage);
            await genericRepository.SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid service image ID.");
            }

            var genericRepository = _unitOfWork.GetRepository<ServiceImage>();

            var serviceImage = await genericRepository.Entities
                .FirstOrDefaultAsync(img => img.Id == id && !img.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, $"Service image with id = {id} not found.");

            serviceImage.DeletedTime = CoreHelper.SystemTimeNow;
            serviceImage.DeletedBy = currentUserId;

            await genericRepository.UpdateAsync(serviceImage);
            await genericRepository.SaveAsync();
        }
    }
}