using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ServiceCategoryModelViews;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySpa.Services.Service
{
    public class ServiceCategoryService : IServiceCategory
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ServiceCategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Guid> CreateAsync(POSTServiceCategoryModelViews model)
        {
            if (string.IsNullOrWhiteSpace(model.CategoryName))
            {
                throw new ArgumentException("Category name cannot be empty.");
            }

            var category = _mapper.Map<ServiceCategory>(model);
            category.Id = Guid.NewGuid();
            category.CreatedTime = DateTimeOffset.UtcNow;
            category.LastUpdatedTime = category.CreatedTime;

            await _unitOfWork.GetRepository<ServiceCategory>().InsertAsync(category);
            await _unitOfWork.SaveAsync();

            return category.Id;
        }

        public async Task<BasePaginatedList<GETServiceCategoryModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                throw new ArgumentException("Page number and page size must be greater than 0.");
            }

            var query = _unitOfWork.GetRepository<ServiceCategory>().Entities
                .Where(c => c.DeletedTime == null);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedItems = _mapper.Map<IReadOnlyCollection<GETServiceCategoryModelViews>>(items);

            return new BasePaginatedList<GETServiceCategoryModelViews>(mappedItems, totalCount, pageNumber, pageSize);
        }

        public async Task<GETServiceCategoryModelViews> GetByIdAsync(Guid id)
        {
            var category = await _unitOfWork.GetRepository<ServiceCategory>().Entities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null);
            if (category == null)
            {
                throw new Exception("Service category not found.");
            }

            return _mapper.Map<GETServiceCategoryModelViews>(category);
        }

        public async Task UpdateAsync(PUTServiceCategoryModelViews model)
        {
            var category = await _unitOfWork.GetRepository<ServiceCategory>().Entities
                .FirstOrDefaultAsync(c => c.Id == model.Id && c.DeletedTime == null);
            if (category == null)
            {
                throw new Exception("Service category not found.");
            }

            if (string.IsNullOrWhiteSpace(model.CategoryName))
            {
                throw new ArgumentException("Category name cannot be empty.");
            }

            _mapper.Map(model, category);
            category.LastUpdatedTime = DateTimeOffset.UtcNow;

            await _unitOfWork.GetRepository<ServiceCategory>().UpdateAsync(category);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await _unitOfWork.GetRepository<ServiceCategory>().GetByIdAsync(id);
            if (category == null || category.DeletedTime != null)
            {
                throw new Exception("Service category not found.");
            }

            category.DeletedTime = DateTimeOffset.UtcNow;
            await _unitOfWork.GetRepository<ServiceCategory>().UpdateAsync(category);
            await _unitOfWork.SaveAsync();
        }
    }
}
