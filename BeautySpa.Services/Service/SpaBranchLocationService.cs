using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.LocationModelViews;
using BeautySpa.Services.Validations.LocationValidator;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Services.Service
{
    public class SpaBranchLocationService : ISpaBranchLocationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEsgooService _esgoo;

        public SpaBranchLocationService(IUnitOfWork unitOfWork, IMapper mapper, IEsgooService esgoo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _esgoo = esgoo;
        }

        public async Task<BaseResponseModel<Guid>> CreateAsync(POSTSpaBranchLocationModelView model)
        {
            await new POSTSpaBranchLocationValidator().ValidateAndThrowAsync(model);

            SpaBranchLocation entity = _mapper.Map<SpaBranchLocation>(model);

            ProvinceModel? province = await _esgoo.GetProvinceByIdAsync(model.ProvinceId!)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Province not found");

            DistrictModel? district = await _esgoo.GetDistrictByIdAsync(model.DistrictId!, model.ProvinceId!)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "District not found");

            entity.ProvinceName = province.name;
            entity.DistrictName = district.name;

            await _unitOfWork.GetRepository<SpaBranchLocation>().InsertAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<Guid>.Success(entity.Id);
        }

        public async Task<BaseResponseModel<string>> UpdateAsync(PUTSpaBranchLocationModelView model)
        {
            await new PUTSpaBranchLocationValidator().ValidateAndThrowAsync(model);

            SpaBranchLocation? entity = await _unitOfWork.GetRepository<SpaBranchLocation>().GetByIdAsync(model.Id)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Branch not found");

            _mapper.Map(model, entity);

            ProvinceModel? province = await _esgoo.GetProvinceByIdAsync(model.ProvinceId!)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Province not found");

            DistrictModel? district = await _esgoo.GetDistrictByIdAsync(model.DistrictId!, model.ProvinceId!)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "District not found");

            entity.ProvinceName = province.name;
            entity.DistrictName = district.name;

            await _unitOfWork.SaveAsync();
            return BaseResponseModel<string>.Success("Updated");
        }

        public async Task<BaseResponseModel<string>> DeleteAsync(Guid id)
        {
            SpaBranchLocation? entity = await _unitOfWork.GetRepository<SpaBranchLocation>().GetByIdAsync(id)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Branch not found");

            entity.DeletedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.SaveAsync();
            return BaseResponseModel<string>.Success("Deleted");
        }

        public async Task<BaseResponseModel<GETSpaBranchLocationModelView>> GetByIdAsync(Guid id)
        {
            SpaBranchLocation? entity = await _unitOfWork.GetRepository<SpaBranchLocation>()
                .Entities.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Branch not found");

            GETSpaBranchLocationModelView result = _mapper.Map<GETSpaBranchLocationModelView>(entity);
            return BaseResponseModel<GETSpaBranchLocationModelView>.Success(result);
        }

        public async Task<BaseResponseModel<BasePaginatedList<GETSpaBranchLocationModelView>>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");
            IQueryable<SpaBranchLocation> query = _unitOfWork.GetRepository<SpaBranchLocation>()
                .Entities.AsNoTracking()
                .Where(x => x.DeletedTime == null)
                .OrderByDescending(x => x.CreatedTime);

            BasePaginatedList<SpaBranchLocation> paged = await _unitOfWork.GetRepository<SpaBranchLocation>().GetPagging(query, pageNumber, pageSize);
            List<GETSpaBranchLocationModelView> result = _mapper.Map<List<GETSpaBranchLocationModelView>>(paged.Items);

            return BaseResponseModel<BasePaginatedList<GETSpaBranchLocationModelView>>.Success(new BasePaginatedList<GETSpaBranchLocationModelView>(result, paged.TotalItems, pageNumber, pageSize));
        }

        public async Task<BaseResponseModel<List<GETSpaBranchLocationModelView>>> GetByProviderAsync(Guid providerId)
        {
            IQueryable<SpaBranchLocation> query = _unitOfWork.GetRepository<SpaBranchLocation>()
                .Entities.AsNoTracking()
                .Where(x => x.ServiceProviderId == providerId && x.DeletedTime == null);

            List<SpaBranchLocation> raw = await query.ToListAsync();
            List<GETSpaBranchLocationModelView> result = _mapper.Map<List<GETSpaBranchLocationModelView>>(raw);

            return BaseResponseModel<List<GETSpaBranchLocationModelView>>.Success(result);
        }
    }
}