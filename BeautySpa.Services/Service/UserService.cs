using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.UserModelViews;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BeautySpa.Services.Service
{
    public class UserService : IUsers
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUsers> _userManager;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<UserInfor> _userInforRepository;

        // Constructor tiêm IUnitOfWork, UserManager và IMapper
        public UserService(IUnitOfWork unitOfWork, UserManager<ApplicationUsers> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _userInforRepository = _unitOfWork.GetRepository<UserInfor>();
        }

        // Lấy thông tin người dùng theo ID
        public async Task<GETUserInfoModelView> GetByIdAsync(Guid id)
        {
            var userInfor = await _userInforRepository.Entities
                .Include(ui => ui.User)
                .FirstOrDefaultAsync(ui => ui.UserId == id);

            if (userInfor == null)
            {
                throw new Exception("User information not found.");
            }

            return _mapper.Map<GETUserInfoModelView>(userInfor);
        }

        // Lấy danh sách người dùng phân trang
        public async Task<BasePaginatedList<GETUserModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _userManager.Users
                .Include(u => u.UserInfor)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedItems = _mapper.Map<IReadOnlyCollection<GETUserModelViews>>(items);

            foreach (var item in mappedItems)
            {
                var user = items.FirstOrDefault(u => u.Id == item.Id);
                if (user != null)
                {
                    item.Roles = (await _userManager.GetRolesAsync(user)).ToList();
                    item.RoleName = item.Roles.FirstOrDefault() ?? string.Empty;
                }
            }

            return new BasePaginatedList<GETUserModelViews>(mappedItems, totalCount, pageNumber, pageSize);
        }

        // Lấy thông tin khách hàng
        public async Task<GETUserInfoforcustomerModelView> GetCustomerInfoAsync(Guid id)
        {
            var user = await _userManager.Users
                .Include(u => u.UserInfor)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                throw new Exception("Customer not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Customer"))
            {
                throw new Exception("User is not a customer.");
            }

            return _mapper.Map<GETUserInfoforcustomerModelView>(user);
        }

        // Cập nhật thông tin người dùng
        public async Task UpdateAsync(PUTUserModelViews model)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Cập nhật thông tin UserInfor
            var userInfor = await _userInforRepository.GetByIdAsync(user.Id);
            if (userInfor == null)
            {
                userInfor = new UserInfor { UserId = user.Id };
                await _userInforRepository.InsertAsync(userInfor);
            }

            _mapper.Map(model, userInfor);
            userInfor.LastUpdatedTime = DateTimeOffset.UtcNow;

            // Cập nhật thông tin ApplicationUsers
            user.PhoneNumber = model.PhoneNumber;
            user.LastUpdatedTime = DateTimeOffset.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to update user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _userInforRepository.UpdateAsync(userInfor);
            await _unitOfWork.SaveAsync();
        }

        // Cập nhật thông tin khách hàng
        public async Task UpdateCustomerAsync(PUTuserforcustomer model)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                throw new Exception("Customer not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Customer"))
            {
                throw new Exception("User is not a customer.");
            }

            // Cập nhật thông tin UserInfor
            var userInfor = await _userInforRepository.GetByIdAsync(user.Id);
            if (userInfor == null)
            {
                userInfor = new UserInfor { UserId = user.Id };
                await _userInforRepository.InsertAsync(userInfor);
            }

            if (model.UserInfor != null)
            {
                _mapper.Map(model.UserInfor, userInfor);
                userInfor.LastUpdatedTime = DateTimeOffset.UtcNow;
            }

            // Cập nhật thông tin ApplicationUsers
            user.Email = model.Email;
            user.LastUpdatedTime = DateTimeOffset.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to update customer: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _userInforRepository.UpdateAsync(userInfor);
            await _unitOfWork.SaveAsync();
        }

        // Xóa người dùng (đánh dấu là inactive)
        public async Task DeleteAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            user.DeletedTime = DateTimeOffset.UtcNow;
            user.Status = "inactive";

            var userInfor = await _userInforRepository.GetByIdAsync(id);
            if (userInfor != null)
            {
                userInfor.DeletedTime = DateTimeOffset.UtcNow;
                await _userInforRepository.UpdateAsync(userInfor);
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to delete user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _unitOfWork.SaveAsync();
        }
    }
}