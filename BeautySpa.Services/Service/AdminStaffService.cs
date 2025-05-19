using AutoMapper;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Core.Utils;
using BeautySpa.Core.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.StaffAdminModelViews;
using BeautySpa.Contract.Services.Interface;

public class AdminStaffService : IAdminStaff
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;
    private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

    public AdminStaffService(IUnitOfWork uow, IMapper mapper, IHttpContextAccessor contextAccessor)
    {
        _unitOfWork = uow;
        _mapper = mapper;
        _contextAccessor = contextAccessor;
    }

    public async Task<BaseResponseModel<Guid>> CreateAsync(POSTAdminStaffModelView model)
    {
        var entity = _mapper.Map<AdminStaff>(model);
        entity.Id = Guid.NewGuid();
        entity.CreatedTime = CoreHelper.SystemTimeNow;
        entity.CreatedBy = CurrentUserId;

        await _unitOfWork.GetRepository<AdminStaff>().InsertAsync(entity);
        await _unitOfWork.SaveAsync();
        return BaseResponseModel<Guid>.Success(entity.Id);
    }

    public async Task<BaseResponseModel<string>> UpdateAsync(PUTAdminStaffModelView model)
    {
        var repo = _unitOfWork.GetRepository<AdminStaff>();
        var entity = await repo.Entities.FirstOrDefaultAsync(x => x.Id == model.Id && x.DeletedTime == null);
        if (entity == null)
            throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Không tìm thấy AdminStaff");

        _mapper.Map(model, entity);
        entity.LastUpdatedTime = CoreHelper.SystemTimeNow;
        entity.LastUpdatedBy = CurrentUserId;

        await repo.UpdateAsync(entity);
        await _unitOfWork.SaveAsync();
        return BaseResponseModel<string>.Success("Cập nhật thành công");
    }

    public async Task<BaseResponseModel<string>> DeleteAsync(Guid id)
    {
        var repo = _unitOfWork.GetRepository<AdminStaff>();
        var entity = await repo.Entities.FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null);
        if (entity == null)
            throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Không tìm thấy AdminStaff");

        entity.DeletedTime = CoreHelper.SystemTimeNow;
        entity.DeletedBy = CurrentUserId;
        await repo.UpdateAsync(entity);
        await _unitOfWork.SaveAsync();
        return BaseResponseModel<string>.Success("Xóa thành công");
    }

    public async Task<BaseResponseModel<GETAdminStaffModelView>> GetByIdAsync(Guid id)
    {
        var entity = await _unitOfWork.GetRepository<AdminStaff>().Entities
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null);

        if (entity == null)
            throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Không tìm thấy AdminStaff");

        return BaseResponseModel<GETAdminStaffModelView>.Success(_mapper.Map<GETAdminStaffModelView>(entity));
    }

    public async Task<BasePaginatedList<GETAdminStaffModelView>> GetAllAsync(int page, int size)
    {
        var query = _unitOfWork.GetRepository<AdminStaff>().Entities
            .AsNoTracking()
            .Where(x => x.DeletedTime == null)
            .OrderByDescending(x => x.CreatedTime);

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();
        var result = _mapper.Map<List<GETAdminStaffModelView>>(items);

        return new BasePaginatedList<GETAdminStaffModelView>(result, total, page, size);
    }
}