using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.SignalR;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.PromotionAdminModelView;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Services.Service
{
    public class PromotionAdminService : IPromotionAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHubContext<MessageHub> _hubContext;

        public PromotionAdminService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor contextAccessor,
            IHubContext<MessageHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _hubContext = hubContext;
        }

        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public async Task<BaseResponseModel<List<GETPromotionAdminModelView>>> GetAllAsync()
        {
            IQueryable<PromotionAdmin> query = _unitOfWork.GetRepository<PromotionAdmin>().Entities
                .Include(x => x.PromotionAdminRanks)
                .Where(x => x.DeletedTime == null)
                .OrderByDescending(x => x.CreatedTime);

            var data = await query.ToListAsync();
            var result = _mapper.Map<List<GETPromotionAdminModelView>>(data);

            return BaseResponseModel<List<GETPromotionAdminModelView>>.Success(result);
        }

        public async Task<BaseResponseModel<GETPromotionAdminModelView>> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ErrorException(400, ErrorCode.InvalidInput, "Invalid promotion id");

            IQueryable<PromotionAdmin> query = _unitOfWork.GetRepository<PromotionAdmin>().Entities
                .Include(x => x.PromotionAdminRanks)
                .Where(x => x.Id == id && x.DeletedTime == null);

            var entity = await query.FirstOrDefaultAsync();
            if (entity == null)
                throw new ErrorException(404, ErrorCode.NotFound, "Promotion not found");

            var result = _mapper.Map<GETPromotionAdminModelView>(entity);
            return BaseResponseModel<GETPromotionAdminModelView>.Success(result);
        }

        public async Task<BaseResponseModel<string>> CreateAsync(POSTPromotionAdminModelView model)
        {
            await new POSTPromotionAdminValidator().ValidateAndThrowAsync(model);

            var entity = _mapper.Map<PromotionAdmin>(model);
            entity.Id = Guid.NewGuid();
            entity.CreatedTime = CoreHelper.SystemTimeNow;
            entity.CreatedBy = CurrentUserId;
            entity.IsActive = true;

            foreach (var rankId in model.RankIds)
            {
                entity.PromotionAdminRanks.Add(new PromotionAdminRank
                {
                    Id = Guid.NewGuid(),
                    PromotionAdminId = entity.Id,
                    RankId = rankId
                });
            }

            await _unitOfWork.GetRepository<PromotionAdmin>().InsertAsync(entity);
            await _unitOfWork.SaveAsync();

            var result = _mapper.Map<GETPromotionAdminModelView>(entity);
            await _hubContext.Clients.All.SendAsync("ReceivePromotionAdmin", result);

            return BaseResponseModel<string>.Success("Tạo khuyến mãi hệ thống thành công");
        }

        public async Task<BaseResponseModel<string>> UpdateAsync(PUTPromotionAdminModelView model)
        {
            await new PUTPromotionAdminValidator().ValidateAndThrowAsync(model);

            var repo = _unitOfWork.GetRepository<PromotionAdmin>();
            var entity = await repo.Entities
                .Include(x => x.PromotionAdminRanks)
                .FirstOrDefaultAsync(x => x.Id == model.Id && x.DeletedTime == null);

            if (entity == null)
                throw new ErrorException(404, ErrorCode.NotFound, "Promotion không tồn tại");

            _mapper.Map(model, entity);
            entity.LastUpdatedTime = CoreHelper.SystemTimeNow;
            entity.LastUpdatedBy = CurrentUserId;

            entity.PromotionAdminRanks.Clear();
            foreach (var rankId in model.RankIds)
            {
                entity.PromotionAdminRanks.Add(new PromotionAdminRank
                {
                    Id = Guid.NewGuid(),
                    PromotionAdminId = entity.Id,
                    RankId = rankId
                });
            }

            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Cập nhật thành công");
        }
    }
}
