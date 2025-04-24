using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.WorkingHourModelViews;
using BeautySpa.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Services.Service
{
    public class WorkingHourService : IWorkingHourService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public WorkingHourService(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BasePaginatedList<GETWorkingHourModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.WorkingHours
                .AsNoTracking()
                .Where(w => w.DeletedTime == null)
                .OrderBy(w => w.DayOfWeek);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var result = _mapper.Map<IReadOnlyCollection<GETWorkingHourModelViews>>(items);

            return new BasePaginatedList<GETWorkingHourModelViews>(result, totalCount, pageNumber, pageSize);
        }

        public async Task<GETWorkingHourModelViews> GetByIdAsync(Guid id)
        {
            var entity = await _context.WorkingHours
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == id && w.DeletedTime == null);

            if (entity == null) throw new Exception("Working hour not found");
            return _mapper.Map<GETWorkingHourModelViews>(entity);
        }

        public async Task<Guid> CreateAsync(POSTWorkingHourModelViews model)
        {
            var entity = _mapper.Map<WorkingHour>(model);
            entity.Id = Guid.NewGuid();
            entity.CreatedTime = DateTimeOffset.UtcNow;

            _context.WorkingHours.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task UpdateAsync(PUTWorkingHourModelViews model)
        {
            var entity = await _context.WorkingHours.FirstOrDefaultAsync(w => w.Id == model.Id && w.DeletedTime == null);
            if (entity == null) throw new Exception("Working hour not found");

            _mapper.Map(model, entity);
            entity.LastUpdatedTime = DateTimeOffset.UtcNow;

            _context.WorkingHours.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.WorkingHours.FirstOrDefaultAsync(w => w.Id == id && w.DeletedTime == null);
            if (entity == null) throw new Exception("Working hour not found");

            entity.DeletedTime = DateTimeOffset.UtcNow;
            _context.WorkingHours.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
