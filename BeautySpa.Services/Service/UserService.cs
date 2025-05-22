using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.UserModelViews;
using BeautySpa.Services.Validations.UserValidator;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServiceProvider = BeautySpa.Contract.Repositories.Entity.ServiceProvider;

namespace BeautySpa.Services.Service
{
    public class UserService : IUsers
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUsers> _userManager;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IEsgooService _esgoo;

        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public UserService(IUnitOfWork unitOfWork, UserManager<ApplicationUsers> userManager, IMapper mapper, IHttpContextAccessor contextAccessor, IEsgooService esgoo)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _esgoo = esgoo; 
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

        //public async Task<BaseResponseModel<BasePaginatedList<GETUserModelViews>>> GetCustomerAsync(int pageNumber, int pageSize)
        //{
        //    if (pageNumber <= 0 || pageSize <= 0)
        //        throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");

        //    IQueryable<ApplicationUsers> query = _unitOfWork.GetRepository<ApplicationUsers>()
        //        .Entities
        //        .Include(u => u.UserInfor)
        //        .Where(r => !r.DeletedTime.HasValue)
        //        .OrderByDescending(r => r.CreatedTime);

        //    var totalCount = await query.CountAsync();
        //    var users = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        //    var customers = new List<GETUserModelViews>();
        //    foreach (var user in users)
        //    {
        //        var roles = await _userManager.GetRolesAsync(user);
        //        if (roles.Contains("Customer"))
        //        {
        //            var model = _mapper.Map<GETUserModelViews>(user);
        //            model.RoleName = roles.FirstOrDefault() ?? string.Empty;
        //            customers.Add(model);
        //        }
        //    }

