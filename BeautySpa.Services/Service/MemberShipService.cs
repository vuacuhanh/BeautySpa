using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.MemberShipModelViews;
using BeautySpa.Services.Validations.MemberShipValidator;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Services.Service
{
    public class MemberShipService : IMemberShipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public MemberShipService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public async Task<BaseResponseModel<BasePaginatedList<GETMemberShipModelView>>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and size must be greater than 0");

            IQueryable<MemberShip> query = _unitOfWork.GetRepository<MemberShip>().Entities
                .Where(x => x.DeletedTime == null)
                .Include(x => x.Rank)
                .Include(x => x.User)
                .OrderByDescending(x => x.AccumulatedPoints);

            var totalCount = await query.CountAsync();
            var data = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var result = _mapper.Map<List<GETMemberShipModelView>>(data);

            return BaseResponseModel<BasePaginatedList<GETMemberShipModelView>>.Success(
                new BasePaginatedList<GETMemberShipModelView>(result, totalCount, pageNumber, pageSize)
            );
        }

        public async Task<BaseResponseModel<GETMemberShipModelView>> GetByUserIdAsync(Guid userId)
        {
            IQueryable<MemberShip> query = _unitOfWork.GetRepository<MemberShip>().Entities
                .Include(x => x.Rank)
                .Include(x => x.User)
                .Where(x => x.UserId == userId && x.DeletedTime == null);

            var entity = await query.FirstOrDefaultAsync();
            if (entity == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Membership not found");

            return BaseResponseModel<GETMemberShipModelView>.Success(_mapper.Map<GETMemberShipModelView>(entity));
        }

        public async Task<BaseResponseModel<Guid>> CreateAsync(POSTMemberShipModelView model)
        {
            await new POSTMemberShipModelViewValidator().ValidateAndThrowAsync(model);

            var entity = _mapper.Map<MemberShip>(model);
            entity.Id = Guid.NewGuid();
            entity.CreatedBy = CurrentUserId;
            entity.CreatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<MemberShip>().InsertAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<Guid>.Success(entity.Id);
        }

        public async Task<BaseResponseModel<string>> AddPointsAsync(PATCHMemberShipAddPointsModel model)
        {
            await new PATCHMemberShipAddPointsModelValidator().ValidateAndThrowAsync(model);

            IQueryable<MemberShip> query = _unitOfWork.GetRepository<MemberShip>().Entities
                .Where(x => x.Id == model.Id && x.DeletedTime == null);

            var entity = await query.FirstOrDefaultAsync();
            if (entity == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Membership not found");

            entity.AccumulatedPoints += model.Points;

            IQueryable<Rank> rankQuery = _unitOfWork.GetRepository<Rank>().Entities
                .Where(r => r.DeletedTime == null)
                .OrderByDescending(r => r.MinPoints);

            var ranks = await rankQuery.ToListAsync();
            var matchedRank = ranks.FirstOrDefault(r => entity.AccumulatedPoints >= r.MinPoints);
            if (matchedRank != null)
            {
                entity.RankId = matchedRank.Id;
            }

            entity.LastUpdatedBy = CurrentUserId;
            entity.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<MemberShip>().UpdateAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Points added and rank updated");
        }

        public async Task<BaseResponseModel<string>> DeleteAsync(Guid id)
        {
            IQueryable<MemberShip> query = _unitOfWork.GetRepository<MemberShip>().Entities
                .Where(x => x.Id == id && x.DeletedTime == null);

            var entity = await query.FirstOrDefaultAsync();
            if (entity == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Membership not found");

            entity.DeletedBy = CurrentUserId;
            entity.DeletedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<MemberShip>().UpdateAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Membership deleted");
        }
    }
}
