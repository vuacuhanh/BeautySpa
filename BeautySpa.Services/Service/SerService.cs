using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ServiceModelViews;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;

namespace BeautySpa.Services.Service
{
    public class SerService : IServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private string currentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public SerService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<Guid> CreateAsync(POSTServiceModelViews model)
        {
            if (string.IsNullOrWhiteSpace(model.ServiceName))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Service name cannot be empty.");
            }

            var providerExists = await _unitOfWork.GetRepository<ApplicationUsers>().Entities
                .FirstOrDefaultAsync(p => p.Id == model.ProviderId && !p.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Provider not found.");

            var categoryExists = await _unitOfWork.GetRepository<ServiceCategory>().Entities
                .FirstOrDefaultAsync(c => c.Id == model.CategoryId && !c.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service category not found.");

            var serviceExists = await _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>()
                .Entities.FirstOrDefaultAsync(s => s.ServiceName.ToLower() == model.ServiceName.ToLower() && !s.DeletedTime.HasValue);

            if (serviceExists != null)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Service name already exists.");
            }

            var servicePromotion = _mapper.Map<BeautySpa.Contract.Repositories.Entity.Service>(model);
            servicePromotion.Id = Guid.NewGuid();
            servicePromotion.CreatedBy = currentUserId;
            servicePromotion.CreatedTime = CoreHelper.SystemTimeNow;
            servicePromotion.LastUpdatedBy = currentUserId;
            servicePromotion.LastUpdatedTime = CoreHelper.SystemTimeNow;

            // Xử lý ServiceImages
            servicePromotion.ServiceImages = new List<ServiceImage>();
            foreach (var image in model.Images)
            {
                var serviceImage = new ServiceImage
                {
                    Id = Guid.NewGuid(),
                    ImageUrl = image.ImageUrl,
                    IsPrimary = image.IsPrimary,

                    //ServiceProviderId = servicePromotion.ProviderId, 
                    CreatedBy = currentUserId,
                    CreatedTime = CoreHelper.SystemTimeNow,
                    LastUpdatedBy = currentUserId,
                    LastUpdatedTime = CoreHelper.SystemTimeNow
                };
                servicePromotion.ServiceImages.Add(serviceImage);
            }

            await _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>().InsertAsync(servicePromotion);
            await _unitOfWork.SaveAsync();
            return servicePromotion.Id;
        }

        public async Task<BasePaginatedList<GETServiceModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");
            }

            IQueryable<BeautySpa.Contract.Repositories.Entity.Service> services = _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>()
                .Entities.Where(s => !s.DeletedTime.HasValue)
                .Include(s => s.ServiceImages)
                .OrderByDescending(s => s.CreatedTime).AsQueryable();

            var paginatedServices = await services
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new BasePaginatedList<GETServiceModelViews>(_mapper.Map<List<GETServiceModelViews>>(paginatedServices),
                await services.CountAsync(), pageNumber, pageSize);
        }

        public async Task<GETServiceModelViews> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid service ID.");
            }

            var service = await _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>().Entities
                .Include(s => s.ServiceImages)
                .FirstOrDefaultAsync(s => s.Id == id && !s.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service not found.");

            return _mapper.Map<GETServiceModelViews>(service);
        }

        public async Task UpdateAsync(PUTServiceModelViews model)
        {
            if (model.Id == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid service ID.");
            }

            if (string.IsNullOrWhiteSpace(model.ServiceName))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Service name cannot be empty.");
            }

            var providerExists = await _unitOfWork.GetRepository<ApplicationUsers>().Entities
                .FirstOrDefaultAsync(p => p.Id == model.ProviderId && !p.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Provider not found.");

            var categoryExists = await _unitOfWork.GetRepository<ServiceCategory>()
                .Entities.FirstOrDefaultAsync(c => c.Id == model.CategoryId && !c.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service category not found.");

            var genericRepository = _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>();
            var servicePromotion = await genericRepository.Entities
                .Include(s => s.ServiceImages)
                .FirstOrDefaultAsync(s => s.Id == model.Id && !s.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, $"Service with id = {model.Id} not found.");

            var serviceNameExists = await genericRepository.Entities
                .FirstOrDefaultAsync(s => s.ServiceName.ToLower() == model.ServiceName.ToLower() && s.Id != model.Id && !s.DeletedTime.HasValue);

            if (serviceNameExists != null)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Service name already exists.");
            }

            _mapper.Map(model, servicePromotion);
            servicePromotion.LastUpdatedBy = currentUserId;
            servicePromotion.LastUpdatedTime = CoreHelper.SystemTimeNow;

            // Xử lý ServiceImages
            servicePromotion.ServiceImages.Clear();
            foreach (var image in model.Images)
            {
                var serviceImage = new ServiceImage
                {
                    Id = Guid.NewGuid(),
                    ImageUrl = image.ImageUrl,
                    IsPrimary = image.IsPrimary,
                    //ServiceProviderId = servicePromotion.ProviderId,
                    CreatedBy = currentUserId,
                    CreatedTime = CoreHelper.SystemTimeNow,
                    LastUpdatedBy = currentUserId,
                    LastUpdatedTime = CoreHelper.SystemTimeNow
                };
                servicePromotion.ServiceImages.Add(serviceImage);
            }

            await genericRepository.UpdateAsync(servicePromotion);
            await genericRepository.SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid service ID.");
            }

            var genericRepository = _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>();

            var service = await genericRepository.Entities
                .FirstOrDefaultAsync(s => s.Id == id && !s.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, $"Service with id = {id} not found.");

            service.DeletedTime = CoreHelper.SystemTimeNow;
            service.DeletedBy = currentUserId;

            await genericRepository.UpdateAsync(service);
            await genericRepository.SaveAsync();
        }
    }
}
