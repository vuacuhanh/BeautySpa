using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Contract.Services.Interface;
using BeautySpa.Core.Base;
using BeautySpa.ModelViews.MessageModelViews;
using BeautySpa.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace BeautySpa.Services.Service
{
    public class MessageService : IMessageService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public MessageService(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BasePaginatedList<GETMessageModelViews>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Messages
                .AsNoTracking()
                .Where(m => m.DeletedTime == null)
                .OrderByDescending(m => m.CreatedTime);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var result = _mapper.Map<IReadOnlyCollection<GETMessageModelViews>>(items);

            return new BasePaginatedList<GETMessageModelViews>(result, totalCount, pageNumber, pageSize);
        }

        public async Task<GETMessageModelViews> GetByIdAsync(Guid id)
        {
            var entity = await _context.Messages
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.DeletedTime == null);

            if (entity == null) throw new Exception("Message not found");
            return _mapper.Map<GETMessageModelViews>(entity);
        }

        public async Task<Guid> CreateAsync(POSTMessageModelViews model)
        {
            var entity = _mapper.Map<Message>(model);
            entity.Id = Guid.NewGuid();
            entity.CreatedTime = DateTimeOffset.UtcNow;

            _context.Messages.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task UpdateAsync(PUTMessageModelViews model)
        {
            var entity = await _context.Messages.FirstOrDefaultAsync(m => m.Id == model.Id && m.DeletedTime == null);
            if (entity == null) throw new Exception("Message not found");

            _mapper.Map(model, entity);
            entity.LastUpdatedTime = DateTimeOffset.UtcNow;

            _context.Messages.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Messages.FirstOrDefaultAsync(m => m.Id == id && m.DeletedTime == null);
            if (entity == null) throw new Exception("Message not found");

            entity.DeletedTime = DateTimeOffset.UtcNow;
            _context.Messages.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
