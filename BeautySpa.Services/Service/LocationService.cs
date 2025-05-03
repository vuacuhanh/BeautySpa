using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.LocationModelViews;
using BeautySpa.Services.Validations.LocationValidator;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace BeautySpa.Services.Service
{
    public class LocationService : ILocationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public LocationService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        // ========== BRANCH ==========
        public async Task<BaseResponseModel<List<GETBranchLocationModelView>>> GetAllBranchesAsync()
        {
            IQueryable<BranchLocationSpa> query = _unitOfWork.GetRepository<BranchLocationSpa>().Entities
                .Where(x => x.DeletedTime == null)
                .OrderByDescending(x => x.CreatedTime);

            var list = await query.ToListAsync();
            var result = _mapper.Map<List<GETBranchLocationModelView>>(list);

            return BaseResponseModel<List<GETBranchLocationModelView>>.Success(result);
        }

        public async Task<BaseResponseModel<GETBranchLocationModelView>> GetBranchByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.GetRepository<BranchLocationSpa>().Entities
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null);

            if (entity == null)
                throw new ErrorException(404, ErrorCode.NotFound, "Branch not found");

            return BaseResponseModel<GETBranchLocationModelView>.Success(_mapper.Map<GETBranchLocationModelView>(entity));
        }

        public async Task<BaseResponseModel<Guid>> CreateBranchAsync(POSTBranchLocationModelView model)
        {
            await new POSTBranchLocationValidator().ValidateAndThrowAsync(model);

            var entity = _mapper.Map<BranchLocationSpa>(model);
            entity.Id = Guid.NewGuid();
            entity.IsActive = true;
            entity.CreatedTime = CoreHelper.SystemTimeNow;
            entity.CreatedBy = CurrentUserId;

            await _unitOfWork.GetRepository<BranchLocationSpa>().InsertAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<Guid>.Success(entity.Id);
        }

        public async Task<BaseResponseModel<string>> UpdateBranchAsync(PUTBranchLocationModelView model)
        {
            await new PUTBranchLocationValidator().ValidateAndThrowAsync(model);

            var repo = _unitOfWork.GetRepository<BranchLocationSpa>();
            var entity = await repo.Entities.FirstOrDefaultAsync(x => x.Id == model.Id && x.DeletedTime == null);

            if (entity == null)
                throw new ErrorException(404, ErrorCode.NotFound, "Branch not found");

            entity.Name = model.Name;
            entity.PhoneNumber = model.PhoneNumber;
            entity.Email = model.Email;
            entity.Description = model.Description;
            entity.IsActive = model.IsActive;
            entity.LastUpdatedTime = CoreHelper.SystemTimeNow;
            entity.LastUpdatedBy = CurrentUserId;

            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Branch updated");
        }

        public async Task<BaseResponseModel<string>> DeleteBranchAsync(Guid id)
        {
            var repo = _unitOfWork.GetRepository<BranchLocationSpa>();
            var entity = await repo.Entities.FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null);

            if (entity == null)
                throw new ErrorException(404, ErrorCode.NotFound, "Branch not found");

            entity.DeletedTime = CoreHelper.SystemTimeNow;
            entity.DeletedBy = CurrentUserId;

            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Branch deleted");
        }

        // ========== LOCATION ==========
        public async Task<BaseResponseModel<List<GETLocationSpaModelView>>> GetAllLocationsAsync()
        {
            IQueryable<LocationSpa> query = _unitOfWork.GetRepository<LocationSpa>().Entities
                .Include(x => x.Branch)
                .Where(x => x.DeletedTime == null)
                .OrderByDescending(x => x.CreatedTime);

            var list = await query.ToListAsync();
            var result = _mapper.Map<List<GETLocationSpaModelView>>(list);

            return BaseResponseModel<List<GETLocationSpaModelView>>.Success(result);
        }

        public async Task<BaseResponseModel<GETLocationSpaModelView>> GetLocationByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.GetRepository<LocationSpa>().Entities
                .Include(x => x.Branch)
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null);

            if (entity == null)
                throw new ErrorException(404, ErrorCode.NotFound, "Location not found");

            return BaseResponseModel<GETLocationSpaModelView>.Success(_mapper.Map<GETLocationSpaModelView>(entity));
        }

        public async Task<BaseResponseModel<Guid>> CreateLocationAsync(POSTLocationSpaModelView model)
        {
            await new POSTLocationSpaValidator().ValidateAndThrowAsync(model);

            var entity = _mapper.Map<LocationSpa>(model);
            entity.Id = Guid.NewGuid();
            entity.IsActive = true;
            entity.CreatedTime = CoreHelper.SystemTimeNow;
            entity.CreatedBy = CurrentUserId;

            await _unitOfWork.GetRepository<LocationSpa>().InsertAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<Guid>.Success(entity.Id);
        }

        public async Task<BaseResponseModel<string>> UpdateLocationAsync(PUTLocationSpaModelView model)
        {
            await new PUTLocationSpaValidator().ValidateAndThrowAsync(model);

            var repo = _unitOfWork.GetRepository<LocationSpa>();
            var entity = await repo.Entities.FirstOrDefaultAsync(x => x.Id == model.Id && x.DeletedTime == null);

            if (entity == null)
                throw new ErrorException(404, ErrorCode.NotFound, "Location not found");

            _mapper.Map(model, entity);
            entity.LastUpdatedTime = CoreHelper.SystemTimeNow;
            entity.LastUpdatedBy = CurrentUserId;

            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Location updated");
        }

        public async Task<BaseResponseModel<string>> DeleteLocationAsync(Guid id)
        {
            var repo = _unitOfWork.GetRepository<LocationSpa>();
            var entity = await repo.Entities.FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null);

            if (entity == null)
                throw new ErrorException(404, ErrorCode.NotFound, "Location not found");

            entity.DeletedTime = CoreHelper.SystemTimeNow;
            entity.DeletedBy = CurrentUserId;

            await repo.UpdateAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Location deleted");
        }

        public async Task<BaseResponseModel<List<GETLocationSpaModelView>>> GetLocationsByBranchIdAsync(Guid branchId)
        {
            IQueryable<LocationSpa> query = _unitOfWork.GetRepository<LocationSpa>().Entities
                .Include(x => x.Branch)
                .Where(x => x.BranchId == branchId && x.DeletedTime == null && x.IsActive);

            var list = await query.ToListAsync();
            var result = _mapper.Map<List<GETLocationSpaModelView>>(list);

            return BaseResponseModel<List<GETLocationSpaModelView>>.Success(result);
        }
    }
}
