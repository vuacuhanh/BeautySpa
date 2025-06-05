using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.PromotionModelViews;
using BeautySpa.Services.Validations.PromotionValidator;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Security.Claims;

public class PromotionService : IPromotionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PromotionService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);

    public async Task<BaseResponseModel<BasePaginatedList<GETPromotionModelView>>> GetAllAsync(int pageNumber, int pageSize)
    {
        var currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
            throw new ErrorException(StatusCodes.Status401Unauthorized, ErrorCode.UnAuthorized, "Unauthorized");

        Guid providerId = Guid.Parse(currentUserId);

        IQueryable<Promotion> query = _unitOfWork.GetRepository<Promotion>()
            .Entities
            .AsNoTracking()
            .Where(x => x.ProviderId == providerId && x.DeletedTime == null)
            .OrderByDescending(x => x.CreatedTime);

        int totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        var mapped = _mapper.Map<List<GETPromotionModelView>>(items);
        var result = new BasePaginatedList<GETPromotionModelView>(mapped, totalCount, pageNumber, pageSize);

        return BaseResponseModel<BasePaginatedList<GETPromotionModelView>>.Success(result);
    }

    public async Task<BaseResponseModel<GETPromotionModelView>> GetByIdAsync(Guid id)
    {
        var entity = await _unitOfWork.GetRepository<Promotion>().Entities
            .Include(p => p.Provider)
            .FirstOrDefaultAsync(p => p.Id == id && p.DeletedTime == null);

        if (entity == null)
            throw new ErrorException(404, ErrorCode.NotFound, "Promotion not found");

        var result = _mapper.Map<GETPromotionModelView>(entity);
        return BaseResponseModel<GETPromotionModelView>.Success(result);
    }

    public async Task<BaseResponseModel<string>> CreateAsync(POSTPromotionModelView model)
    {
        await new POSTPromotionValidator().ValidateAndThrowAsync(model);

        var entity = _mapper.Map<Promotion>(model);
        entity.Id = Guid.NewGuid();
        entity.ProviderId = Guid.Parse(CurrentUserId);
        entity.CreatedBy = CurrentUserId;
        entity.CreatedTime = CoreHelper.SystemTimeNow;

        await _unitOfWork.GetRepository<Promotion>().InsertAsync(entity);
        await _unitOfWork.SaveAsync();

        return BaseResponseModel<string>.Success("Promotion created successfully");
    }

    public async Task<BaseResponseModel<List<GETPromotionModelView>>> GetByProviderIdAsync(Guid providerId)
    {
        var promotions = await _unitOfWork.GetRepository<Promotion>().Entities
            .Include(p => p.Provider)
            .Where(p => p.ProviderId == providerId && p.DeletedTime == null)
            .OrderByDescending(p => p.CreatedTime)
            .ToListAsync();

        var result = _mapper.Map<List<GETPromotionModelView>>(promotions);
        return BaseResponseModel<List<GETPromotionModelView>>.Success(result);
    }

    public async Task<BaseResponseModel<string>> UpdateAsync(PUTPromotionModelView model)
    {
        await new PUTPromotionValidator().ValidateAndThrowAsync(model);

        var repo = _unitOfWork.GetRepository<Promotion>();
        var entity = await repo.Entities.FirstOrDefaultAsync(p => p.Id == model.Id && p.DeletedTime == null);
        if (entity == null)
            throw new ErrorException(404, ErrorCode.NotFound, "Promotion not found");

        var currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId) || entity.ProviderId != Guid.Parse(currentUserId))
            throw new ErrorException(StatusCodes.Status403Forbidden, ErrorCode.UnAuthorized, "Access denied");

        _mapper.Map(model, entity);
        entity.LastUpdatedTime = CoreHelper.SystemTimeNow;
        entity.LastUpdatedBy = CurrentUserId;

        await repo.UpdateAsync(entity);
        await _unitOfWork.SaveAsync();

        return BaseResponseModel<string>.Success("Promotion updated successfully");
    }

    public async Task<BaseResponseModel<string>> DeleteAsync(Guid id)
    {
        var repo = _unitOfWork.GetRepository<Promotion>();
        var entity = await repo.Entities.FirstOrDefaultAsync(p => p.Id == id && p.DeletedTime == null);
        if (entity == null)
            throw new ErrorException(404, ErrorCode.NotFound, "Promotion not found");

        entity.DeletedTime = CoreHelper.SystemTimeNow;
        entity.DeletedBy = CurrentUserId;

        await repo.UpdateAsync(entity);
        await _unitOfWork.SaveAsync();

        return BaseResponseModel<string>.Success("Promotion deleted successfully");
    }
    public async Task<BaseResponseModel<string>> DeleteHardAsync(Guid id)
    {
        var repo = _unitOfWork.GetRepository<Promotion>();
        var entity = await repo.Entities.FirstOrDefaultAsync(p => p.Id == id);

        if (entity == null)
            throw new ErrorException(404, ErrorCode.NotFound, "Promotion not found");

        var currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId) || entity.ProviderId != Guid.Parse(currentUserId))
            throw new ErrorException(StatusCodes.Status403Forbidden, ErrorCode.UnAuthorized, "Access denied");

        await repo.DeleteAsync(entity.Id); // Xoá cứng
        await _unitOfWork.SaveAsync();

        return BaseResponseModel<string>.Success("Promotion permanently deleted");
    }
}
