using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ServiceImageModelViews;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.Services.Service
{
    public class ServiceImageSer : IServiceImages
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ServiceImageSer(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Guid> CreateAsync(POSTServiceImageModelViews model)
        {
            if (string.IsNullOrWhiteSpace(model.ImageUrl))
            {
                throw new ArgumentException("Image URL cannot be empty.");
            }

            var serviceExists = await _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>().GetByIdAsync(model.ServiceId);
            if (serviceExists == null)
            {
                throw new Exception("Service not found.");
            }

            var serviceImage = _mapper.Map<ServiceImage>(model);
            serviceImage.Id = Guid.NewGuid();
            serviceImage.CreatedTime = DateTimeOffset.UtcNow;
            serviceImage.LastUpdatedTime = serviceImage.CreatedTime;

            await _unitOfWork.GetRepository<ServiceImage>().InsertAsync(serviceImage);
            await _unitOfWork.SaveAsync();

            return serviceImage.Id;
        }

        public async Task<BasePaginatedList<GETServiceImageModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                throw new ArgumentException("Page number and page size must be greater than 0.");
            }

            var query = _unitOfWork.GetRepository<ServiceImage>().Entities
                .Where(img => img.DeletedTime == null);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedItems = _mapper.Map<IReadOnlyCollection<GETServiceImageModelViews>>(items);

            return new BasePaginatedList<GETServiceImageModelViews>(mappedItems, totalCount, pageNumber, pageSize);
        }

        public async Task<GETServiceImageModelViews> GetByIdAsync(Guid id)
        {
            var serviceImage = await _unitOfWork.GetRepository<ServiceImage>().Entities
                .FirstOrDefaultAsync(img => img.Id == id && img.DeletedTime == null);
            if (serviceImage == null)
            {
                throw new Exception("Service image not found.");
            }

            return _mapper.Map<GETServiceImageModelViews>(serviceImage);
        }

        public async Task UpdateAsync(PUTServiceImageModelViews model)
        {
            var serviceImage = await _unitOfWork.GetRepository<ServiceImage>().Entities
                .FirstOrDefaultAsync(img => img.Id == model.Id && img.DeletedTime == null);
            if (serviceImage == null)
            {
                throw new Exception("Service image not found.");
            }

            if (string.IsNullOrWhiteSpace(model.ImageUrl))
            {
                throw new ArgumentException("Image URL cannot be empty.");
            }

            var serviceExists = await _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>().GetByIdAsync(model.ServiceId);
            if (serviceExists == null)
            {
                throw new Exception("Service not found.");
            }

            _mapper.Map(model, serviceImage);
            serviceImage.LastUpdatedTime = DateTimeOffset.UtcNow;

            await _unitOfWork.GetRepository<ServiceImage>().UpdateAsync(serviceImage);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var serviceImage = await _unitOfWork.GetRepository<ServiceImage>().GetByIdAsync(id);
            if (serviceImage == null || serviceImage.DeletedTime != null)
            {
                throw new Exception("Service image not found.");
            }

            serviceImage.DeletedTime = DateTimeOffset.UtcNow;
            await _unitOfWork.GetRepository<ServiceImage>().UpdateAsync(serviceImage);
            await _unitOfWork.SaveAsync();
        }
    }
}
