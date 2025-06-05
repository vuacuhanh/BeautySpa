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

    public async Task<BaseResponseModel<List<GETServicePromotionModelView>>> GetAllByCurrentUserAsync()
    {
        var userId = Guid.Parse(CurrentUserId);

        var provider = await _unitOfWork.GetRepository<ServiceProvider>()
            .Entities.FirstOrDefaultAsync(p => p.ProviderId == userId && p.DeletedTime == null);

        if (provider == null)
            throw new ErrorException(StatusCodes.Status403Forbidden, ErrorCode.UnAuthorized, "Bạn không phải nhà cung cấp.");

        var items = await _unitOfWork.GetRepository<ServicePromotion>()
            .Entities
            .Include(x => x.Service)
            .Where(x => x.DeletedTime == null && x.Service!.ProviderId == provider.Id)
            .OrderByDescending(x => x.CreatedTime)
            .ToListAsync();

        var result = _mapper.Map<List<GETServicePromotionModelView>>(items);
        return BaseResponseModel<List<GETServicePromotionModelView>>.Success(result);
    }

    public async Task<BaseResponseModel<GETServicePromotionModelView>> GetByIdAsync(Guid id)
    {
        var entity = await _unitOfWork.GetRepository<ServicePromotion>().Entities
            .Include(x => x.Service)
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null);

        if (entity == null)
            throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Flash sale not found");

        return BaseResponseModel<GETServicePromotionModelView>.Success(_mapper.Map<GETServicePromotionModelView>(entity));
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
        var entity = await repo.Entities.Include(x => x.Service).FirstOrDefaultAsync(x => x.Id == model.Id && x.DeletedTime == null);

        if (entity == null)
            throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Flash sale not found");

        var userId = Guid.Parse(CurrentUserId);
        var provider = await _unitOfWork.GetRepository<ServiceProvider>()
            .Entities.FirstOrDefaultAsync(p => p.Id == entity.Service!.ProviderId && p.ProviderId == userId && p.DeletedTime == null);

        if (provider == null)
            throw new ErrorException(StatusCodes.Status403Forbidden, ErrorCode.UnAuthorized, "Bạn không có quyền cập nhật flash sale này.");

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
        var entity = await repo.Entities.Include(x => x.Service).FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null);

        if (entity == null)
            throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Flash sale not found");

        var userId = Guid.Parse(CurrentUserId);
        var provider = await _unitOfWork.GetRepository<ServiceProvider>()
            .Entities.FirstOrDefaultAsync(p => p.Id == entity.Service!.ProviderId && p.ProviderId == userId && p.DeletedTime == null);

        if (provider == null)
            throw new ErrorException(StatusCodes.Status403Forbidden, ErrorCode.UnAuthorized, "Bạn không có quyền xóa flash sale này.");

        entity.DeletedTime = CoreHelper.SystemTimeNow;
        entity.DeletedBy = CurrentUserId;

        await repo.UpdateAsync(entity);
        await _unitOfWork.SaveAsync();

        return BaseResponseModel<string>.Success("Flash sale deleted successfully");
    }
}
