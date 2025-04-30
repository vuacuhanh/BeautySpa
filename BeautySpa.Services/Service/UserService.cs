using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.ModelViews.UserModelViews;
using BeautySpa.Services.Validations.UserValidator;
using FluentValidation;
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

        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public UserService(IUnitOfWork unitOfWork, UserManager<ApplicationUsers> userManager, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<BaseResponseModel<GETUserInfoModelView>> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid Id.");

            var userInfor = await _unitOfWork.GetRepository<UserInfor>()
                .Entities
                .Include(u => u.User)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (userInfor == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User information not found.");

            return BaseResponseModel<GETUserInfoModelView>.Success(_mapper.Map<GETUserInfoModelView>(userInfor));
        }

        public async Task<BaseResponseModel<BasePaginatedList<GETUserModelViews>>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");

            IQueryable<ApplicationUsers> query = _unitOfWork.GetRepository<ApplicationUsers>()
                .Entities
                .Include(u => u.UserInfor)
                .Where(r => !r.DeletedTime.HasValue)
                .OrderByDescending(r => r.CreatedTime);

            var totalCount = await query.CountAsync();
            var users = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var results = new List<GETUserModelViews>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var model = _mapper.Map<GETUserModelViews>(user);
                model.RoleName = roles.FirstOrDefault() ?? string.Empty;
                results.Add(model);
            }

            return BaseResponseModel<BasePaginatedList<GETUserModelViews>>.Success(
                new BasePaginatedList<GETUserModelViews>(results, totalCount, pageNumber, pageSize)
            );
        }

        public async Task<BaseResponseModel<BasePaginatedList<GETUserModelViews>>> GetCustomerAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");

            IQueryable<ApplicationUsers> query = _unitOfWork.GetRepository<ApplicationUsers>()
                .Entities
                .Include(u => u.UserInfor)
                .Where(r => !r.DeletedTime.HasValue)
                .OrderByDescending(r => r.CreatedTime);

            var totalCount = await query.CountAsync();
            var users = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var customers = new List<GETUserModelViews>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Customer"))
                {
                    var model = _mapper.Map<GETUserModelViews>(user);
                    model.RoleName = roles.FirstOrDefault() ?? string.Empty;
                    customers.Add(model);
                }
            }

            return BaseResponseModel<BasePaginatedList<GETUserModelViews>>.Success(
                new BasePaginatedList<GETUserModelViews>(customers, totalCount, pageNumber, pageSize)
            );
        }

        public async Task<BaseResponseModel<string>> UpdateAsync(PUTUserModelViews model)
        {
            await new PUTUserModelViewsValidator().ValidateAndThrowAsync(model);

            var user = await _userManager.FindByIdAsync(model.Id.ToString())
                       ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User not found.");

            var userInforRepo = _unitOfWork.GetRepository<UserInfor>();
            var userInfor = await userInforRepo.Entities
                .FirstOrDefaultAsync(u => u.UserId == user.Id);

            if (userInfor == null)
            {
                // Nếu chưa có UserInfor thì tạo mới
                userInfor = new UserInfor
                {
                    UserId = user.Id,
                    CreatedBy = CurrentUserId,
                    CreatedTime = DateTimeOffset.UtcNow
                };
                _mapper.Map(model, userInfor);
                userInfor.LastUpdatedBy = CurrentUserId;
                userInfor.LastUpdatedTime = DateTimeOffset.UtcNow;

                await userInforRepo.InsertAsync(userInfor);
            }
            else
            {
                // Update userInfor nhưng không được map đè Id
                _mapper.Map(model, userInfor);
                userInfor.LastUpdatedBy = CurrentUserId;
                userInfor.LastUpdatedTime = DateTimeOffset.UtcNow;

                await userInforRepo.UpdateAsync(userInfor);
            }

            user.PhoneNumber = model.PhoneNumber;
            user.LastUpdatedBy = CurrentUserId;
            user.LastUpdatedTime = DateTimeOffset.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.InternalServerError, "Failed to update user.");

            await _unitOfWork.SaveAsync();
            return BaseResponseModel<string>.Success("User updated successfully.");
        }

        public async Task<BaseResponseModel<string>> UpdateCustomerAsync(PUTuserforcustomer model)
        {
            await new PUTuserforcustomerValidator().ValidateAndThrowAsync(model);

            var user = await _userManager.FindByIdAsync(model.Id.ToString())
                       ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Customer not found.");

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Customer"))
                throw new ErrorException(StatusCodes.Status403Forbidden, ErrorCode.UnAuthorized, "User is not a customer.");

            var userInforRepo = _unitOfWork.GetRepository<UserInfor>();
            var userInfor = await userInforRepo.Entities
                .FirstOrDefaultAsync(u => u.UserId == user.Id);

            if (model.UserInfor != null)
            {
                if (userInfor == null)
                {
                    userInfor = new UserInfor
                    {
                        UserId = user.Id,
                        CreatedBy = CurrentUserId,
                        CreatedTime = DateTimeOffset.UtcNow
                    };
                    _mapper.Map(model.UserInfor, userInfor);
                    userInfor.LastUpdatedBy = CurrentUserId;
                    userInfor.LastUpdatedTime = DateTimeOffset.UtcNow;

                    await userInforRepo.InsertAsync(userInfor);
                }
                else
                {
                    _mapper.Map(model.UserInfor, userInfor);
                    userInfor.LastUpdatedBy = CurrentUserId;
                    userInfor.LastUpdatedTime = DateTimeOffset.UtcNow;

                    await userInforRepo.UpdateAsync(userInfor);
                }
            }

            user.Email = model.Email;
            user.LastUpdatedBy = CurrentUserId;
            user.LastUpdatedTime = DateTimeOffset.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.InternalServerError, "Failed to update customer.");

            await _unitOfWork.SaveAsync();
            return BaseResponseModel<string>.Success("Customer updated successfully.");
        }

        public async Task<BaseResponseModel<string>> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid Id.");

            var user = await _userManager.FindByIdAsync(id.ToString())
                       ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User not found.");

            user.DeletedTime = DateTimeOffset.UtcNow;
            user.Status = "inactive";
            user.DeletedBy = CurrentUserId;

            var userInforRepo = _unitOfWork.GetRepository<UserInfor>();
            var userInfor = await userInforRepo.Entities.FirstOrDefaultAsync(u => u.UserId == id);

            if (userInfor != null)
            {
                userInfor.DeletedTime = DateTimeOffset.UtcNow;
                userInfor.DeletedBy = CurrentUserId;
                await userInforRepo.UpdateAsync(userInfor);
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.BadRequest, "Failed to delete user.");

            await _unitOfWork.SaveAsync();
            return BaseResponseModel<string>.Success("User deleted successfully.");
        }
    }
}
