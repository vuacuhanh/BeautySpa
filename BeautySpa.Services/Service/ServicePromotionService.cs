using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.ServicePromotionModelView;
using BeautySpa.Services.Validations.ServicePromotionValidator;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Services.Service
{
    public class ServicePromotionService : IServicePromotionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;

        public ServicePromotionService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContext = httpContext;
        }

        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_httpContext);

        public async Task<BaseResponseModel<BasePaginatedList<GETServicePromotionModelView>>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and size must be greater than 0");

            IQueryable<ServicePromotion> query = _unitOfWork.GetRepository<ServicePromotion>().Entities
                .Where(x => x.DeletedTime == null)
                .Include(x => x.Service)
                .OrderByDescending(x => x.CreatedTime);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var result = _mapper.Map<List<GETServicePromotionModelView>>(items);

            return BaseResponseModel<BasePaginatedList<GETServicePromotionModelView>>.Success(
                new BasePaginatedList<GETServicePromotionModelView>(result, totalCount, pageNumber, pageSize)
            );
        }

        public async Task<BaseResponseModel<GETServicePromotionModelView>> GetByIdAsync(Guid id)
        {
            IQueryable<ServicePromotion> query = _unitOfWork.GetRepository<ServicePromotion>().Entities
                .Include(x => x.Service)
                .Where(x => x.Id == id && x.DeletedTime == null);

            var entity = await query.FirstOrDefaultAsync();
            if (entity == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Flash sale not found");

            var result = _mapper.Map<GETServicePromotionModelView>(entity);
            return BaseResponseModel<GETServicePromotionModelView>.Success(result);
        }

        public async Task<BaseResponseModel<string>> CreateAsync(POSTServicePromotionModelView model)
        {
            await new POSTServicePromotionValidator().ValidateAndThrowAsync(model);

            var entity = _mapper.Map<ServicePromotion>(model);
            entity.Id = Guid.NewGuid();
            entity.CreatedBy = CurrentUserId;
            entity.CreatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<ServicePromotion>().InsertAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Flash sale created successfully");
        }

        public async Task<BaseResponseModel<string>> UpdateAsync(PUTServicePromotionModelView model)
        {
            await new PUTServicePromotionValidator().ValidateAndThrowAsync(model);

            var repo = _unitOfWork.GetRepository<ServicePromotion>();
            var entity = await repo.Entities.FirstOrDefaultAsync(x => x.Id == model.Id && x.DeletedTime == null);
            if (entity == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Flash sale not found");

            _mapper.Map(model, entity);
            entity.LastUpdatedTime = CoreHelper.SystemTimeNow;
            entity.LastUpdatedBy = CurrentUserId;

            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Flash sale updated successfully");
        }

        public async Task<BaseResponseModel<string>> DeleteAsync(Guid id)
        {
            var repo = _unitOfWork.GetRepository<ServicePromotion>();
            var entity = await repo.Entities.FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null);
            if (entity == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Flash sale not found");

            entity.DeletedTime = CoreHelper.SystemTimeNow;
            entity.DeletedBy = CurrentUserId;

            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Flash sale deleted successfully");
        }
    }
}