        //    return BaseResponseModel<BasePaginatedList<GETUserModelViews>>.Success(
        //        new BasePaginatedList<GETUserModelViews>(customers, totalCount, pageNumber, pageSize)
        //    );
        //}
        public async Task<BaseResponseModel<BasePaginatedList<GETUserModelViews>>> GetByRoleAsync(string role, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");
            if (string.IsNullOrWhiteSpace(role))
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Role must be provided.");

            IQueryable<ApplicationUsers> query = _unitOfWork.GetRepository<ApplicationUsers>()
                .Entities
                .Include(u => u.UserInfor)
                .Where(r => !r.DeletedTime.HasValue)
                .OrderByDescending(r => r.CreatedTime);

            var totalCount = await query.CountAsync();
            var users = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var filteredUsers = new List<GETUserModelViews>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains(role, StringComparer.OrdinalIgnoreCase))
                {
                    var model = _mapper.Map<GETUserModelViews>(user);
                    model.RoleName = roles.FirstOrDefault() ?? string.Empty;
                    filteredUsers.Add(model);
                }
            }

            return BaseResponseModel<BasePaginatedList<GETUserModelViews>>.Success(
                new BasePaginatedList<GETUserModelViews>(filteredUsers, totalCount, pageNumber, pageSize)
            );
        }
        public async Task<BaseResponseModel<string>> UpdateAsync(PUTUserModelViews model)
        {
            await new PUTUserModelViewsValidator().ValidateAndThrowAsync(model);

            var user = await _userManager.FindByIdAsync(model.Id.ToString())
                       ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User not found.");

            var userInforRepo = _unitOfWork.GetRepository<UserInfor>();
            var userInfor = await userInforRepo.Entities.FirstOrDefaultAsync(u => u.UserId == user.Id);

            if (userInfor == null)
            {
                userInfor = new UserInfor
                {
                    UserId = user.Id,
                    CreatedBy = CurrentUserId,
                    CreatedTime = CoreHelper.SystemTimeNow
                };
                _mapper.Map(model, userInfor);
                userInfor.LastUpdatedBy = CurrentUserId;
                userInfor.LastUpdatedTime = CoreHelper.SystemTimeNow;

                // Lấy tên tỉnh và huyện
                if (!string.IsNullOrWhiteSpace(model.ProvinceId))
                {
                    var province = await _esgoo.GetProvinceByIdAsync(model.ProvinceId);
                    if (province != null)
                        userInfor.ProvinceName = province.name;
                }

                if (!string.IsNullOrWhiteSpace(model.ProvinceId) && !string.IsNullOrWhiteSpace(model.DistrictId))
                {
                    var district = await _esgoo.GetDistrictByIdAsync(model.DistrictId, model.ProvinceId);
                    if (district != null)
                        userInfor.DistrictName = district.name;
                }

                await userInforRepo.InsertAsync(userInfor);
            }
            else
            {
                _mapper.Map(model, userInfor);
                userInfor.LastUpdatedBy = CurrentUserId;
                userInfor.LastUpdatedTime = CoreHelper.SystemTimeNow;

                if (!string.IsNullOrWhiteSpace(model.ProvinceId))
                {
                    var province = await _esgoo.GetProvinceByIdAsync(model.ProvinceId);
                    if (province != null)
                        userInfor.ProvinceName = province.name;
                }

                if (!string.IsNullOrWhiteSpace(model.ProvinceId) && !string.IsNullOrWhiteSpace(model.DistrictId))
                {
                    var district = await _esgoo.GetDistrictByIdAsync(model.DistrictId, model.ProvinceId);
                    if (district != null)
                        userInfor.DistrictName = district.name;
                }

                await userInforRepo.UpdateAsync(userInfor);
            }

            // Cập nhật thông tin từ ApplicationUsers nếu cần (PhoneNumber)
            user.PhoneNumber = model.PhoneNumber;
            user.LastUpdatedBy = CurrentUserId;
            user.LastUpdatedTime = CoreHelper.SystemTimeNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.InternalServerError, "Failed to update user.");

            await _unitOfWork.SaveAsync();
            return BaseResponseModel<string>.Success("User updated successfully.");
        }

        //public async Task<BaseResponseModel<string>> UpdateCustomerAsync(PUTuserforcustomer model)
        //{
        //    await new PUTuserforcustomerValidator().ValidateAndThrowAsync(model);

        //    var user = await _userManager.FindByIdAsync(model.Id.ToString())
        //               ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Customer not found.");

        //    var roles = await _userManager.GetRolesAsync(user);
        //    if (!roles.Contains("Customer"))
        //        throw new ErrorException(StatusCodes.Status403Forbidden, ErrorCode.UnAuthorized, "User is not a customer.");

        //    var userInforRepo = _unitOfWork.GetRepository<UserInfor>();
        //    var userInfor = await userInforRepo.Entities
        //        .FirstOrDefaultAsync(u => u.UserId == user.Id);

        //    if (model.UserInfor != null)
        //    {
        //        if (userInfor == null)
        //        {
        //            userInfor = new UserInfor
        //            {
        //                UserId = user.Id,
        //                CreatedBy = CurrentUserId,
        //                CreatedTime = DateTimeOffset.UtcNow
        //            };
        //            _mapper.Map(model.UserInfor, userInfor);
        //            userInfor.LastUpdatedBy = CurrentUserId;
        //            userInfor.LastUpdatedTime = DateTimeOffset.UtcNow;

        //            await userInforRepo.InsertAsync(userInfor);
        //        }
        //        else
        //        {
        //            _mapper.Map(model.UserInfor, userInfor);
        //            userInfor.LastUpdatedBy = CurrentUserId;
        //            userInfor.LastUpdatedTime = CoreHelper.SystemTimeNow;

        //            await userInforRepo.UpdateAsync(userInfor);
        //        }
        //    }

        //    user.Email = model.Email;
        //    user.LastUpdatedBy = CurrentUserId;
        //    user.LastUpdatedTime = DateTimeOffset.UtcNow;

        //    var result = await _userManager.UpdateAsync(user);
        //    if (!result.Succeeded)
        //        throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.InternalServerError, "Failed to update customer.");

        //    await _unitOfWork.SaveAsync();
        //    return BaseResponseModel<string>.Success("Customer updated successfully.");
        //}

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
        public async Task<BaseResponseModel<string>> DeactivateProviderAccountAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid user ID.");

            var user = await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User not found.");

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Provider"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Provider");
            }

            // Xóa mềm ServiceProvider nếu có
            var spRepo = _unitOfWork.GetRepository<ServiceProvider>();
            var serviceProvider = await spRepo.Entities.FirstOrDefaultAsync(x => x.ProviderId == user.Id && x.DeletedTime == null);
            if (serviceProvider != null)
            {
                serviceProvider.DeletedTime = CoreHelper.SystemTimeNow;
                serviceProvider.DeletedBy = CurrentUserId;
                await spRepo.UpdateAsync(serviceProvider);
            }

            // Khóa tài khoản
            user.Status = "inactive";
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;
            user.LastUpdatedBy = CurrentUserId;
            user.LastUpdatedTime = CoreHelper.SystemTimeNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.InternalServerError, "Failed to deactivate user.");

            await _unitOfWork.SaveAsync();
            return BaseResponseModel<string>.Success("Provider account deactivated successfully.");
        }
        public async Task<BaseResponseModel<string>> ReactivateProviderAccountAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid user ID.");

            var user = await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User not found.");

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Provider"))
            {
                var roleExists = await _unitOfWork.GetRepository<ApplicationRoles>().Entities
                    .AnyAsync(r => r.Name == "Provider" && r.DeletedTime == null);

                if (!roleExists)
                {
                    var roleManager = _contextAccessor.HttpContext?.RequestServices.GetRequiredService<RoleManager<ApplicationRoles>>();
                    await roleManager!.CreateAsync(new ApplicationRoles { Name = "Provider" });
                }

                await _userManager.AddToRoleAsync(user, "Provider");
            }

            // Khôi phục tài khoản
            user.Status = "active";
            user.LockoutEnabled = false;
            user.LockoutEnd = null;
            user.LastUpdatedBy = CurrentUserId;
            user.LastUpdatedTime = CoreHelper.SystemTimeNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.InternalServerError, "Failed to reactivate user.");

            // Khôi phục ServiceProvider nếu có
            var spRepo = _unitOfWork.GetRepository<ServiceProvider>();
            var serviceProvider = await spRepo.Entities
                .IgnoreQueryFilters() // nếu dùng global filter cho soft delete
                .FirstOrDefaultAsync(x => x.ProviderId == user.Id && x.DeletedTime != null);

            if (serviceProvider != null)
            {
                serviceProvider.DeletedTime = null;
                serviceProvider.DeletedBy = null;
                serviceProvider.LastUpdatedBy = CurrentUserId;
                serviceProvider.LastUpdatedTime = CoreHelper.SystemTimeNow;

                await spRepo.UpdateAsync(serviceProvider);
            }

            await _unitOfWork.SaveAsync();
            return BaseResponseModel<string>.Success("Provider account reactivated successfully.");
        }

    }

}
