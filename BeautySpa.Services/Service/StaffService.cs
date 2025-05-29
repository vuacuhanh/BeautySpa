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
                "Provider not found for current user.");
        }

        public async Task<BaseResponseModel<Guid>> CreateAsync(POSTStaffModelView model)
        {
            await new POSTStaffModelViewValidator().ValidateAndThrowAsync(model);

            var staff = _mapper.Map<Staff>(model);
            staff.Id = Guid.NewGuid();
            staff.ProviderId = await GetProviderIdAsync();
            staff.CreatedBy = CurrentUserId;
            staff.CreatedTime = CoreHelper.SystemTimeNow;

            // Gán danh mục dịch vụ
            staff.StaffServiceCategories = model.ServiceCategoryIds.Select(id => new StaffServiceCategory
            {
                StaffId = staff.Id,
                ServiceCategoryId = id
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

            _mapper.Map(model, staff);
            staff.LastUpdatedBy = CurrentUserId;
            staff.LastUpdatedTime = CoreHelper.SystemTimeNow;

            // Cập nhật lại ServiceCategory
            var linkRepo = _unitOfWork.GetRepository<StaffServiceCategory>();
            var oldLinks = await linkRepo.Entities.Where(x => x.StaffId == model.Id).ToListAsync();
            foreach (var link in oldLinks)
                await linkRepo.DeleteAsync(link);

            staff.StaffServiceCategories = model.ServiceCategoryIds.Select(id => new StaffServiceCategory
            {
                StaffId = model.Id,
                ServiceCategoryId = id
            }).ToList();

            await repo.UpdateAsync(staff);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Staff updated successfully.");
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
                .Entities.AsNoTracking()
                .Include(s => s.StaffServiceCategories)
                .ThenInclude(ssc => ssc.ServiceCategory)
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
                .ThenInclude(ssc => ssc.ServiceCategory)
                .Where(s => s.ProviderId == providerId && s.DeletedTime == null)
                .OrderByDescending(s => s.CreatedTime);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            var result = new BasePaginatedList<GETStaffModelView>(_mapper.Map<List<GETStaffModelView>>(items), totalCount, page, size);
            return BaseResponseModel<BasePaginatedList<GETStaffModelView>>.Success(result);
        }

        public async Task<BaseResponseModel<string>> DeleteHardAsync(Guid id)
        {
            var providerId = await GetProviderIdAsync();
            var repo = _unitOfWork.GetRepository<Staff>();

            var staff = await repo.Entities
                .FirstOrDefaultAsync(s => s.Id == id && s.ProviderId == providerId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Staff not found.");

            await repo.DeleteAsync(staff.Id);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Staff permanently deleted.");
        }
    }
}
