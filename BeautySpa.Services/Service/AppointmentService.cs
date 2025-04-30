using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.AppointmentModelViews;
using BeautySpa.Repositories.Context;
using BeautySpa.Services.Validations.AppoitmentValidator;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Services.Service
{
    public class AppointmentService : IAppointmentService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public AppointmentService(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel<BasePaginatedList<GETAppointmentModelViews>>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, "Invalid pagination values.");

            IQueryable<Appointment> query = _context.Appointments
                .AsNoTracking()
                .Where(a => a.DeletedTime == null)
                .OrderByDescending(a => a.AppointmentDate);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var mappedItems = _mapper.Map<IReadOnlyCollection<GETAppointmentModelViews>>(items);

            var result = new BasePaginatedList<GETAppointmentModelViews>(mappedItems, totalCount, pageNumber, pageSize);
            return BaseResponseModel<BasePaginatedList<GETAppointmentModelViews>>.Success(result);
        }

        public async Task<BaseResponseModel<GETAppointmentModelViews>> GetByIdAsync(Guid id)
        {
            var entity = await _context.Appointments
                .AsNoTracking()
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
            entity.CreatedTime = DateTimeOffset.UtcNow;

            _context.Appointments.Add(entity);
            await _context.SaveChangesAsync();

            return BaseResponseModel<Guid>.Success(entity.Id);
        }

        public async Task<BaseResponseModel<string>> UpdateAsync(PUTAppointmentModelViews model)
        {
            var validator = new PUTAppointmentModelViewsValidator();
            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.InvalidInput, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

            var entity = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == model.Id && a.DeletedTime == null);

            if (entity == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Appointment not found.");

            _mapper.Map(model, entity);
            entity.LastUpdatedTime = DateTimeOffset.UtcNow;

            _context.Appointments.Update(entity);
            await _context.SaveChangesAsync();

            return BaseResponseModel<string>.Success("Appointment updated successfully.");
        }

        public async Task<BaseResponseModel<string>> DeleteAsync(Guid id)
        {
            var entity = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id && a.DeletedTime == null);

            if (entity == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NotFound, "Appointment not found.");

            entity.DeletedTime = DateTimeOffset.UtcNow;
            _context.Appointments.Update(entity);
            await _context.SaveChangesAsync();

            return BaseResponseModel<string>.Success("Appointment deleted successfully.");
        }
    }
}
