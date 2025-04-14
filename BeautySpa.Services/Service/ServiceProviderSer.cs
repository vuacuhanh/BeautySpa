using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ServiceProviderModelViews;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.Services.Service
{
    public class ServiceProviderSer : IServiceProviders
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ServiceProviderSer(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Guid> CreateAsync(POSTServiceProviderModelViews model)
        {
            if (string.IsNullOrWhiteSpace(model.BusinessName))
            {
                throw new ArgumentException("Business name cannot be empty.");
            }

            var providerExists = await _unitOfWork.GetRepository<ApplicationUsers>().GetByIdAsync(model.ProviderId);
            if (providerExists == null)
            {
                throw new Exception("Provider not found.");
            }

            var serviceProvider = _mapper.Map<ServiceProvider>(model);
            serviceProvider.Id = Guid.NewGuid();
            serviceProvider.CreatedTime = DateTimeOffset.UtcNow;
            serviceProvider.LastUpdatedTime = serviceProvider.CreatedTime;

            await _unitOfWork.GetRepository<ServiceProvider>().InsertAsync(serviceProvider);
            await _unitOfWork.SaveAsync();

            return serviceProvider.Id;
        }

        public async Task<BasePaginatedList<GETServiceProviderModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                throw new ArgumentException("Page number and page size must be greater than 0.");
            }

            var query = _unitOfWork.GetRepository<ServiceProvider>().Entities
                .Where(sp => sp.DeletedTime == null);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedItems = _mapper.Map<IReadOnlyCollection<GETServiceProviderModelViews>>(items);

            return new BasePaginatedList<GETServiceProviderModelViews>(mappedItems, totalCount, pageNumber, pageSize);
        }

        public async Task<GETServiceProviderModelViews> GetByIdAsync(Guid id)
        {
            var serviceProvider = await _unitOfWork.GetRepository<ServiceProvider>().Entities
                .FirstOrDefaultAsync(sp => sp.Id == id && sp.DeletedTime == null);
            if (serviceProvider == null)
            {
                throw new Exception("Service provider not found.");
            }

            return _mapper.Map<GETServiceProviderModelViews>(serviceProvider);
        }

        public async Task UpdateAsync(PUTServiceProviderModelViews model)
        {
            var serviceProvider = await _unitOfWork.GetRepository<ServiceProvider>().Entities
                .FirstOrDefaultAsync(sp => sp.Id == model.Id && sp.DeletedTime == null);
            if (serviceProvider == null)
            {
                throw new Exception("Service provider not found.");
            }

            if (string.IsNullOrWhiteSpace(model.BusinessName))
            {
                throw new ArgumentException("Business name cannot be empty.");
            }

            var providerExists = await _unitOfWork.GetRepository<ApplicationUsers>().GetByIdAsync(model.ProviderId);
            if (providerExists == null)
            {
                throw new Exception("Provider not found.");
            }

            _mapper.Map(model, serviceProvider);
            serviceProvider.LastUpdatedTime = DateTimeOffset.UtcNow;

            await _unitOfWork.GetRepository<ServiceProvider>().UpdateAsync(serviceProvider);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var serviceProvider = await _unitOfWork.GetRepository<ServiceProvider>().GetByIdAsync(id);
            if (serviceProvider == null || serviceProvider.DeletedTime != null)
            {
                throw new Exception("Service provider not found.");
            }

            serviceProvider.DeletedTime = DateTimeOffset.UtcNow;
            await _unitOfWork.GetRepository<ServiceProvider>().UpdateAsync(serviceProvider);
            await _unitOfWork.SaveAsync();
        }
    }
}
