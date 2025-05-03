// ... (các using giữ nguyên)

using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.PromotionModelViews;
using BeautySpa.Services.Validations.PromotionValidator;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Services.Service
{
    public class PromotionService : IPromotionService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public PromotionService(IUnitOfWork uow, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _uow = uow;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<BaseResponseModel<Guid>> CreateAsync(POSTPromotionModelViews model)
        {
            var validator = new POSTPromotionModelViewValidator();
            var result = await validator.ValidateAsync(model);
            if (!result.IsValid)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput,
                    string.Join("; ", result.Errors.Select(e => e.ErrorMessage)));

            var promo = _mapper.Map<Promotion>(model);
            promo.Id = Guid.NewGuid();
            promo.CreatedTime = CoreHelper.SystemTimeNow;
            promo.CreatedBy = CurrentUserId;

            await _uow.GetRepository<Promotion>().InsertAsync(promo);
            await _uow.SaveAsync();
            return BaseResponseModel<Guid>.Success(promo.Id);
        }

        public async Task<BaseResponseModel<string>> UpdateAsync(PUTPromotionModelViews model)
        {
            var validator = new PUTPromotionModelViewValidator();
            var result = await validator.ValidateAsync(model);
            if (!result.IsValid)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput,
                    string.Join("; ", result.Errors.Select(e => e.ErrorMessage)));

            var repo = _uow.GetRepository<Promotion>();
            var entity = await repo.Entities.FirstOrDefaultAsync(x => x.Id == model.Id && x.DeletedTime == null);
            if (entity == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Promotion Not Found");

            _mapper.Map(model, entity);
            entity.LastUpdatedTime = CoreHelper.SystemTimeNow;
            entity.LastUpdatedBy = CurrentUserId;

            await repo.UpdateAsync(entity);
            await _uow.SaveAsync();
            return BaseResponseModel<string>.Success("Update Successful");
        }

        public async Task<BaseResponseModel<string>> DeleteAsync(Guid id)
        {
            var repo = _uow.GetRepository<Promotion>();
            var entity = await repo.Entities.FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null);
            if (entity == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Promotion Not Found");

            entity.DeletedTime = CoreHelper.SystemTimeNow;
            entity.DeletedBy = CurrentUserId;

            await repo.UpdateAsync(entity);
            await _uow.SaveAsync();
            return BaseResponseModel<string>.Success("Delete Successful");
        }

        public async Task<BaseResponseModel<GETPromotionModelViews>> GetByIdAsync(Guid id)
        {
            var entity = await _uow.GetRepository<Promotion>().Entities
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null);
            if (entity == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Promotion Not Found");

            return BaseResponseModel<GETPromotionModelViews>.Success(_mapper.Map<GETPromotionModelViews>(entity));
        }

        public async Task<BaseResponseModel<BasePaginatedList<GETPromotionModelViews>>> GetAllAsync(int page, int size)
        {
            if (page <= 0 || size <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");

            IQueryable<Promotion> query = _uow.GetRepository<Promotion>().Entities
                .AsNoTracking()
                .Where(x => x.DeletedTime == null)
                .OrderByDescending(x => x.CreatedTime);

            var count = await query.CountAsync();
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();
            var result = _mapper.Map<List<GETPromotionModelViews>>(items);

            return BaseResponseModel<BasePaginatedList<GETPromotionModelViews>>.Success(
                new BasePaginatedList<GETPromotionModelViews>(result, count, page, size));
        }
    }
}
