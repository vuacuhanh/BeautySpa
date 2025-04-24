using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.NotificationModelViews;
using BeautySpa.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Services.Service
{
    public class NotificationService : INotificationService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public NotificationService(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BasePaginatedList<GETNotificationModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Notifications
                .AsNoTracking()
                .Where(n => n.DeletedTime == null)
                .OrderByDescending(n => n.CreatedTime);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var result = _mapper.Map<IReadOnlyCollection<GETNotificationModelViews>>(items);

            return new BasePaginatedList<GETNotificationModelViews>(result, totalCount, pageNumber, pageSize);
        }

        public async Task<GETNotificationModelViews> GetByIdAsync(Guid id)
        {
            var entity = await _context.Notifications
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.Id == id && n.DeletedTime == null);

            if (entity == null) throw new Exception("Notification not found");
            return _mapper.Map<GETNotificationModelViews>(entity);
        }

        public async Task<Guid> CreateAsync(POSTNotificationModelViews model)
        {
            var entity = _mapper.Map<Notification>(model);
            entity.Id = Guid.NewGuid();
            entity.CreatedTime = DateTimeOffset.UtcNow;

            _context.Notifications.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task UpdateAsync(PUTNotificationModelViews model)
        {
            var entity = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == model.Id && n.DeletedTime == null);
            if (entity == null) throw new Exception("Notification not found");

            _mapper.Map(model, entity);
            entity.LastUpdatedTime = DateTimeOffset.UtcNow;

            _context.Notifications.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id && n.DeletedTime == null);
            if (entity == null) throw new Exception("Notification not found");

            entity.DeletedTime = DateTimeOffset.UtcNow;
            _context.Notifications.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
