using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.ModelViews.ServiceProviderModelViews;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using BeautySpa.Core.Utils;

namespace BeautySpa.Services.Service
{
    public class ServiceProviderSer : IServiceProviders
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public ServiceProviderSer(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<Guid> CreateAsync(POSTServiceProviderModelViews model)
        {
            if (string.IsNullOrWhiteSpace(model.BusinessName))
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Business name cannot be empty.");

            if (string.IsNullOrWhiteSpace(model.PhoneNumber))
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Phone number is required.");

            // Kiểm tra số điện thoại đã tồn tại trong ServiceProvider
            var phoneExists = await _unitOfWork.GetRepository<ServiceProvider>()
                .Entities.AnyAsync(x => x.PhoneNumber == model.PhoneNumber && x.DeletedTime == null);

            if (phoneExists)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Phone number is already in use.");

            // Kiểm tra User có tồn tại
            var user = await _unitOfWork.GetRepository<ApplicationUsers>().GetByIdAsync(model.UserId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User not found.");

            // Cập nhật PhoneNumber và Email cho ApplicationUsers nếu cần
            if (user.PhoneNumber != model.PhoneNumber)
            {
                bool phoneInUse = await _unitOfWork.GetRepository<ApplicationUsers>()
                    .Entities.AnyAsync(u => u.PhoneNumber == model.PhoneNumber && u.Id != model.UserId);

                if (phoneInUse)
                    throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Phone number is already used by another user.");

                user.PhoneNumber = model.PhoneNumber;
            }

            if (!string.IsNullOrEmpty(model.Email) && user.Email != model.Email)
            {
                bool emailInUse = await _unitOfWork.GetRepository<ApplicationUsers>()
                    .Entities.AnyAsync(u => u.Email == model.Email && u.Id != model.UserId);

                if (emailInUse)
                    throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Email is already used by another user.");

                user.Email = model.Email;
            }

            await _unitOfWork.GetRepository<ApplicationUsers>().UpdateAsync(user);

            var entity = _mapper.Map<ServiceProvider>(model);
            entity.Id = Guid.NewGuid();
            entity.ProviderId = model.UserId;
            entity.CreatedTime = CoreHelper.SystemTimeNow;
            entity.LastUpdatedTime = entity.CreatedTime;
            entity.CreatedBy = CurrentUserId;
            entity.LastUpdatedBy = CurrentUserId;

            await _unitOfWork.GetRepository<ServiceProvider>().InsertAsync(entity);
            await _unitOfWork.SaveAsync();

            return entity.Id;
        }

        public async Task<BasePaginatedList<GETServiceProviderModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");

            IQueryable<ServiceProvider> provider = _unitOfWork.GetRepository<ServiceProvider>()
                .Entities.Where(x => x.DeletedTime == null)
                .OrderByDescending(x => x.CreatedTime);

            var paginatedProvider = await provider
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new BasePaginatedList<GETServiceProviderModelViews>(_mapper.Map<List<GETServiceProviderModelViews>>(paginatedProvider),
                await provider.CountAsync(), pageNumber, pageSize);
        }

        public async Task<GETServiceProviderModelViews> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid ServiceProvider ID.");

            var entity = await _unitOfWork.GetRepository<ServiceProvider>()
                .Entities.FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service Provider not found.");

            return _mapper.Map<GETServiceProviderModelViews>(entity);
        }

        public async Task UpdateAsync(PUTServiceProviderModelViews model)
        {
            var entity = await _unitOfWork.GetRepository<ServiceProvider>()
                .Entities.FirstOrDefaultAsync(x => x.Id == model.Id && x.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service Provider not found.");

            if (string.IsNullOrWhiteSpace(model.BusinessName))
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Business name cannot be empty.");

            if (string.IsNullOrWhiteSpace(model.PhoneNumber))
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Phone number is required.");

            // Kiểm tra số điện thoại trùng
            bool phoneExists = await _unitOfWork.GetRepository<ServiceProvider>()
                .Entities.AnyAsync(x => x.PhoneNumber == model.PhoneNumber && x.Id != model.Id && x.DeletedTime == null);

            if (phoneExists)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Phone number already in use.");

            var user = await _unitOfWork.GetRepository<ApplicationUsers>().GetByIdAsync(model.ProviderId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User not found.");

            if (user.PhoneNumber != model.PhoneNumber)
            {
                bool userPhoneExists = await _unitOfWork.GetRepository<ApplicationUsers>()
                    .Entities.AnyAsync(u => u.PhoneNumber == model.PhoneNumber && u.Id != model.ProviderId);

                if (userPhoneExists)
                    throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Phone number already used by another user.");

                user.PhoneNumber = model.PhoneNumber;
            }

            if (!string.IsNullOrEmpty(model.Email) && user.Email != model.Email)
            {
                bool userEmailExists = await _unitOfWork.GetRepository<ApplicationUsers>()
                    .Entities.AnyAsync(u => u.Email == model.Email && u.Id != model.ProviderId);

                if (userEmailExists)
                    throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Email already used by another user.");

                user.Email = model.Email;
            }

            await _unitOfWork.GetRepository<ApplicationUsers>().UpdateAsync(user);

            _mapper.Map(model, entity);
            entity.LastUpdatedTime = DateTimeOffset.UtcNow;
            entity.LastUpdatedBy = CurrentUserId;

            await _unitOfWork.GetRepository<ServiceProvider>().UpdateAsync(entity);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid ServiceProvider ID.");

            var entity = await _unitOfWork.GetRepository<ServiceProvider>().GetByIdAsync(id)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service Provider not found.");

            if (entity.DeletedTime != null)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Service Provider already deleted.");

            entity.DeletedTime = CoreHelper.SystemTimeNow;
            entity.DeletedBy = CurrentUserId;

            await _unitOfWork.GetRepository<ServiceProvider>().UpdateAsync(entity);
            await _unitOfWork.SaveAsync();
        }
    }
}
