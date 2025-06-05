using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.ServiceProviderModelViews;
<<<<<<< HEAD
using BeautySpa.Services.Validations.ServiceProviderValidator;
=======
>>>>>>> 69164a2c523de3343cce845b080745ed3f4d99bd
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Services.Service
{
    public class ServiceProviderSer : IServiceProviders
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public ServiceProviderSer(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<BaseResponseModel<BasePaginatedList<GETServiceProviderModelViews>>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");

            IQueryable<ServiceProvider> query = _unitOfWork.GetRepository<ServiceProvider>()
                .Entities
                .AsNoTracking()
                .Include(sp => sp.ServiceImages)
                .Include(p => p.ServiceProviderCategories)
                .ThenInclude(spc => spc.ServiceCategory)
                .Where(x => x.DeletedTime == null)
                .OrderByDescending(x => x.CreatedTime);

            int totalCount = await query.CountAsync();
            List<ServiceProvider> items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mapped = _mapper.Map<List<GETServiceProviderModelViews>>(items);

            // Gán địa chỉ chi nhánh chính
            var branchRepo = _unitOfWork.GetRepository<SpaBranchLocation>();
            foreach (var item in mapped)
            {
                var mainBranch = await branchRepo.Entities.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ServiceProviderId == item.Id && x.BranchName == "Cơ sở chính" && x.DeletedTime == null);

                if (mainBranch != null)
                {
                    item.AddressDetail = mainBranch.Street;
                    item.ProvinceId = mainBranch.ProvinceId;
                    item.DistrictId = mainBranch.DistrictId;
                    item.ProvinceName = mainBranch.ProvinceName;
                    item.DistrictName = mainBranch.DistrictName;
                }
            }

            var result = new BasePaginatedList<GETServiceProviderModelViews>(mapped, totalCount, pageNumber, pageSize);
            return BaseResponseModel<BasePaginatedList<GETServiceProviderModelViews>>.Success(result);
        }


        public async Task<BaseResponseModel<GETServiceProviderModelViews>> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid ServiceProvider ID.");

            IQueryable<ServiceProvider> query = _unitOfWork.GetRepository<ServiceProvider>()
                .Entities
                .AsNoTracking()
                .Include(x => x.ServiceImages)
                .Include(p => p.ServiceProviderCategories)
                .ThenInclude(spc => spc.ServiceCategory);

            var entity = await query.FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service Provider not found.");

            var mapped = _mapper.Map<GETServiceProviderModelViews>(entity);

            // Gán địa chỉ từ chi nhánh chính
            var mainBranch = await _unitOfWork.GetRepository<SpaBranchLocation>()
                .Entities.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ServiceProviderId == entity.Id && x.BranchName == "Cơ sở chính" && x.DeletedTime == null);

            if (mainBranch != null)
            {
                mapped.AddressDetail = mainBranch.Street;
                mapped.ProvinceId = mainBranch.ProvinceId;
                mapped.DistrictId = mainBranch.DistrictId;
                mapped.ProvinceName = mainBranch.ProvinceName;
                mapped.DistrictName = mainBranch.DistrictName;
            }

            return BaseResponseModel<GETServiceProviderModelViews>.Success(mapped);
        }


        public async Task<BaseResponseModel<Guid>> CreateAsync(POSTServiceProviderModelViews model)
        {
            var validator = new POSTServiceProviderModelViewsValidator();
            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

            var phoneExists = await _unitOfWork.GetRepository<ServiceProvider>()
                .Entities.AnyAsync(x => x.PhoneNumber == model.PhoneNumber && x.DeletedTime == null);
            if (phoneExists)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Phone number is already in use.");

            var user = await _unitOfWork.GetRepository<ApplicationUsers>().GetByIdAsync(model.UserId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User not found.");

            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;
            await _unitOfWork.GetRepository<ApplicationUsers>().UpdateAsync(user);

            var entity = _mapper.Map<ServiceProvider>(model);
            entity.Id = Guid.NewGuid();
            entity.ProviderId = model.UserId;
            entity.CreatedBy = CurrentUserId;
            entity.CreatedTime = CoreHelper.SystemTimeNow;

            if (string.IsNullOrWhiteSpace(entity.ImageUrl))
            {
                var firstImage = await _unitOfWork.GetRepository<ServiceImage>().Entities
                    .Where(x => x.ServiceProviderId == entity.Id && x.DeletedTime == null)
                    .OrderBy(x => x.CreatedTime)
                    .FirstOrDefaultAsync();

                if (firstImage != null)
                {
                    entity.ImageUrl = firstImage.ImageUrl;
                }
            }

            await _unitOfWork.GetRepository<ServiceProvider>().InsertAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<Guid>.Success(entity.Id);
        }

        public async Task<BaseResponseModel<string>> UpdateAsync(PUTServiceProviderModelViews model)
        {
            var validator = new PUTServiceProviderModelViewsValidator();
            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput,
                    string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

            // Lấy ServiceProvider hiện tại
            var entity = await _unitOfWork.GetRepository<ServiceProvider>()
                .Entities
                .Include(sp => sp.ServiceProviderCategories)
                .FirstOrDefaultAsync(sp => sp.Id == model.Id && sp.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service Provider not found.");

            // Lấy thông tin người dùng hiện tại từ token
            var userId = Guid.Parse(CurrentUserId);
            var user = await _unitOfWork.GetRepository<ApplicationUsers>().GetByIdAsync(userId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "User not found.");

            // Cập nhật user info
            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;
            await _unitOfWork.GetRepository<ApplicationUsers>().UpdateAsync(user);

            // Cập nhật thông tin provider
            _mapper.Map(model, entity);
            entity.LastUpdatedTime = CoreHelper.SystemTimeNow;
            entity.LastUpdatedBy = CurrentUserId;

            await _unitOfWork.GetRepository<ServiceProvider>().UpdateAsync(entity);

            // Cập nhật danh mục dịch vụ
            var categoryRepo = _unitOfWork.GetRepository<ServiceProviderCategory>();
            var oldCategories = await categoryRepo.Entities
                .Where(c => c.ServiceProviderId == entity.Id)
                .ToListAsync();

            foreach (var cat in oldCategories)
            {
                await categoryRepo.DeleteAsync(cat.Id);
            }

            if (model.ServiceCategoryIds != null && model.ServiceCategoryIds.Any())
            {
                foreach (var categoryId in model.ServiceCategoryIds.Distinct())
                {
                    await categoryRepo.InsertAsync(new ServiceProviderCategory
                    {
                        Id = Guid.NewGuid(),
                        ServiceProviderId = entity.Id,
                        ServiceCategoryId = categoryId
                    });
                }
            }

            await _unitOfWork.SaveAsync();
            return BaseResponseModel<string>.Success("Service provider updated successfully.");
        }


        public async Task<BaseResponseModel<string>> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid ServiceProvider ID.");

            var entity = await _unitOfWork.GetRepository<ServiceProvider>().GetByIdAsync(id)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Service Provider not found.");

            if (entity.DeletedTime != null)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Service Provider already deleted.");

            entity.DeletedTime = CoreHelper.SystemTimeNow;
            entity.DeletedBy = CurrentUserId;

            await _unitOfWork.GetRepository<ServiceProvider>().UpdateAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Service provider deleted successfully.");
        }
        public async Task<BaseResponseModel<List<GETServiceProviderModelViews>>> GetByCategory(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid category ID.");

            var providers = await _unitOfWork.GetRepository<ServiceProvider>()
                .Entities
                .AsNoTracking()
                .Include(p => p.ServiceImages)
                .Include(p => p.ServiceProviderCategories)
                    .ThenInclude(spc => spc.ServiceCategory)
                .Where(p => p.ServiceProviderCategories.Any(spc => spc.ServiceCategoryId == categoryId)
                            && p.DeletedTime == null)
                .ToListAsync();

            var mapped = _mapper.Map<List<GETServiceProviderModelViews>>(providers);
            return BaseResponseModel<List<GETServiceProviderModelViews>>.Success(mapped);
        }
    }
}