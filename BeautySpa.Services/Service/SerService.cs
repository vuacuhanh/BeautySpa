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

            var providerExists = await _unitOfWork.GetRepository<ApplicationUsers>().Entities
                .AnyAsync(p => p.Id == model.ProviderId && !p.DeletedTime.HasValue);
            if (!providerExists)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Provider not found.");

            var categoryExists = await _unitOfWork.GetRepository<ServiceCategory>().Entities
                .AnyAsync(c => c.Id == model.CategoryId && !c.DeletedTime.HasValue);
            if (!categoryExists)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service category not found.");

            var nameExists = await _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>().Entities
                .AnyAsync(s => s.ServiceName.ToLower() == model.ServiceName.ToLower() && !s.DeletedTime.HasValue);
            if (nameExists)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Service name already exists.");

            var entity = _mapper.Map<BeautySpa.Contract.Repositories.Entity.Service>(model);
            entity.Id = Guid.NewGuid();
            entity.CreatedBy = CurrentUserId;
            entity.CreatedTime = CoreHelper.SystemTimeNow;
            entity.LastUpdatedBy = CurrentUserId;
            entity.LastUpdatedTime = entity.CreatedTime;

            entity.ServiceImages = model.Images.Select(img => new ServiceImage
            {
                Id = Guid.NewGuid(),
                ImageUrl = img.ImageUrl,
                IsPrimary = img.IsPrimary,
                CreatedBy = CurrentUserId,
                CreatedTime = CoreHelper.SystemTimeNow,
                LastUpdatedBy = CurrentUserId,
                LastUpdatedTime = CoreHelper.SystemTimeNow
            }).ToList();

            await _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>().InsertAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<Guid>.Success(entity.Id);
        }

        public async Task<BaseResponseModel<BasePaginatedList<GETServiceModelViews>>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");

            IQueryable<BeautySpa.Contract.Repositories.Entity.Service> query = _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>()
                .Entities
                .Where(x => x.DeletedTime == null)
                .Include(x => x.ServiceImages)
                .OrderByDescending(x => x.CreatedTime);

            var count = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var mapped = _mapper.Map<List<GETServiceModelViews>>(items);

            var result = new BasePaginatedList<GETServiceModelViews>(mapped, count, pageNumber, pageSize);
            return BaseResponseModel<BasePaginatedList<GETServiceModelViews>>.Success(result);
        }
    

        public async Task<BaseResponseModel<GETServiceModelViews>> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid service ID.");

            var entity = await _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>().Entities
                .Include(x => x.ServiceImages)
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service not found.");

            return BaseResponseModel<GETServiceModelViews>.Success(_mapper.Map<GETServiceModelViews>(entity));
        }

        public async Task<BaseResponseModel<string>> UpdateAsync(PUTServiceModelViews model)
        {
            if (model.Id == Guid.Empty || string.IsNullOrWhiteSpace(model.ServiceName))
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid input.");

            var repo = _unitOfWork.GetRepository<BeautySpa.Contract.Repositories.Entity.Service>();
            var entity = await repo.Entities.Include(x => x.ServiceImages)
                .FirstOrDefaultAsync(x => x.Id == model.Id && x.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service not found.");

            var nameExists = await repo.Entities.AnyAsync(x => x.ServiceName.ToLower() == model.ServiceName.ToLower() && x.Id != model.Id && x.DeletedTime == null);
            if (nameExists)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Service name already exists.");

            _mapper.Map(model, entity);
            entity.LastUpdatedBy = CurrentUserId;
            entity.LastUpdatedTime = CoreHelper.SystemTimeNow;

            entity.ServiceImages.Clear();
            entity.ServiceImages = model.Images.Select(img => new ServiceImage
            {
                Id = Guid.NewGuid(),
                ImageUrl = img.ImageUrl,
                IsPrimary = img.IsPrimary,
                CreatedBy = CurrentUserId,
                CreatedTime = CoreHelper.SystemTimeNow,
                LastUpdatedBy = CurrentUserId,
                LastUpdatedTime = CoreHelper.SystemTimeNow
            }).ToList();

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
