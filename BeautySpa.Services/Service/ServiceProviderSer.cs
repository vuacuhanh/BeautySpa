using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.ServiceProviderModelViews;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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

        public async Task<BaseResponseModel<BasePaginatedList<GETServiceProviderModelViews>>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");

           IQueryable<ServiceProvider> provider = _unitOfWork.GetRepository<ServiceProvider>()
                .Entities.Where(r => !r.DeletedTime.HasValue)
                .OrderByDescending(r => r.CreatedTime);

            var items = await provider
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new BasePaginatedList<GETServiceProviderModelViews>(
                _mapper.Map<List<GETServiceProviderModelViews>>(items),
                await provider.CountAsync(),
                pageNumber, pageSize);

            return BaseResponseModel<BasePaginatedList<GETServiceProviderModelViews>>.Success(result);
        }

        public async Task<BaseResponseModel<GETServiceProviderModelViews>> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid ServiceProvider ID.");

            var entity = await _unitOfWork.GetRepository<ServiceProvider>()
                .Entities.FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service Provider not found.");

            return BaseResponseModel<GETServiceProviderModelViews>.Success(_mapper.Map<GETServiceProviderModelViews>(entity));
        }

        public async Task<BaseResponseModel<Guid>> CreateAsync(POSTServiceProviderModelViews model)
        {
            var validator = new POSTServiceProviderModelViewsValidator();
            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

            var phoneExists = await _unitOfWork.GetRepository<ServiceProvider>()
                .Entities.AnyAsync(x => x.PhoneNumber == model.PhoneNumber && x.DeletedTime == null);
            if (phoneExists)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Phone number is already in use.");

            var user = await _unitOfWork.GetRepository<ApplicationUsers>().GetByIdAsync(model.UserId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User not found.");

            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;
            await _unitOfWork.GetRepository<ApplicationUsers>().UpdateAsync(user);

            var entity = _mapper.Map<ServiceProvider>(model);
            entity.Id = Guid.NewGuid();
            entity.ProviderId = model.UserId;
            entity.CreatedBy = CurrentUserId;
            entity.CreatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<ServiceProvider>().InsertAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<Guid>.Success(entity.Id);
        }

        public async Task<BaseResponseModel<string>> UpdateAsync(PUTServiceProviderModelViews model)
        {
            var validator = new PUTServiceProviderModelViewsValidator();
            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

            var entity = await _unitOfWork.GetRepository<ServiceProvider>()
                .Entities.FirstOrDefaultAsync(x => x.Id == model.Id && x.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service Provider not found.");

            var user = await _unitOfWork.GetRepository<ApplicationUsers>().GetByIdAsync(model.ProviderId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User not found.");

            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;
            await _unitOfWork.GetRepository<ApplicationUsers>().UpdateAsync(user);

            _mapper.Map(model, entity);
            entity.LastUpdatedTime = CoreHelper.SystemTimeNow;
            entity.LastUpdatedBy = CurrentUserId;

            await _unitOfWork.GetRepository<ServiceProvider>().UpdateAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Service provider updated successfully.");
        }

        public async Task<BaseResponseModel<string>> DeleteAsync(Guid id)
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

            return BaseResponseModel<string>.Success("Service provider deleted successfully.");
        }
    }
}