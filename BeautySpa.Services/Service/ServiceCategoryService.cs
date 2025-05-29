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
using BeautySpa.Services.Validations.ServiceCategoryValidator;

namespace BeautySpa.Services.Service
{
    public class ServiceCategoryService : IServiceCategory
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public ServiceCategoryService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<BaseResponseModel<Guid>> CreateAsync(POSTServiceCategoryModelViews model)
        {
            var validator = new POSTServiceCategoryModelViewsValidator();
            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

            var exists = await _unitOfWork.GetRepository<ServiceCategory>()
                .Entities.AnyAsync(c => c.CategoryName.ToLower() == model.CategoryName.ToLower() && c.DeletedTime == null);

            if (exists)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Category already exists.");

            var entity = _mapper.Map<ServiceCategory>(model);
            entity.Id = Guid.NewGuid();
            entity.CreatedBy = CurrentUserId;
            entity.CreatedTime = CoreHelper.SystemTimeNow;
            entity.LastUpdatedBy = CurrentUserId;
            entity.LastUpdatedTime = entity.CreatedTime;

            await _unitOfWork.GetRepository<ServiceCategory>().InsertAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<Guid>.Success(entity.Id);
        }

        public async Task<BaseResponseModel<BasePaginatedList<GETServiceCategoryModelViews>>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");

            IQueryable<ServiceCategory> query = _unitOfWork.GetRepository<ServiceCategory>()
                .Entities
                .Where(c => !c.DeletedTime.HasValue)
                .OrderByDescending(c => c.CreatedTime);

            // Đếm tổng số bản ghi
            var count = await query.CountAsync();

            // Áp dụng phân trang và lấy dữ liệu
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedItems = _mapper.Map<List<GETServiceCategoryModelViews>>(items);

            var result = new BasePaginatedList<GETServiceCategoryModelViews>(mappedItems, count, pageNumber, pageSize);
            return BaseResponseModel<BasePaginatedList<GETServiceCategoryModelViews>>.Success(result);
        }

        public async Task<BaseResponseModel<GETServiceCategoryModelViews>> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid category ID.");

            var entity = await _unitOfWork.GetRepository<ServiceCategory>().Entities
                .FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Category not found.");

            return BaseResponseModel<GETServiceCategoryModelViews>.Success(_mapper.Map<GETServiceCategoryModelViews>(entity));
        }

        public async Task<BaseResponseModel<string>> UpdateAsync(PUTServiceCategoryModelViews model)
        {
            var validator = new PUTServiceCategoryModelViewsValidator();
            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

            var repo = _unitOfWork.GetRepository<ServiceCategory>();
            var entity = await repo.Entities.FirstOrDefaultAsync(c => c.Id == model.Id && c.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Category not found.");

            _mapper.Map(model, entity);
            entity.LastUpdatedBy = CurrentUserId;
            entity.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Category updated successfully.");
        }

        public async Task<BaseResponseModel<string>> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid category ID.");

            var repo = _unitOfWork.GetRepository<ServiceCategory>();
            var entity = await repo.Entities.FirstOrDefaultAsync(c => c.Id == id && c.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Category not found.");

            entity.DeletedBy = CurrentUserId;
            entity.DeletedTime = CoreHelper.SystemTimeNow;

            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Category deleted successfully.");
        }
        public async Task<BaseResponseModel<string>> DeleteHardAsync(Guid id)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var categoryRepo = _unitOfWork.GetRepository<ServiceCategory>();
                var serviceRepo = _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>();
                var spcRepo = _unitOfWork.GetRepository<ServiceProviderCategory>();
                var promotionRepo = _unitOfWork.GetRepository<ServicePromotion>();

                var category = await categoryRepo.GetByIdAsync(id);
                if (category == null)
                    throw new ErrorException(404, ErrorCode.NotFound, "Category not found");

                // Xóa các service liên quan
                var services = await serviceRepo.Entities.Where(s => s.ServiceCategoryId == id).ToListAsync();
                foreach (var service in services)
                {
                    // Xóa các ServicePromotion liên quan đến service
                    var promotions = await promotionRepo.Entities
                        .Where(p => p.ServiceId == service.Id).ToListAsync();
                    foreach (var promo in promotions)
                    {
                        await promotionRepo.DeleteAsync(promo.Id);
                    }

                    // Xóa service
                    await serviceRepo.DeleteAsync(service.Id);
                }

                // Xóa liên kết category với provider
                var links = await spcRepo.Entities.Where(x => x.ServiceCategoryId == id).ToListAsync();
                foreach (var link in links)
                {
                    await spcRepo.DeleteAsync(link.Id);
                }

                // Xoá category
                await categoryRepo.DeleteAsync(id);

                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                return BaseResponseModel<string>.Success("Service category permanently deleted.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw new ErrorException(500, ErrorCode.InternalServerError, "Failed to hard delete category.");
            }
        }
    }
}