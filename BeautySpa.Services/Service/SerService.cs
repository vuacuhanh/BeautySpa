using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ServiceModelViews;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BeautySpa.Services.Service
{
    public class SerService : IServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Guid> CreateAsync(POSTServiceModelViews model)
        {
            if (string.IsNullOrWhiteSpace(model.ServiceName))
            {
                throw new ArgumentException("Service name cannot be empty.");
            }

            var providerExists = await _unitOfWork.GetRepository<ApplicationUsers>().GetByIdAsync(model.ProviderId);
            if (providerExists == null)
            {
                throw new Exception("Provider not found.");
            }

            var categoryExists = await _unitOfWork.GetRepository<ServiceCategory>().GetByIdAsync(model.CategoryId);
            if (categoryExists == null)
            {
                throw new Exception("Service category not found.");
            }

            var service = _mapper.Map<BeautySpa.Contract.Repositories.Entity.Service>(model);
            service.Id = Guid.NewGuid();
            service.CreatedTime = DateTimeOffset.UtcNow;
            service.LastUpdatedTime = service.CreatedTime;

            foreach (var image in model.Images)
            {
                var serviceImage = new ServiceImage
                {
                    Id = Guid.NewGuid(),
                    ImageUrl = image.ImageUrl,
                    IsPrimary = image.IsPrimary,
                    CreatedTime = DateTimeOffset.UtcNow,
                    LastUpdatedTime = DateTimeOffset.UtcNow
                };
                service.ServiceImages.Add(serviceImage);
            }

            await _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>().InsertAsync(service);
            await _unitOfWork.SaveAsync();

            return service.Id;
        }

        public async Task<BasePaginatedList<GETServiceModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                throw new ArgumentException("Page number and page size must be greater than 0.");
            }

            var query = _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>().Entities
                .Where(s => s.DeletedTime == null)
                .Include(s => s.ServiceImages);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedItems = _mapper.Map<IReadOnlyCollection<GETServiceModelViews>>(items);

            return new BasePaginatedList<GETServiceModelViews>(mappedItems, totalCount, pageNumber, pageSize);
        }

        public async Task<GETServiceModelViews> GetByIdAsync(Guid id)
        {
            var service = await _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>().Entities
                .Include(s => s.ServiceImages)
                .FirstOrDefaultAsync(s => s.Id == id && s.DeletedTime == null);
            if (service == null)
            {
                throw new Exception("Service not found.");
            }

            return _mapper.Map<GETServiceModelViews>(service);
        }

        public async Task UpdateAsync(PUTServiceModelViews model)
        {
            var service = await _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>().Entities
                .Include(s => s.ServiceImages)
                .FirstOrDefaultAsync(s => s.Id == model.Id && s.DeletedTime == null);
            if (service == null)
            {
                throw new Exception("Service not found.");
            }

            var providerExists = await _unitOfWork.GetRepository<ApplicationUsers>().GetByIdAsync(model.ProviderId);
            if (providerExists == null)
            {
                throw new Exception("Provider not found.");
            }

            var categoryExists = await _unitOfWork.GetRepository<ServiceCategory>().GetByIdAsync(model.CategoryId);
            if (categoryExists == null)
            {
                throw new Exception("Service category not found.");
            }

            _mapper.Map(model, service);
            service.LastUpdatedTime = DateTimeOffset.UtcNow;

            service.ServiceImages.Clear();
            foreach (var image in model.Images)
            {
                var serviceImage = new ServiceImage
                {
                    Id = Guid.NewGuid(),
                    ImageUrl = image.ImageUrl,
                    IsPrimary = image.IsPrimary,
                    CreatedTime = DateTimeOffset.UtcNow,
                    LastUpdatedTime = DateTimeOffset.UtcNow
                };
                service.ServiceImages.Add(serviceImage);
            }

            await _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>().UpdateAsync(service);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var service = await _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>().GetByIdAsync(id);
            if (service == null || service.DeletedTime != null)
            {
                throw new Exception("Service not found.");
            }

            service.DeletedTime = DateTimeOffset.UtcNow;
            await _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>().UpdateAsync(service);
            await _unitOfWork.SaveAsync();
        }
    }
}