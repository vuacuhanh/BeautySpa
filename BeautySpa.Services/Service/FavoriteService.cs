using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.MemberShipModelViews.FavoriteModelViews;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Services.Service
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public FavoriteService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public async Task<BaseResponseModel<string>> LikeOrUnlikeAsync(Guid providerId)
        {
            Guid customerId = Guid.Parse(CurrentUserId); // ✅ Lấy từ token

            IQueryable<Favorite> query = _unitOfWork.GetRepository<Favorite>().Entities
                .IgnoreQueryFilters()
                .Where(f => f.CustomerId == customerId && f.ProviderId == providerId);

            Favorite? existing = await query.FirstOrDefaultAsync();

            if (existing == null)
            {
                var favorite = new Favorite
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    ProviderId = providerId,
                    CreatedTime = CoreHelper.SystemTimeNow,
                    CreatedBy = CurrentUserId
                };

                await _unitOfWork.GetRepository<Favorite>().InsertAsync(favorite);
                await _unitOfWork.SaveAsync();

                return BaseResponseModel<string>.Success("Like Success");
            }

            if (existing.DeletedTime == null)
            {
                existing.DeletedTime = CoreHelper.SystemTimeNow;
                existing.DeletedBy = CurrentUserId;

                await _unitOfWork.GetRepository<Favorite>().UpdateAsync(existing);
                await _unitOfWork.SaveAsync();

                return BaseResponseModel<string>.Success("Unlike Success");
            }

            existing.DeletedTime = null;
            existing.DeletedBy = null;
            existing.LastUpdatedTime = CoreHelper.SystemTimeNow;
            existing.LastUpdatedBy = CurrentUserId;

            await _unitOfWork.GetRepository<Favorite>().UpdateAsync(existing);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Relike Success");
        }

        public async Task<BaseResponseModel<bool>> IsFavoriteAsync(Guid providerId)
        {
            Guid customerId = Guid.Parse(CurrentUserId); // ✅ Lấy từ token

            IQueryable<Favorite> query = _unitOfWork.GetRepository<Favorite>().Entities
                .Where(f => f.CustomerId == customerId && f.ProviderId == providerId && f.DeletedTime == null);

            bool exists = await query.AnyAsync();
            return BaseResponseModel<bool>.Success(exists);
        }

        public async Task<BaseResponseModel<List<GETFavoriteModelViews>>> GetFavoritesByProviderAsync(Guid providerId)
        {
            IQueryable<Favorite> query = _unitOfWork.GetRepository<Favorite>().Entities
                .Where(f => f.ProviderId == providerId && f.DeletedTime == null)
                .OrderByDescending(f => f.CreatedTime);

            List<Favorite> list = await query.ToListAsync();
            var result = _mapper.Map<List<GETFavoriteModelViews>>(list);

            return BaseResponseModel<List<GETFavoriteModelViews>>.Success(result);
        }
    }
}
