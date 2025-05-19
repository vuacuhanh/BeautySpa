using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.RoleModelViews;
using BeautySpa.Services.Validations.RoleValidator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Services.Service
{
    public class RoleService : IRoles
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<ApplicationRoles> _roleManager;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public RoleService(RoleManager<ApplicationRoles> roleManager, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public async Task<BaseResponseModel<BasePaginatedList<GETRoleModelViews>>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");

            IQueryable<ApplicationRoles> query = _unitOfWork.GetRepository<ApplicationRoles>()
                .Entities.Where(r => !r.DeletedTime.HasValue)
                .OrderByDescending(r => r.CreatedTime);

            var pagedRoles = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new BasePaginatedList<GETRoleModelViews>(
                _mapper.Map<List<GETRoleModelViews>>(pagedRoles),
                await query.CountAsync(),
                pageNumber,
                pageSize
            );

            return BaseResponseModel<BasePaginatedList<GETRoleModelViews>>.Success(result);
        }

        public async Task<BaseResponseModel<GETRoleModelViews>> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid role ID.");

            var role = await _unitOfWork.GetRepository<ApplicationRoles>()
                .Entities.FirstOrDefaultAsync(r => r.Id == id && !r.DeletedTime.HasValue);

            if (role == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Role not found.");

            return BaseResponseModel<GETRoleModelViews>.Success(_mapper.Map<GETRoleModelViews>(role));
        }

        public async Task<BaseResponseModel<Guid>> CreateAsync(POSTRoleModelViews model)
        {
            var validator = new POSTRoleModelViewsValidator();
            var validationResult = await validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, errors);
            }

            var exists = await _roleManager.RoleExistsAsync(model.RoleName);
            if (exists)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Role already exists.");

            var role = new ApplicationRoles
            {
                Id = Guid.NewGuid(),
                Name = model.RoleName,
                NormalizedName = model.RoleName.ToUpper(),
                CreatedBy = CurrentUserId,
                CreatedTime = CoreHelper.SystemTimeNow,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.BadRequest, string.Join(", ", result.Errors.Select(x => x.Description)));

            return BaseResponseModel<Guid>.Success(role.Id);
        }
        public async Task<BaseResponseModel<string>> UpdateAsync(PUTRoleModelViews model)
        {
            var validator = new PUTRoleModelViewsValidator();
            var validationResult = await validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, errors);
            }

            var repo = _unitOfWork.GetRepository<ApplicationRoles>();
            var role = await repo.Entities.FirstOrDefaultAsync(r => r.Id == model.Id && !r.DeletedTime.HasValue);

            if (role == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Role not found.");

            role.Name = model.RoleName;
            role.NormalizedName = model.RoleName.ToUpper();
            role.LastUpdatedBy = CurrentUserId;
            role.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await repo.UpdateAsync(role);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Role updated successfully.");
        }
        public async Task<BaseResponseModel<string>> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Role ID is invalid.");

            var repo = _unitOfWork.GetRepository<ApplicationRoles>();
            var role = await repo.Entities.FirstOrDefaultAsync(r => r.Id == id && !r.DeletedTime.HasValue);

            if (role == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Role not found.");

            role.DeletedTime = CoreHelper.SystemTimeNow;
            role.DeletedBy = CurrentUserId;

            await repo.UpdateAsync(role);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Role deleted successfully.");
        }

    }
}