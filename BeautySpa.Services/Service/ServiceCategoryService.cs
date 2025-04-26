using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ServiceCategoryModelViews;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;

namespace BeautySpa.Services.Service
{
    public class ServiceCategoryService : IServiceCategory
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private string currentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public ServiceCategoryService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<Guid> CreateAsync(POSTServiceCategoryModelViews model)
        {
            // Kiểm tra CategoryName có hợp lệ không
            if (string.IsNullOrWhiteSpace(model.CategoryName))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Category name cannot be empty.");
            }

            // Kiểm tra đã tồn tại chưa
            var categoryExists = await _unitOfWork.GetRepository<ServiceCategory>()
                .Entities.FirstOrDefaultAsync(s => s.CategoryName.ToLower() == model.CategoryName.ToLower());

            if (categoryExists != null)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Category already exists.");
            }

            // Map từ model sang entity
            var category = _mapper.Map<ServiceCategory>(model);
            category.Id = Guid.NewGuid();
            category.CreatedBy = currentUserId;
            category.CreatedTime = CoreHelper.SystemTimeNow;
            category.LastUpdatedBy = currentUserId;
            category.LastUpdatedTime = CoreHelper.SystemTimeNow;

            // Thêm vào DB
            await _unitOfWork.GetRepository<ServiceCategory>().InsertAsync(category);
            await _unitOfWork.SaveAsync();

            return category.Id;
        }

        public async Task<BasePaginatedList<GETServiceCategoryModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");
            }

            IQueryable<ServiceCategory> categories = _unitOfWork.GetRepository<ServiceCategory>()
                .Entities.Where(c => !c.DeletedTime.HasValue)
                .OrderByDescending(c => c.CreatedTime).AsQueryable();

            var paginatedCategories = await categories
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new BasePaginatedList<GETServiceCategoryModelViews>(_mapper.Map<List<GETServiceCategoryModelViews>>(paginatedCategories),
                await categories.CountAsync(), pageNumber, pageSize);
        }

        public async Task<GETServiceCategoryModelViews> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid category ID.");
            }

            var category = await _unitOfWork.GetRepository<ServiceCategory>().Entities
                .FirstOrDefaultAsync(c => c.Id == id && !c.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service category not found.");

            return _mapper.Map<GETServiceCategoryModelViews>(category);
        }

        public async Task UpdateAsync(PUTServiceCategoryModelViews model)
        {
            if (model.Id == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid category ID.");
            }

            if (string.IsNullOrWhiteSpace(model.CategoryName))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Category name cannot be empty.");
            }

            var genericRepository = _unitOfWork.GetRepository<ServiceCategory>();

            var category = await genericRepository.Entities
                .FirstOrDefaultAsync(c => c.Id == model.Id && !c.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, $"Service category with id = {model.Id} not found.");

            _mapper.Map(model, category);
            category.LastUpdatedBy = currentUserId;
            category.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await genericRepository.UpdateAsync(category);
            await genericRepository.SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid category ID.");
            }

            var genericRepository = _unitOfWork.GetRepository<ServiceCategory>();

            var category = await genericRepository.Entities
                .FirstOrDefaultAsync(c => c.Id == id && !c.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, $"Service category with id = {id} not found.");

            category.DeletedTime = CoreHelper.SystemTimeNow;
            category.DeletedBy = currentUserId;

            await genericRepository.UpdateAsync(category);
            await genericRepository.SaveAsync();
        }
    }
}