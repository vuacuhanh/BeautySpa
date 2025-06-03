using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.LocationModelViews;
using BeautySpa.Services.Validations.LocationValidator;
using BeautySpa.Services.Validations.SpaBranchLocationModelViewValidator;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
        public SpaBranchLocationService(IUnitOfWork unitOfWork, IMapper mapper, IEsgooService esgoo, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _esgoo = esgoo;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BaseResponseModel<Guid>> CreateAsync(POSTSpaBranchLocationModelView model)
        {
            await new POSTSpaBranchLocationModelViewValidator().ValidateAndThrowAsync(model);

            // ✅ Tự động lấy ProviderId từ token
            var currentUserId = Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
            var userGuid = Guid.Parse(currentUserId);

            var serviceProvider = await _unitOfWork.GetRepository<ServiceProvider>()
                .Entities
                .FirstOrDefaultAsync(x => x.ProviderId == userGuid && x.DeletedTime == null);

            if (serviceProvider == null)
                throw new ErrorException(StatusCodes.Status403Forbidden, ErrorCode.UnAuthenticated, "Provider not found.");

            var branch = _mapper.Map<SpaBranchLocation>(model);

            var province = await _esgoo.GetProvinceByIdAsync(model.ProvinceId!)
             ?? throw new ErrorException(404, ErrorCode.NotFound, "Province not found");

            var district = await _esgoo.GetDistrictByIdAsync(model.DistrictId!, model.ProvinceId!)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "District not found");

            branch.ProvinceName = province.name;
            branch.DistrictName = district.name;
            branch.Id = Guid.NewGuid();
            branch.ServiceProviderId = serviceProvider.Id; 
            branch.CreatedTime = CoreHelper.SystemTimeNow;
            branch.CreatedBy = currentUserId;

            await _unitOfWork.GetRepository<SpaBranchLocation>().InsertAsync(branch);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<Guid>.Success(branch.Id);
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
                .Entities
                .Include(x => x.WorkingHours)
                .Where(x => x.DeletedTime == null)
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedTime);

            BasePaginatedList<SpaBranchLocation> paged = await _unitOfWork.GetRepository<SpaBranchLocation>().GetPagging(query, pageNumber, pageSize);
            List<GETSpaBranchLocationModelView> result = _mapper.Map<List<GETSpaBranchLocationModelView>>(paged.Items);

            return BaseResponseModel<BasePaginatedList<GETSpaBranchLocationModelView>>.Success(new BasePaginatedList<GETSpaBranchLocationModelView>(result, paged.TotalItems, pageNumber, pageSize));
        }

        public async Task<BaseResponseModel<List<GETSpaBranchLocationModelView>>> GetByProviderAsync(Guid providerId)
        {
            //  Tìm ServiceProvider theo ProviderId (là userId)
            var serviceProvider = await _unitOfWork.GetRepository<ServiceProvider>()
                .Entities.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ProviderId == providerId && x.DeletedTime == null);

            if (serviceProvider == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Provider not found.");

            // Truy vấn nhánh theo ServiceProviderId (internal GUID)
            IQueryable<SpaBranchLocation> query = _unitOfWork.GetRepository<SpaBranchLocation>()
                .Entities.AsNoTracking()
                .Where(x => x.ServiceProviderId == providerId && x.DeletedTime == null);

            List<SpaBranchLocation> raw = await query.ToListAsync();
            List<GETSpaBranchLocationModelView> result = _mapper.Map<List<GETSpaBranchLocationModelView>>(raw);

            return BaseResponseModel<List<GETSpaBranchLocationModelView>>.Success(result);
        }
    }
}