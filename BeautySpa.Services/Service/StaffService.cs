using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.StaffModelViews;
using BeautySpa.Services.Validations.StaffValidator;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BeautySpa.Services.Service
{
    public class StaffService : IStaff
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public StaffService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        private async Task<Guid> GetProviderIdAsync()
        {
            var userId = Guid.Parse(CurrentUserId);
            var provider = await _unitOfWork.GetRepository<ServiceProvider>()
                .Entities.FirstOrDefaultAsync(x => x.ProviderId == userId && x.DeletedTime == null);

            return provider?.Id ?? throw new ErrorException(
                StatusCodes.Status403Forbidden,
                ErrorCode.UnAuthenticated,
                "Provider not found for current user."
            );
        }

        public async Task<BaseResponseModel<Guid>> CreateAsync(POSTStaffModelView model)
        {
            await new POSTStaffModelViewValidator().ValidateAndThrowAsync(model);

            var providerId = await GetProviderIdAsync();

            // Kiểm tra các ServiceCategory có thuộc provider không
            var validCategories = await ValidateServiceCategories(model.ServiceCategoryIds, providerId);
            if (validCategories.Count != model.ServiceCategoryIds.Count)
            {
                throw new ErrorException(
                    StatusCodes.Status400BadRequest,
                    ErrorCode.InvalidInput,
                    "Some service categories are invalid or not belong to your provider");
            }

            var staff = _mapper.Map<Staff>(model);
            staff.Id = Guid.NewGuid();
            staff.ProviderId = providerId;
            staff.CreatedBy = CurrentUserId;
            staff.CreatedTime = CoreHelper.SystemTimeNow;

            // Thêm chuyên môn cho nhân viên
            staff.StaffServiceCategories = model.ServiceCategoryIds.Select(categoryId =>
                new StaffServiceCategory
                {
                    StaffId = staff.Id,
                    ServiceCategoryId = categoryId
                }).ToList();

            await _unitOfWork.GetRepository<Staff>().InsertAsync(staff);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<Guid>.Success(staff.Id);
        }

        public async Task<BaseResponseModel<string>> UpdateAsync(PUTStaffModelView model)
        {
            await new PUTStaffModelViewValidator().ValidateAndThrowAsync(model);

            var providerId = await GetProviderIdAsync();
            var repo = _unitOfWork.GetRepository<Staff>();

            var staff = await repo.Entities
                .Include(s => s.StaffServiceCategories)
                .FirstOrDefaultAsync(s => s.Id == model.Id && s.ProviderId == providerId && s.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Staff not found.");

            // Kiểm tra các ServiceCategory có thuộc provider không
            var validCategories = await ValidateServiceCategories(model.ServiceCategoryIds, providerId);
            if (validCategories.Count != model.ServiceCategoryIds.Count)
            {
                throw new ErrorException(
                    StatusCodes.Status400BadRequest,
                    ErrorCode.InvalidInput,
                    "Some service categories are invalid or not belong to your provider");
            }

            _mapper.Map(model, staff);
            staff.LastUpdatedBy = CurrentUserId;
            staff.LastUpdatedTime = CoreHelper.SystemTimeNow;

            // Cập nhật chuyên môn
            staff.StaffServiceCategories.Clear();
            foreach (var categoryId in model.ServiceCategoryIds)
            {
                staff.StaffServiceCategories.Add(new StaffServiceCategory
                {
                    StaffId = staff.Id,
                    ServiceCategoryId = categoryId
                });
            }

            await repo.UpdateAsync(staff);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Staff updated successfully.");
        }

        private async Task<List<Guid>> ValidateServiceCategories(List<Guid> categoryIds, Guid providerId)
        {
            if (categoryIds == null || !categoryIds.Any())
                return new List<Guid>();

            // Lấy danh sách category thuộc provider
            var validCategories = await _unitOfWork.GetRepository<ServiceProviderCategory>()
                .Entities
                .Where(spc => spc.ServiceProviderId == providerId &&
                             categoryIds.Contains(spc.ServiceCategoryId))
                .Select(spc => spc.ServiceCategoryId)
                .ToListAsync();

            return validCategories;
        }

        public async Task<BaseResponseModel<string>> DeleteAsync(Guid id)
        {
            var providerId = await GetProviderIdAsync();
            var repo = _unitOfWork.GetRepository<Staff>();

            var staff = await repo.Entities
                .FirstOrDefaultAsync(s => s.Id == id && s.ProviderId == providerId && s.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Staff not found.");

            staff.DeletedTime = CoreHelper.SystemTimeNow;
            staff.DeletedBy = CurrentUserId;

            await repo.UpdateAsync(staff);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Staff deleted successfully.");
        }

        public async Task<BaseResponseModel<GETStaffModelView>> GetByIdAsync(Guid id)
        {
            var providerId = await GetProviderIdAsync();

            var staff = await _unitOfWork.GetRepository<Staff>()
                .Entities
                .AsNoTracking()
                .Include(s => s.StaffServiceCategories)
                .FirstOrDefaultAsync(s => s.Id == id && s.ProviderId == providerId && s.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Staff not found.");

            return BaseResponseModel<GETStaffModelView>.Success(_mapper.Map<GETStaffModelView>(staff));
        }

        public async Task<BaseResponseModel<BasePaginatedList<GETStaffModelView>>> GetAllAsync(int page, int size)
        {
            if (page <= 0 || size <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid pagination.");

            var providerId = await GetProviderIdAsync();

            var query = _unitOfWork.GetRepository<Staff>().Entities
                .AsNoTracking()
                .Include(s => s.StaffServiceCategories)
                .Where(s => s.ProviderId == providerId && s.DeletedTime == null)
                .OrderByDescending(s => s.CreatedTime);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            var result = new BasePaginatedList<GETStaffModelView>(
                _mapper.Map<List<GETStaffModelView>>(items),
                totalCount,
                page,
                size);

            return BaseResponseModel<BasePaginatedList<GETStaffModelView>>.Success(result);
        }
    }
}