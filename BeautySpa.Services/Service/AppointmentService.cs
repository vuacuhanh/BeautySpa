using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.AppointmentModelViews;
using BeautySpa.Repositories.Context;
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

        public async Task<BasePaginatedList<GETAppointmentModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Appointments
                .AsNoTracking()
                .Where(a => a.DeletedTime == null)
                .OrderByDescending(a => a.AppointmentDate);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var result = _mapper.Map<IReadOnlyCollection<GETAppointmentModelViews>>(items);

            return new BasePaginatedList<GETAppointmentModelViews>(result, totalCount, pageNumber, pageSize);
        }

        public async Task<GETAppointmentModelViews> GetByIdAsync(Guid id)
        {
            var entity = await _context.Appointments
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id && a.DeletedTime == null);

            if (entity == null) throw new Exception("Appointment not found");
            return _mapper.Map<GETAppointmentModelViews>(entity);
        }

        public async Task<Guid> CreateAsync(POSTAppointmentModelViews model)
        {
            var entity = _mapper.Map<Appointment>(model);
            entity.Id = Guid.NewGuid();
            entity.CreatedTime = DateTimeOffset.UtcNow;

            _context.Appointments.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task UpdateAsync(PUTAppointmentModelViews model)
        {
            var entity = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == model.Id && a.DeletedTime == null);
            if (entity == null) throw new Exception("Appointment not found");

            _mapper.Map(model, entity);
            entity.LastUpdatedTime = DateTimeOffset.UtcNow;

            _context.Appointments.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id && a.DeletedTime == null);
            if (entity == null) throw new Exception("Appointment not found");

            entity.DeletedTime = DateTimeOffset.UtcNow;
            _context.Appointments.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
