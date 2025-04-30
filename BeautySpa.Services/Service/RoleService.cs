using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.RoleModelViews;
using BeautySpa.Services.Validations.RoleValidator;
using FluentValidation;
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

        public async Task<BasePaginatedList<GETRoleModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");

            IQueryable<ApplicationRoles> query = _unitOfWork.GetRepository<ApplicationRoles>()
            .Entities.Where(r => !r.DeletedTime.HasValue)
            .OrderByDescending(r => r.CreatedTime);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mapped = _mapper.Map<List<GETRoleModelViews>>(items);

            return new BasePaginatedList<GETRoleModelViews>(mapped, totalCount, pageNumber, pageSize);
        }

        public async Task<GETRoleModelViews> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid role ID.");

            var role = await _unitOfWork.GetRepository<ApplicationRoles>()
                .Entities.FirstOrDefaultAsync(r => r.Id == id && !r.DeletedTime.HasValue);

            if (role == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Role not found.");

            return _mapper.Map<GETRoleModelViews>(role);
        }

        public async Task<Guid> CreateAsync(POSTRoleModelViews model)
        {
            // Validate Model
            var validator = new POSTRoleModelViewsValidator();
            var validateResult = await validator.ValidateAsync(model);
            if (!validateResult.IsValid)
            {
                var errors = string.Join("; ", validateResult.Errors.Select(e => e.ErrorMessage));
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, errors);
            }

            // Kiểm tra role đã tồn tại chưa
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

            return role.Id;
        }

        public async Task UpdateAsync(PUTRoleModelViews model)
        {
            // Validate Model
            var validator = new PUTRoleModelViewsValidator();
            var validateResult = await validator.ValidateAsync(model);
            if (!validateResult.IsValid)
            {
                var errors = string.Join("; ", validateResult.Errors.Select(e => e.ErrorMessage));
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
        }

        public async Task DeleteAsync(Guid id)
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
        }
    }
}
