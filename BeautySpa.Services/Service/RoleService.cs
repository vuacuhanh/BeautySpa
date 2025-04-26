using AutoMapper;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.RoleModelViews;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Contract.Repositories.IUOW;
using Microsoft.AspNetCore.Http;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
namespace BeautySpa.Services.Service
{
    public class RoleService : IRoles
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<ApplicationRoles> _roleManager;
        private readonly IMapper _mapper;
        private string currentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
        private readonly IHttpContextAccessor _contextAccessor;


        public RoleService(RoleManager<ApplicationRoles> roleManager, IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor )
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<BasePaginatedList<GETRoleModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
               throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");
            }

            IQueryable<ApplicationRoles> roles = _unitOfWork.GetRepository<ApplicationRoles>()
                .Entities.Where(i => !i.DeletedTime.HasValue)
                .OrderByDescending(c => c.CreatedTime).AsQueryable();

            var paginatedMemberShips = await roles
             .Skip((pageNumber - 1) * pageSize)
             .Take(pageSize)
             .ToListAsync();

            return new BasePaginatedList<GETRoleModelViews>(_mapper.Map<List<GETRoleModelViews>>(paginatedMemberShips),
                await roles.CountAsync(), pageNumber, pageSize);
        }

        public async Task<GETRoleModelViews> GetByIdAsync(Guid roleid)
        {
            if (roleid == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid membership ID.");
            }
            var existedMemberShips = await _unitOfWork.GetRepository<ApplicationUserRoles>().Entities.FirstOrDefaultAsync(p => p.RoleId == roleid) ??
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Not found membership");
            return _mapper.Map<GETRoleModelViews>(existedMemberShips);
        }

        public async Task<Guid> CreateAsync(POSTRoleModelViews rolemodel)
        {
            // Kiểm tra RoleName có hợp lệ không
            if (string.IsNullOrWhiteSpace(rolemodel.RoleName))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Role name cannot be empty.");
            }

            // Kiểm tra đã tồn tại chưa
            var roleExists = await _unitOfWork.GetRepository<ApplicationRoles>()
                .Entities.FirstOrDefaultAsync(s => s.Name.ToLower() == rolemodel.RoleName.ToLower());

            if (roleExists != null)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Role already exists.");
            }

            // Map từ model sang entity
            var role = _mapper.Map<ApplicationRoles>(rolemodel);
            role.Id = Guid.NewGuid();
            role.CreatedBy = currentUserId;
            role.CreatedTime = CoreHelper.SystemTimeNow;

            // Thêm vào DB
            await _unitOfWork.GetRepository<ApplicationRoles>().InsertAsync(role);
            await _unitOfWork.SaveAsync();

            return role.Id;
        }


        public async Task UpdateAsync(PUTRoleModelViews rolemodel)
        {
            if (string.IsNullOrWhiteSpace(rolemodel.RoleName))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Role name cannot be null or empty.");
            }

            var genericRepository = _unitOfWork.GetRepository<ApplicationRoles>();

            var role = await genericRepository.Entities
                .FirstOrDefaultAsync(r => r.Id == rolemodel.Id && !r.DeletedTime.HasValue);

            if (role == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, $"Not found Role with id = {rolemodel.Id}");
            }

            _mapper.Map(rolemodel, role);
            role.LastUpdatedBy = currentUserId;
            role.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await genericRepository.UpdateAsync(role);
            await genericRepository.SaveAsync();
        }

        public async Task DeleteAsync(Guid roleid)
        {
            if (roleid == Guid.Empty)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid Role ID.");
            }

            var genericRepository = _unitOfWork.GetRepository<ApplicationRoles>();

            var role = await genericRepository.Entities
                .FirstOrDefaultAsync(r => r.Id == roleid && !r.DeletedTime.HasValue);

            if (role == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, $"Not found Role with id = {roleid}");
            }

            role.DeletedTime = CoreHelper.SystemTimeNow;
            role.DeletedBy = currentUserId;

            await genericRepository.UpdateAsync(role);
            await genericRepository.SaveAsync();
        }

    }
}