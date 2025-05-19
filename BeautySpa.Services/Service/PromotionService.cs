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

public class PromotionService : IPromotionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;

    public PromotionService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _contextAccessor = contextAccessor;
    }

    private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

    public async Task<BaseResponseModel<BasePaginatedList<GETPromotionModelView>>> GetAllAsync(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0)
            throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Page number and page size must be greater than 0.");

        IQueryable<Promotion> query = _unitOfWork.GetRepository<Promotion>().Entities
            .Include(p => p.Provider)
            .Where(p => !p.DeletedTime.HasValue)
            .OrderByDescending(p => p.CreatedTime);

        var page = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = new BasePaginatedList<GETPromotionModelView>(
            _mapper.Map<List<GETPromotionModelView>>(page),
            await query.CountAsync(),
            pageNumber,
            pageSize
        );

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

    public async Task<BaseResponseModel<string>> UpdateAsync(PUTPromotionModelView model)
    {
        await new PUTPromotionValidator().ValidateAndThrowAsync(model);

        var repo = _unitOfWork.GetRepository<Promotion>();
        var entity = await repo.Entities.FirstOrDefaultAsync(p => p.Id == model.Id && p.DeletedTime == null);
        if (entity == null)
            throw new ErrorException(404, ErrorCode.NotFound, "Promotion not found");

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
}
