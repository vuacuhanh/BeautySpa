using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.ModelViews.UserModelViews;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Services.Service
{
    public class UserService : IUsers
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUsers> _userManager;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        private string currentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
        private IGenericRepository<UserInfor> UserInforRepository => _unitOfWork.GetRepository<UserInfor>();
        private IGenericRepository<ApplicationUsers> UserRepository => _unitOfWork.GetRepository<ApplicationUsers>();

        public UserService(IUnitOfWork unitOfWork, UserManager<ApplicationUsers> userManager, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<GETUserInfoModelView> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid Id");

            var userInfor = await UserInforRepository.Entities
                .Include(u => u.User)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (userInfor == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User information not found");

            return _mapper.Map<GETUserInfoModelView>(userInfor);
        }

        public async Task<GETUserModelViews> GetCustomerInfoAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid Id");

            var user = await UserRepository.Entities
                .Include(u => u.UserInfor)
                .FirstOrDefaultAsync(u => u.Id == id && u.DeletedTime == null);

            if (user == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Customer not found");

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Customer"))
                throw new ErrorException(StatusCodes.Status403Forbidden, ErrorCode.UnAuthorized, "User is not a customer");

            var result = _mapper.Map<GETUserModelViews>(user);
            result.RoleName = roles.FirstOrDefault() ?? string.Empty;
            return result;
        }

        public async Task<BasePaginatedList<GETUserModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0");

            var query = UserRepository.Entities
                .Include(u => u.UserInfor)
                .Where(u => u.DeletedTime == null)
                .OrderByDescending(u => u.CreatedTime);

            var paginatedUsers = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var results = new List<GETUserModelViews>();

            foreach (var user in paginatedUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userModel = _mapper.Map<GETUserModelViews>(user);
                userModel.RoleName = roles.FirstOrDefault() ?? string.Empty;
                results.Add(userModel);
            }

            return new BasePaginatedList<GETUserModelViews>(results, await query.CountAsync(), pageNumber, pageSize);
        }

        public async Task<BasePaginatedList<GETUserModelViews>> GetCustomerAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0");

            var query = UserRepository.Entities
                .Include(u => u.UserInfor)
                .Where(u => u.DeletedTime == null)
                .OrderByDescending(u => u.CreatedTime);

            var paginatedUsers = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var customers = new List<GETUserModelViews>();

            foreach (var user in paginatedUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Customer"))
                {
                    var userModel = _mapper.Map<GETUserModelViews>(user);
                    userModel.RoleName = roles.FirstOrDefault() ?? string.Empty;
                    customers.Add(userModel);
                }
            }

            return new BasePaginatedList<GETUserModelViews>(customers, await query.CountAsync(), pageNumber, pageSize);
        }

        public async Task UpdateAsync(PUTUserModelViews model)
        {
            if (model == null || model.Id == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid update information");

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User not found");

            var userInfor = await UserInforRepository.GetByIdAsync(user.Id) ?? new UserInfor { UserId = user.Id, CreatedBy = currentUserId };
            _mapper.Map(model, userInfor);
            userInfor.LastUpdatedTime = DateTimeOffset.UtcNow;
            userInfor.LastUpdatedBy = currentUserId;

            user.PhoneNumber = model.PhoneNumber;
            user.LastUpdatedTime = DateTimeOffset.UtcNow;
            user.LastUpdatedBy = currentUserId;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.InternalServerError, "Failed to update user");

            if (userInfor.Id == Guid.Empty)
                await UserInforRepository.InsertAsync(userInfor);
            else
                await UserInforRepository.UpdateAsync(userInfor);

            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateCustomerAsync(PUTuserforcustomer model)
        {
            if (model == null || model.Id == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid update information");

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Customer not found");

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Customer"))
                throw new ErrorException(StatusCodes.Status403Forbidden, ErrorCode.UnAuthorized, "User is not a customer");

            var userInfor = await UserInforRepository.GetByIdAsync(user.Id) ?? new UserInfor { UserId = user.Id, CreatedBy = currentUserId };

            if (model.UserInfor != null)
            {
                _mapper.Map(model.UserInfor, userInfor);
                userInfor.LastUpdatedTime = DateTimeOffset.UtcNow;
                userInfor.LastUpdatedBy = currentUserId;
            }

            user.Email = model.Email;
            user.LastUpdatedTime = DateTimeOffset.UtcNow;
            user.LastUpdatedBy = currentUserId;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.InternalServerError, "Failed to update customer");

            if (userInfor.Id == Guid.Empty)
                await UserInforRepository.InsertAsync(userInfor);
            else
                await UserInforRepository.UpdateAsync(userInfor);

            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid Id");

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User not found");

            user.DeletedTime = DateTimeOffset.UtcNow;
            user.Status = "inactive";
            user.DeletedBy = currentUserId;

            var userInfor = await UserInforRepository.GetByIdAsync(id);
            if (userInfor != null)
            {
                userInfor.DeletedTime = DateTimeOffset.UtcNow;
                userInfor.DeletedBy = currentUserId;
                await UserInforRepository.UpdateAsync(userInfor);
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.BadRequest, "Failed to delete user");

            await _unitOfWork.SaveAsync();
        }
    }
}
