using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.WorkingHourModelViews;
using BeautySpa.Services.Validations.WorkingHourValidator;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Services.Service
{
    public class WorkingHourService : IWorkingHourService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WorkingHourService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel<BasePaginatedList<GETWorkingHourModelViews>>> GetAllAsync(int pageNumber, int pageSize)
        {
            IQueryable<WorkingHour> query = _unitOfWork.GetRepository<WorkingHour>()
                .Entities.AsNoTracking()
                .Include(x => x.SpaBranchLocation!)
                .Where(x => x.DeletedTime == null)
                .OrderByDescending(x => x.CreatedTime);

            var result = await _unitOfWork.GetRepository<WorkingHour>().GetPagging(query, pageNumber, pageSize);
            var data = result.Items.Select(x => _mapper.Map<GETWorkingHourModelViews>(x)).ToList();

            return BaseResponseModel<BasePaginatedList<GETWorkingHourModelViews>>.Success(new BasePaginatedList<GETWorkingHourModelViews>(data, result.TotalItems, pageNumber, pageSize));
        }

        public async Task<BaseResponseModel<GETWorkingHourModelViews>> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.GetRepository<WorkingHour>()
                .Entities.Include(x => x.SpaBranchLocation!)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedTime == null)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Working hour not found.");

            return BaseResponseModel<GETWorkingHourModelViews>.Success(_mapper.Map<GETWorkingHourModelViews>(entity));
        }

        public async Task<BaseResponseModel<Guid>> CreateAsync(POSTWorkingHourModelViews model)
        {
            await new POSTWorkingHourModelValidator().ValidateAndThrowAsync(model);

            var entity = _mapper.Map<WorkingHour>(model);
            entity.Id = Guid.NewGuid();
            entity.CreatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<WorkingHour>().InsertAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<Guid>.Success(entity.Id);
        }

        public async Task<BaseResponseModel<string>> UpdateAsync(PUTWorkingHourModelViews model)
        {
            await new PUTWorkingHourModelValidator().ValidateAndThrowAsync(model);

            var entity = await _unitOfWork.GetRepository<WorkingHour>().GetByIdAsync(model.Id)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Working hour not found.");

            _mapper.Map(model, entity);
            entity.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.SaveAsync();
            return BaseResponseModel<string>.Success("Updated successfully.");
        }

        public async Task<BaseResponseModel<string>> DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.GetRepository<WorkingHour>().GetByIdAsync(id)
                ?? throw new ErrorException(404, ErrorCode.NotFound, "Working hour not found.");

            entity.DeletedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Deleted successfully.");
        }
    }
}
