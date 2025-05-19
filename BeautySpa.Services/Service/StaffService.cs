using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.StaffModelViews;
using BeautySpa.Services.Validations.StaffValidator;
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

        public async Task<BaseResponseModel<Guid>> CreateAsync(POSTStaffModelView model)
        {
            var validator = new POSTStaffModelViewValidator();
            var result = await validator.ValidateAsync(model);
            if (!result.IsValid)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, string.Join("; ", result.Errors.Select(e => e.ErrorMessage)));

            var staff = _mapper.Map<Staff>(model);
            staff.Id = Guid.NewGuid();
            staff.CreatedBy = CurrentUserId;
            staff.CreatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Staff>().InsertAsync(staff);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<Guid>.Success(staff.Id);
        }

        public async Task<BaseResponseModel<string>> UpdateAsync(PUTStaffModelView model)
        {
            var validator = new PUTStaffModelViewValidator();
            var result = await validator.ValidateAsync(model);
            if (!result.IsValid)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, string.Join("; ", result.Errors.Select(e => e.ErrorMessage)));

            var repo = _unitOfWork.GetRepository<Staff>();
            var staff = await repo.Entities.FirstOrDefaultAsync(s => s.Id == model.Id && s.DeletedTime == null);
            if (staff == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Staff not found.");

            _mapper.Map(model, staff);
            staff.LastUpdatedBy = CurrentUserId;
            staff.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await repo.UpdateAsync(staff);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Staff updated successfully.");
        }

        public async Task<BaseResponseModel<string>> DeleteAsync(Guid id)
        {
            var repo = _unitOfWork.GetRepository<Staff>();
            var staff = await repo.Entities.FirstOrDefaultAsync(s => s.Id == id && s.DeletedTime == null);
            if (staff == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Staff not found.");

            staff.DeletedTime = CoreHelper.SystemTimeNow;
            staff.DeletedBy = CurrentUserId;

            await repo.UpdateAsync(staff);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Staff deleted successfully.");
        }

        public async Task<BaseResponseModel<GETStaffModelView>> GetByIdAsync(Guid id)
        {
            IQueryable<Staff> query = _unitOfWork.GetRepository<Staff>()
                .Entities
                .AsNoTracking();

            var staff = await query.FirstOrDefaultAsync(s => s.Id == id && s.DeletedTime == null);
            if (staff == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Staff not found.");

            return BaseResponseModel<GETStaffModelView>.Success(_mapper.Map<GETStaffModelView>(staff));
        }

        public async Task<BaseResponseModel<BasePaginatedList<GETStaffModelView>>> GetAllAsync(int page, int size, Guid? providerId)
        {
            if (page <= 0 || size <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid pagination.");

            IQueryable<Staff> query = _unitOfWork.GetRepository<Staff>()
                .Entities
                .AsNoTracking()
                .Where(s => s.DeletedTime == null && (!providerId.HasValue || s.ProviderId == providerId))
                .OrderByDescending(s => s.CreatedTime);

            var totalCount = await query.CountAsync();
            var pagedStaff = await query.Skip((page - 1) * size).Take(size).ToListAsync();
            var result = new BasePaginatedList<GETStaffModelView>(
                _mapper.Map<List<GETStaffModelView>>(pagedStaff),
                totalCount,
                page,
                size
            );

            return BaseResponseModel<BasePaginatedList<GETStaffModelView>>.Success(result);
        }
    }
}