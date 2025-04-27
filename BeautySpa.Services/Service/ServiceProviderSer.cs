using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ServiceProviderModelViews;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Services.Service
{
    public class ServiceProviderSer : IServiceProviders
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ServiceProviderSer(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Guid> CreateAsync(POSTServiceProviderModelViews model)
        {
            // Kiểm tra BusinessName
            if (string.IsNullOrWhiteSpace(model.BusinessName))
            {
                throw new ArgumentException("Business name cannot be empty.");
            }
            // Kiểm tra PhoneNumber
            if (string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                throw new ArgumentException("Phone number is required.");
            }

            // Kiểm tra tính duy nhất của PhoneNumber trong ServiceProviders
            var phoneExistsInProviders = await _unitOfWork.GetRepository<ServiceProvider>()
                .Entities.AnyAsync(sp => sp.PhoneNumber == model.PhoneNumber && sp.DeletedTime == null);
            if (phoneExistsInProviders)
            {
                throw new ArgumentException("Phone number is already in use by another service provider.");
            }

            // Kiểm tra sự tồn tại của ApplicationUsers
            var userExists = await _unitOfWork.GetRepository<ApplicationUsers>().GetByIdAsync(model.UserId);
            if (userExists == null)
            {
                throw new Exception("User not found.");
            }

            // Kiểm tra và đồng bộ PhoneNumber với ApplicationUsers
            if (string.IsNullOrEmpty(userExists.PhoneNumber))
            {
                userExists.PhoneNumber = model.PhoneNumber;
            }
            else if (userExists.PhoneNumber != model.PhoneNumber)
            {
                // Kiểm tra tính duy nhất trong ApplicationUsers
                var userPhoneExists = await _unitOfWork.GetRepository<ApplicationUsers>()
                    .Entities.AnyAsync(u => u.PhoneNumber == model.PhoneNumber && u.Id != model.UserId);
                if (userPhoneExists)
                {
                    throw new ArgumentException("Phone number is already in use by another user.");
                }
                userExists.PhoneNumber = model.PhoneNumber;
            }

            // Đồng bộ Email với ApplicationUsers
            if (!string.IsNullOrEmpty(model.Email) && userExists.Email != model.Email)
            {
                var userEmailExists = await _unitOfWork.GetRepository<ApplicationUsers>()
                    .Entities.AnyAsync(u => u.Email == model.Email && u.Id != model.UserId);
                if (userEmailExists)
                {
                    throw new ArgumentException("Email is already in use by another user.");
                }
                userExists.Email = model.Email;
            }

            // Cập nhật ApplicationUsers nếu có thay đổi
            if (userExists.PhoneNumber != model.PhoneNumber || userExists.Email != model.Email)
            {
                await _unitOfWork.GetRepository<ApplicationUsers>().UpdateAsync(userExists);
            }

            // Tạo ServiceProvider
            var serviceProvider = _mapper.Map<ServiceProvider>(model);
            serviceProvider.Id = Guid.NewGuid();
            serviceProvider.ProviderId = model.UserId; // Sử dụng ProviderId thay vì UserId trong entity
            serviceProvider.PhoneNumber = model.PhoneNumber;
            serviceProvider.Email = model.Email;
            serviceProvider.CreatedTime = DateTimeOffset.UtcNow;
            serviceProvider.LastUpdatedTime = serviceProvider.CreatedTime;

            await _unitOfWork.GetRepository<ServiceProvider>().InsertAsync(serviceProvider);
            await _unitOfWork.SaveAsync();

            return serviceProvider.Id;
        }

        public async Task<BasePaginatedList<GETServiceProviderModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                throw new ArgumentException("Page number and page size must be greater than 0.");
            }

            var query = _unitOfWork.GetRepository<ServiceProvider>().Entities
                .Where(sp => sp.DeletedTime == null);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedItems = _mapper.Map<IReadOnlyCollection<GETServiceProviderModelViews>>(items);

            return new BasePaginatedList<GETServiceProviderModelViews>(mappedItems, totalCount, pageNumber, pageSize);
        }

        public async Task<GETServiceProviderModelViews> GetByIdAsync(Guid id)
        {
            var serviceProvider = await _unitOfWork.GetRepository<ServiceProvider>().Entities
                .FirstOrDefaultAsync(sp => sp.Id == id && sp.DeletedTime == null);
            if (serviceProvider == null)
            {
                throw new Exception("Service provider not found.");
            }

            return _mapper.Map<GETServiceProviderModelViews>(serviceProvider);
        }

        public async Task UpdateAsync(PUTServiceProviderModelViews model)
        {
            var serviceProvider = await _unitOfWork.GetRepository<ServiceProvider>().Entities
                .FirstOrDefaultAsync(sp => sp.Id == model.Id && sp.DeletedTime == null);
            if (serviceProvider == null)
            {
                throw new Exception("Service provider not found.");
            }

            if (string.IsNullOrWhiteSpace(model.BusinessName))
            {
                throw new ArgumentException("Business name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                throw new ArgumentException("Phone number is required.");
            }

            var phoneExists = await _unitOfWork.GetRepository<ServiceProvider>()
                .Entities.AnyAsync(sp => sp.PhoneNumber == model.PhoneNumber && sp.Id != model.Id && sp.DeletedTime == null);
            if (phoneExists)
            {
                throw new ArgumentException("Phone number is already in use.");
            }

            var userExists = await _unitOfWork.GetRepository<ApplicationUsers>().GetByIdAsync(model.ProviderId);
            if (userExists == null)
            {
                throw new Exception("User not found.");
            }

            if (userExists.PhoneNumber != model.PhoneNumber)
            {
                var userPhoneExists = await _unitOfWork.GetRepository<ApplicationUsers>()
                    .Entities.AnyAsync(u => u.PhoneNumber == model.PhoneNumber && u.Id != model.ProviderId);
                if (userPhoneExists)
                {
                    throw new ArgumentException("Phone number is already in use by another user.");
                }
                userExists.PhoneNumber = model.PhoneNumber;
                await _unitOfWork.GetRepository<ApplicationUsers>().UpdateAsync(userExists);
            }

            if (!string.IsNullOrEmpty(model.Email) && userExists.Email != model.Email)
            {
                var userEmailExists = await _unitOfWork.GetRepository<ApplicationUsers>()
                    .Entities.AnyAsync(u => u.Email == model.Email && u.Id != model.ProviderId);
                if (userEmailExists)
                {
                    throw new ArgumentException("Email is already in use by another user.");
                }
                userExists.Email = model.Email;
                await _unitOfWork.GetRepository<ApplicationUsers>().UpdateAsync(userExists);
            }

            _mapper.Map(model, serviceProvider);
            serviceProvider.LastUpdatedTime = DateTimeOffset.UtcNow;

            await _unitOfWork.GetRepository<ServiceProvider>().UpdateAsync(serviceProvider);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var serviceProvider = await _unitOfWork.GetRepository<ServiceProvider>().GetByIdAsync(id);
            if (serviceProvider == null || serviceProvider.DeletedTime != null)
            {
                throw new Exception("Service provider not found.");
            }

            serviceProvider.DeletedTime = DateTimeOffset.UtcNow;
            await _unitOfWork.GetRepository<ServiceProvider>().UpdateAsync(serviceProvider);
            await _unitOfWork.SaveAsync();
        }
    }
}
