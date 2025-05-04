using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Repositories.IUOW;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.Core.Infrastructure;
using BeautySpa.Core.Utils;
using BeautySpa.ModelViews.AppointmentModelViews;
using BeautySpa.Services.Validations.AppoitmentValidator;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Services.Service
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private string CurrentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<BaseResponseModel<BasePaginatedList<GETAppointmentModelViews>>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid pagination values.");

            var query = _unitOfWork.GetRepository<Appointment>()
                .Entities.Where(a => a.DeletedTime == null)
                .OrderByDescending(a => a.AppointmentDate);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var mappedItems = _mapper.Map<IReadOnlyCollection<GETAppointmentModelViews>>(items);

            var result = new BasePaginatedList<GETAppointmentModelViews>(mappedItems, totalCount, pageNumber, pageSize);
            return BaseResponseModel<BasePaginatedList<GETAppointmentModelViews>>.Success(result);
        }

        public async Task<BaseResponseModel<GETAppointmentModelViews>> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.GetRepository<Appointment>()
                .Entities.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id && a.DeletedTime == null);

            if (entity == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Appointment not found.");

            return BaseResponseModel<GETAppointmentModelViews>.Success(_mapper.Map<GETAppointmentModelViews>(entity));
        }

        public async Task<BaseResponseModel<Guid>> CreateAsync(POSTAppointmentModelViews model)
        {
            var validator = new POSTAppointmentModelViewsValidator();
            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

            var entity = _mapper.Map<Appointment>(model);
            entity.Id = Guid.NewGuid();
            entity.CreatedTime = CoreHelper.SystemTimeNow;
            entity.CreatedBy = CurrentUserId;
            entity.LastUpdatedTime = entity.CreatedTime;
            entity.LastUpdatedBy = CurrentUserId;

            await _unitOfWork.GetRepository<Appointment>().InsertAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<Guid>.Success(entity.Id);
        }

        public async Task<BaseResponseModel<string>> UpdateAsync(PUTAppointmentModelViews model)
        {
            var validator = new PUTAppointmentModelViewsValidator();
            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

            var entity = await _unitOfWork.GetRepository<Appointment>()
                .Entities.FirstOrDefaultAsync(a => a.Id == model.Id && a.DeletedTime == null);

            if (entity == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Appointment not found.");

            _mapper.Map(model, entity);
            entity.LastUpdatedTime = CoreHelper.SystemTimeNow;
            entity.LastUpdatedBy = CurrentUserId;

            await _unitOfWork.GetRepository<Appointment>().UpdateAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Appointment updated successfully.");
        }

        public async Task<BaseResponseModel<string>> DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.GetRepository<Appointment>()
                .Entities.FirstOrDefaultAsync(a => a.Id == id && a.DeletedTime == null);

            if (entity == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Appointment not found.");

            entity.DeletedTime = CoreHelper.SystemTimeNow;
            entity.DeletedBy = CurrentUserId;

            await _unitOfWork.GetRepository<Appointment>().UpdateAsync(entity);
            await _unitOfWork.SaveAsync();

            return BaseResponseModel<string>.Success("Appointment deleted successfully.");
        }
    }
}
