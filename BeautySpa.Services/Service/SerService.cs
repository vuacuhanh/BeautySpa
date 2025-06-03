using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.ServiceModelViews;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using AutoMapper.QueryableExtensions;

namespace BeautySpa.Services.Service
{
    public class SerService : IServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public SerService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<BaseResponseModel<Guid>> CreateAsync(POSTServiceModelViews model)
        {
            if (string.IsNullOrWhiteSpace(model.ServiceName))
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Service name cannot be empty.");

            var currentUserId = Guid.Parse(CurrentUserId);
            var provider = await _unitOfWork.GetRepository<ServiceProvider>()
                .Entities.FirstOrDefaultAsync(p => p.ProviderId == currentUserId && p.DeletedTime == null);

            if (provider == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Bạn không phải là nhà cung cấp dịch vụ.");

            var categoryExists = await _unitOfWork.GetRepository<ServiceCategory>().Entities
                .AnyAsync(c => c.Id == model.CategoryId && !c.DeletedTime.HasValue);
            if (!categoryExists)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service category not found.");

            var nameExists = await _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>().Entities
                .AnyAsync(s => s.ServiceName.ToLower() == model.ServiceName.ToLower() && !s.DeletedTime.HasValue);
            if (nameExists)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Service name already exists.");

            var entity = _mapper.Map<BeautySpa.Contract.Repositories.Entity.Service>(model);
            entity.ProviderId = provider.Id;
            entity.Id = Guid.NewGuid();
            entity.CreatedBy = CurrentUserId;
            entity.CreatedTime = CoreHelper.SystemTimeNow;
            entity.LastUpdatedBy = CurrentUserId;
            entity.LastUpdatedTime = entity.CreatedTime;

            await _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>().InsertAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<Guid>.Success(entity.Id);
        }


        public async Task<BaseResponseModel<BasePaginatedList<GETServiceModelViews>>> GetAllAsync(int pageNumber, int pageSize, Guid? providerId = null)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");

            IQueryable<BeautySpa.Contract.Repositories.Entity.Service> query = _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>()
                .Entities
                .Include(x => x.ServiceCategory)
                .Where(x => x.DeletedTime == null);

            if (providerId.HasValue && providerId != Guid.Empty)
            {
                var provider = await _unitOfWork.GetRepository<ServiceProvider>()
                    .Entities.FirstOrDefaultAsync(p => p.ProviderId == providerId && p.DeletedTime == null);

                if (provider != null)
                {
                    query = query.Where(x => x.ProviderId == provider.Id);
                }
            }

            query = query.OrderByDescending(x => x.CreatedTime);

            var pagedQuery = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            var mappedItems = await pagedQuery.ProjectTo<GETServiceModelViews>(_mapper.ConfigurationProvider).ToListAsync();
            var totalCount = await query.CountAsync();

            var result = new BasePaginatedList<GETServiceModelViews>(mappedItems, totalCount, pageNumber, pageSize);
            return BaseResponseModel<BasePaginatedList<GETServiceModelViews>>.Success(result);
        }
        public async Task<BaseResponseModel<List<GETServiceModelViews>>> GetMyServicesAsync()
        {
            var currentUserId = Guid.Parse(CurrentUserId);

            var provider = await _unitOfWork.GetRepository<ServiceProvider>()
                .Entities.FirstOrDefaultAsync(p => p.ProviderId == currentUserId && p.DeletedTime == null);

            if (provider == null)
                throw new ErrorException(StatusCodes.Status403Forbidden, ErrorCode.UnAuthorized, "Bạn không phải là nhà cung cấp dịch vụ.");

            var services = await _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>()
                .Entities
                .Include(s => s.ServiceCategory)
                .Where(s => s.ProviderId == provider.Id && s.DeletedTime == null)
                .OrderByDescending(s => s.CreatedTime)
                .ToListAsync();

            var result = _mapper.Map<List<GETServiceModelViews>>(services);
            return BaseResponseModel<List<GETServiceModelViews>>.Success(result);
        }

        public async Task<BaseResponseModel<List<GETServiceModelViews>>> GetByProviderIdAsync(Guid providerId)
        {
            if (providerId == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "ProviderId không hợp lệ.");

            var provider = await _unitOfWork.GetRepository<ServiceProvider>()
                .Entities.FirstOrDefaultAsync(x => x.ProviderId == providerId && x.DeletedTime == null);

            if (provider == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Nhà cung cấp không tồn tại.");

            var services = await _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>()
                .Entities
                .Include(s => s.ServiceCategory)
                .Where(s => s.ProviderId == provider.Id && s.DeletedTime == null)
                .OrderByDescending(s => s.CreatedTime)
                .ToListAsync();

            var result = _mapper.Map<List<GETServiceModelViews>>(services);
            return BaseResponseModel<List<GETServiceModelViews>>.Success(result);
        }

        public async Task<BaseResponseModel<GETServiceModelViews>> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid service ID.");

            var entity = await _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>().Entities
                .Include(s => s.ServiceCategory)
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service not found.");

            return BaseResponseModel<GETServiceModelViews>.Success(_mapper.Map<GETServiceModelViews>(entity));
        }

        public async Task<BaseResponseModel<string>> UpdateAsync(PUTServiceModelViews model)
        {
            var categoryExists = await _unitOfWork.GetRepository<ServiceCategory>().Entities
                .AnyAsync(c => c.Id == model.CategoryId && !c.DeletedTime.HasValue);
            if (!categoryExists)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service category not found.");

            if (model.Id == Guid.Empty || string.IsNullOrWhiteSpace(model.ServiceName))
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid input.");

            var repo = _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>();
            var entity = await repo.Entities.FirstOrDefaultAsync(x => x.Id == model.Id && x.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service not found.");

            var nameExists = await repo.Entities.AnyAsync(x => x.ServiceName.ToLower() == model.ServiceName.ToLower() && x.Id != model.Id && x.DeletedTime == null);
            if (nameExists)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Service name already exists.");

            var currentUserId = Guid.Parse(CurrentUserId);
            var provider = await _unitOfWork.GetRepository<ServiceProvider>()
                .Entities.FirstOrDefaultAsync(p => p.Id == entity.ProviderId && p.ProviderId == currentUserId && p.DeletedTime == null);

            if (provider == null)
                throw new ErrorException(StatusCodes.Status403Forbidden, ErrorCode.UnAuthorized, "You don't have permission to update this service");


            _mapper.Map(model, entity);
            entity.LastUpdatedBy = CurrentUserId;
            entity.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Service updated successfully.");
        }

        public async Task<BaseResponseModel<string>> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid service ID.");

            var repo = _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>();
            var entity = await repo.Entities.FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service not found.");

            entity.DeletedTime = CoreHelper.SystemTimeNow;
            entity.DeletedBy = CurrentUserId;

            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Service deleted successfully.");
        }
    }
}
