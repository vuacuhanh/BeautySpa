using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.RankModelViews;
using BeautySpa.Services.Validations.RankValidator;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Services.Service
{
    public class RankService : IRankService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public RankService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public async Task<BaseResponseModel<BasePaginatedList<GETRankModelView>>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and size must be greater than 0");

            IQueryable<Rank> query = _unitOfWork.GetRepository<Rank>().Entities
                .Where(x => x.DeletedTime == null)
                .OrderByDescending(x => x.MinPoints);

            var totalCount = await query.CountAsync();
            var data = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var result = _mapper.Map<List<GETRankModelView>>(data);

            return BaseResponseModel<BasePaginatedList<GETRankModelView>>.Success(
                new BasePaginatedList<GETRankModelView>(result, totalCount, pageNumber, pageSize)
            );
        }

        public async Task<BaseResponseModel<GETRankModelView>> GetByIdAsync(Guid id)
        {
            IQueryable<Rank> query = _unitOfWork.GetRepository<Rank>().Entities
                .Where(x => x.Id == id && x.DeletedTime == null);

            var entity = await query.FirstOrDefaultAsync();
            if (entity == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Rank not found");

            return BaseResponseModel<GETRankModelView>.Success(_mapper.Map<GETRankModelView>(entity));
        }

        public async Task<BaseResponseModel<Guid>> CreateAsync(POSTRankModelView model)
        {
            await new POSTRankModelViewValidator().ValidateAndThrowAsync(model);

            var entity = _mapper.Map<Rank>(model);
            entity.Id = Guid.NewGuid();
            entity.CreatedBy = CurrentUserId;
            entity.CreatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Rank>().InsertAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<Guid>.Success(entity.Id);
        }

        public async Task<BaseResponseModel<string>> UpdateAsync(PUTRankModelView model)
        {
            await new PUTRankModelViewValidator().ValidateAndThrowAsync(model);

            IQueryable<Rank> query = _unitOfWork.GetRepository<Rank>().Entities
                .Where(x => x.Id == model.Id && x.DeletedTime == null);

            var entity = await query.FirstOrDefaultAsync();
            if (entity == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Rank not found");

            _mapper.Map(model, entity);
            entity.LastUpdatedTime = CoreHelper.SystemTimeNow;
            entity.LastUpdatedBy = CurrentUserId;

            await _unitOfWork.GetRepository<Rank>().UpdateAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Rank updated");
        }

        public async Task<BaseResponseModel<string>> DeleteAsync(Guid id)
        {
            IQueryable<Rank> query = _unitOfWork.GetRepository<Rank>().Entities
                .Where(x => x.Id == id && x.DeletedTime == null);

            var entity = await query.FirstOrDefaultAsync();
            if (entity == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Rank not found");

            entity.DeletedTime = CoreHelper.SystemTimeNow;
            entity.DeletedBy = CurrentUserId;

            await _unitOfWork.GetRepository<Rank>().UpdateAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Rank deleted");
        }
    }
}
